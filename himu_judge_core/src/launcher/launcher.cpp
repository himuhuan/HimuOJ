#include "launcher.h"
#include "compiler/compiler_provider.h"
#include "result_compare/stream_result_compare.h"
#include "runcode/runcode.h"
#include "shared/global_variables.h"
#include <cstdio>

using namespace himu;

namespace himu::launcher
{

	Launcher::Launcher(size_t maxThread) : _pool(maxThread)
	{
		// start waiting for tasks
		_pool.start();
	}

	Launcher::~Launcher()
	{
		_pool.stop();
	}

	void Launcher::submitTask(long commitId)
	{
		_pool.submit(launcherTask, commitId);
		MyLogger.trace("commit@{0} has submitted to task pool", commitId);
	}

	void Launcher::launcherTask(long commitId)
	{
		MyLogger.debug("Starting commit@{0}", commitId);

		// connect to database and get data
		auto threadId = std::this_thread::get_id();
		auto context  = gDbContextFactory->getContext();
		if (context == nullptr)
		{
			MyLogger.error("commit@{} (thread@{}): cannot create connection to database", commitId, threadId);
			return;
		}
#if _DEBUG
		MyLogger.trace("commit@{} (thread@{}): connected to database", commitId, threadId);
#endif
		auto commit    = context->getCommit(commitId);
		auto testCases = context->getCommitTasks(commitId);
		if (commit == nullptr || !testCases.has_value() || commit->status != dto::ProgramExecutionStatus::PENDING)
		{
			MyLogger.error("commit@{0} is not found or it has already been tested", commitId);
			return;
		}

		MyLogger.debug(
			"Running task: commit@{0} (has {1} testpoints, using {2})", commitId, testCases->size(), commit->compiler);

		// compile stage
		auto compilerProvider = himu::createCompilerProvider(commit->compiler.c_str());
		// If web server has checked the compiler, this should not happen...
		if (compilerProvider == nullptr) [[unlikely]]
		{
			MyLogger.error("Compiler {0} does not exist or is not supported", commit->compiler);
			commit->status = dto::ProgramExecutionStatus::INTERNAL_ERROR;
			context->updateContext(*commit, *testCases);
			return;
		}

#if _DEBUG
		MyLogger.trace("Compiler {0} founded, start compiling ({1})", commit->compiler, commit->sourceUri);
#endif// ^^^ _DEBUG

		fs::path sourcePath = commit->sourceUri;
		auto compileResult  = compilerProvider->compile(sourcePath);
		if (compileResult.exitCode != 0)
		{
			MyLogger.debug(
				"commit@{0} compile failed({1}):\n{2}", commitId, compileResult.exitCode, compileResult.message);
			commit->status         = dto::ProgramExecutionStatus::COMPILE_ERROR;
			commit->compileMessage = std::make_unique<std::string>(compileResult.message);
			context->updateContext(*commit, *testCases);
			return;
		}

		// run stage
		auto problemLimit = context->getProblemLimit(commit->problemId);
		if (problemLimit == nullptr)
		{
			MyLogger.error("commit@{0} cannot get problem limit (from problem@{1})", commitId, commit->problemId);
			commit->status = dto::ProgramExecutionStatus::INTERNAL_ERROR;
			context->updateContext(*commit, *testCases);
			return;
		}
#if _DEBUG
		MyLogger.trace(
			"commit@{0} submited to problem@{1}(memory={2}B, time={3} ticks), starting running", commit->id,
			commit->problemId, problemLimit->maxMemoryLimitByte, problemLimit->maxExecuteTimeLimitTick);
#endif
		auto codeRunner = himu::createCodeRunner(*problemLimit);
		// If web server has checked the problem limit, this should not happen...
		if (codeRunner == nullptr) [[unlikely]]
		{
			MyLogger.error("fatal error: cannot create code runner");
			commit->status = dto::ProgramExecutionStatus::INTERNAL_ERROR;
			context->updateContext(*commit, *testCases);
			return;
		}
		commit->status = dto::ProgramExecutionStatus::RUNNING;
		// TODO: provide a option to decide whether to update the commit status during the running stage
		context->updateContext(*commit, *testCases);
		fs::path outputPath = fmt::format("commits/{}/output", commit->id);
		if (!fs::exists(outputPath))
		{
			MyLogger.trace("created output directory: {0}", outputPath);
			fs::create_directories(outputPath);
		}
		dto::ProgramExecutionStatus commitStatus = dto::ProgramExecutionStatus::ACCEPTED;
		for (auto &testCase : *testCases)
		{
			// User maybe want to rejudge the commit, so we should ignore the status of the previous testpoint
			// to avoid the status of the previous testpoint affect the current testpoint
			// TODO: provide a option to decide whether to skip the testpoint that has been tested
			MyLogger.trace("start running case@{0} => testpoint@{1}", testCase.id, testCase.testpointId);
			FILE *outputStream = nullptr, *answerStream = nullptr;
			auto outputFilePath = outputPath / std::to_string(testCase.id);
			auto runResult      = codeRunner->runOnFile(
                *compileResult.executablePath, testCase.testPoint->input, outputFilePath.u8string());
			testCase.runResult     = std::make_unique<dto::ExitCodeWithMessage>();
			testCase.resourceUsage = std::make_unique<dto::ResourceUsage>();
			if (runResult.exitCode != 0)
			{
				// The program exited with a non-zero status, not killed by signal
				if (runResult.message.empty())
				{
#if _DEBUG
					MyLogger.trace(
						"case@{0} => testpoint@{1} runtime error: exit code {2}", testCase.id, testCase.testpointId,
						runResult.exitCode);
#endif
					testCase.runResult->exitCode = runResult.exitCode;
					testCase.status              = dto::ProgramExecutionStatus::RUNTIME_ERROR;
					testCase.runResult->message  = fmt::format("RUNTIME_ERROR ({0})", runResult.exitCode);
				}
				else
				{
					// The program exited with a non-zero status, killed by signal
					MyLogger.trace(
						"case@{0} => testpoint@{1} killed by signal {2}", testCase.id, testCase.testpointId,
						runResult.exitCode);
					testCase.runResult->exitCode = runResult.exitCode;
					testCase.status              = codeRunner->getExecStatusFromSignal(runResult.exitCode);
					testCase.runResult->message  = runResult.message;
				}
				goto end_of_case;
			}

			testCase.resourceUsage->memory = runResult.resourceUsage.memory;
			testCase.resourceUsage->time   = runResult.resourceUsage.time;
			if (runResult.resourceUsage.memory > problemLimit->maxMemoryLimitByte)
			{
				testCase.status = dto::ProgramExecutionStatus::MEMORY_LIMIT_EXCEEDED;
				goto end_of_case;
			}
			if (runResult.resourceUsage.time > problemLimit->maxExecuteTimeLimitTick / TICKS_PER_MS)
			{
				testCase.status = dto::ProgramExecutionStatus::TIME_LIMIT_EXCEEDED;
				goto end_of_case;
			}
			MyLogger.trace("case@{0} => testpoint@{1} start comparing", testCase.id, testCase.testpointId);
			outputStream = std::fopen(outputFilePath.c_str(), "r");
			answerStream = std::fopen(testCase.testPoint->expected.c_str(), "r");
			// should not happen...
			if (outputStream == nullptr || answerStream == nullptr) [[unlikely]]
			{
				MyLogger.error(
					"case@{0} => testpoint@{1} cannot open output file or answer file", testCase.id,
					testCase.testpointId);
				testCase.status = dto::ProgramExecutionStatus::INTERNAL_ERROR;
				goto end_of_case;
			}
			testCase.diffResult = std::make_unique<dto::DiffResult>(
				result_compare::streamResultCompare<WordTokenReader>(outputStream, answerStream));
			testCase.status = (testCase.diffResult->actual == testCase.diffResult->expected)
								  ? dto::ProgramExecutionStatus::ACCEPTED
								  : dto::ProgramExecutionStatus::WRONG_ANSWER;
		end_of_case:
			if (outputStream != nullptr)
				std::fclose(outputStream);
			if (answerStream != nullptr)
				std::fclose(answerStream);
			if (testCase.status != dto::ProgramExecutionStatus::ACCEPTED)
			{
				commitStatus = testCase.status;
				// Has internal error, stop testing
				if (testCase.status == dto::ProgramExecutionStatus::INTERNAL_ERROR)
					break;
			}
		}
		commit->status = commitStatus;
		context->updateContext(*commit, *testCases);
		MyLogger.debug("commit@{0} finished, status: {1}", commitId, commitStatus);
	}

	std::unique_ptr<Launcher> createLauncher(size_t maxThread)
	{
		return std::make_unique<Launcher>(maxThread);
	}

}// namespace himu::launcher