#include "pch.h"
#include "shared/Shared.h"
#include "procedure/JudgeProcedure.h"
#include "procedure/process/Process.h"
#include "utils/Utils.h"
#include "procedure/comparer/Comparer.h"

#include <fmt/std.h>// for std::filesystem::path

namespace
{

constexpr int kMaxCompilerMemoryLimit = 1024 * 1024 * 1024;// 1 GB

std::pair<std::string, std::filesystem::path> GenerateCompilerCommand(const judge::JudgeCompilerInfo &compilerInfo,
																	  const std::string &sourceUri,
																	  int64_t commitId)
{
	std::string command = compilerInfo.command();
	himu::utils::ReplaceAllSubString(command, "{src}", sourceUri);
	// Any output file should be placed in a temporary directory
	std::filesystem::path compileTargetPath = std::filesystem::temp_directory_path() / std::to_string(commitId);
	himu::utils::ReplaceAllSubString(command, "{out}", compileTargetPath.string());
#if _WIN32
	// Windows requires the .exe extension
	compileTargetPath += ".exe";
#endif
	return std::make_pair(command, compileTargetPath);
}

}// namespace

namespace himu
{

struct JudgeWin32ProcedureImpl
{
public:
	[[nodiscard]]
	std::optional<CompileResult> CallCompiler(int64_t commitId,
											  const std::string &sourceUri,
											  const judge::JudgeCompilerInfo &compilerInfo);

	std::optional<judge::JudgeTestPointResult> RunTestPoint(const std::filesystem::path &compiledPath,
															const judge::JudgeTestPoint &testPoint,
															const judge::JudgeTask &task,
															const std::filesystem::path &outputPath);

	[[nodiscard]]
	void SetCompileFailed(judge::JudgeResult &result, const judge::JudgeTask &task, const CompileResult &compileResult);
};

std::optional<judge::JudgeResult> JudgeWin32Procedure::Execute(const judge::JudgeTask &task)
{
	auto result        = std::make_optional<judge::JudgeResult>();
	int64_t commitId   = task.commitid();
	auto &sourceUri    = task.sourceuri();
	auto &compilerInfo = task.compiler();

	result->set_commitid(commitId);
	result->set_status(judge::INTERNAL_ERROR);
	LOGGER_VERBOSE("<commit {0}> ================= Compiler Stage  =================", task.commitid());
	LOGGER->info("<commit {}> Compiling {} with compiler \"{}\" ({})", commitId, sourceUri, compilerInfo.name(),
				 compilerInfo.command());
	auto compilerResult = _impl->CallCompiler(commitId, sourceUri, compilerInfo);
	if (!compilerResult.has_value())
	{
		LOGGER->error("<commit {0}> fatal errors in compiler stage", commitId);
		return std::nullopt;
	}
	if (compilerResult->ExitCode != 0)
	{
		LOGGER->info("<commit {0}> Compile failed, procedure returned", commitId);
		_impl->SetCompileFailed(result.value(), task, compilerResult.value());
		return result;
	}

	auto &target = compilerResult.value();
	LOGGER->info("<commit {0}> The compilation completed, target file: {1}", commitId, target.TargetPath);
	LOGGER_VERBOSE("<commit {0}> ================= Test Stage  =================", commitId);
	auto &testPoints = task.testpoints();
	LOGGER->info("<commit {}> Running test points (count = {}) ...", commitId, testPoints.size());
	std::filesystem::path outputDir = std::filesystem::path(sourceUri).parent_path() / "output";
	if (!std::filesystem::exists(outputDir))
		std::filesystem::create_directories(outputDir);

	bool allPointsPassed = true;
	int pointTestedCount = 0;
	for (const auto &testPoint : testPoints)
	{
		LOGGER_VERBOSE("Running test point (input={}, expected={})", testPoint.input(), testPoint.expected());
		if (!allPointsPassed)
		{
			auto pointRes = result->add_testpointresults();
			pointRes->set_status(judge::SKIPPED);
			pointRes->set_testpointid(testPoint.testpointid());
			continue;
		}
		std::filesystem::path inputFilePath  = testPoint.input();
		std::filesystem::path outputFilePath = outputDir / fmt::format("{}.out", inputFilePath.stem());
		auto testPointResult = _impl->RunTestPoint(compilerResult->TargetPath, testPoint, task, outputFilePath);
		if (!testPointResult.has_value())
		{
			LOGGER->error("<commit {0}> fatal errors in test stage, procedure has been terminated.", commitId);
			allPointsPassed = false;
			result->set_status(judge::INTERNAL_ERROR);
			break;
		}
		
		pointTestedCount++;

		result->add_testpointresults()->CopyFrom(*testPointResult);
		if (testPointResult->status() != judge::ACCEPTED)
		{
			LOGGER_VERBOSE("<commit {0}> Test point ({1}) failed: status={2}, procedure returned", commitId,
						   inputFilePath, (int) testPointResult->status());
			allPointsPassed = false;
			result->set_status(testPointResult->status());
		}
		else
		{
			LOGGER_VERBOSE("<commit {0}> Test point (from {1}) passed.", commitId, inputFilePath);
		}
	}

	if (allPointsPassed)
	{
		LOGGER->info("<commit {0}> All test points passed.", commitId);
		result->set_status(judge::ACCEPTED);
	}
	else
	{
		LOGGER->info("<commit {0}> {1} testpoints had skipped for previous failure", commitId,
					 testPoints.size() - pointTestedCount);
	}

	return result;
}

const char *JudgeWin32Procedure::Platform() const
{
	return "Win32";
}

std::optional<CompileResult> JudgeWin32ProcedureImpl::CallCompiler(int64_t commitId,
																   const std::string &sourceUri,
																   const judge::JudgeCompilerInfo &compilerInfo)

{
	auto result = std::make_optional<CompileResult>();
	ProcessStartInfo startInfo;
	auto [command, targetPath]      = GenerateCompilerCommand(compilerInfo, sourceUri, commitId);
	startInfo.Command               = command;
	startInfo.MemoryLimit           = kMaxCompilerMemoryLimit;
	startInfo.TimeLimit             = compilerInfo.timeout();
	startInfo.RedirectStandardError = true;
	auto compileProcess             = Process::StartProcess(startInfo);
	if (compileProcess.has_value())
	{
		LOGGER->info("<commit {}> Compiler process (from {}) finished with exit code {}.", commitId, startInfo.Command,
					 compileProcess->ExitCode);
		result->ExitCode = compileProcess->ExitCode;
		if (compileProcess->ExitCode != 0)
		{
			LOGGER_VERBOSE("<commit {}> Compile failed ({}): {}", commitId, compileProcess->ExitCode,
						   compileProcess->StdError);
			result->Message = compileProcess->StdError;
		}
		else
		{
			result->TargetPath = targetPath;
		}
	}
	else
	{
		FATAL_ERROR_RETURN("Failed to start compiler process.", std::nullopt);
	}

	return result;
}

std::optional<judge::JudgeTestPointResult>
JudgeWin32ProcedureImpl::RunTestPoint(const std::filesystem::path &compiledPath,
									  const judge::JudgeTestPoint &testPoint,
									  const judge::JudgeTask &task,
									  const std::filesystem::path &outputPath)
{
	auto result          = std::make_optional<judge::JudgeTestPointResult>();
	auto &input          = testPoint.input();
	auto &expected       = testPoint.expected();
	auto &limit          = task.limit();
	FILE *inputStream    = nullptr;
	FILE *outputStream   = nullptr;
	FILE *expectedStream = nullptr;
	bool success         = true;

	result->set_status(judge::ACCEPTED);
	result->set_testpointid(testPoint.testpointid());
	do
	{
		if (utils::OpenStdFile(&inputStream, input, "r") != 0)
			FATAL_ERROR_BREAK(success, "Failed to open input file.");
		if (utils::OpenStdFile(&outputStream, outputPath, "wb") != 0)
			FATAL_ERROR_BREAK(success, "Failed to open output file for write.");
		auto process = Process::StartProcess(
			[&](ProcessStartInfoBuilder &builder)
			{
				builder.Command(compiledPath.string());
				builder.MemoryLimit(limit.memory());
				builder.TimeLimit(limit.time());
				builder.InputRedirection(inputStream);
				builder.RedirectStandardOutput(true);
				builder.OutputRedirection(outputStream);
			});
		if (!process.has_value())
		{
			FATAL_ERROR_BREAK(success, "Failed to start process.");
		}

		LOGGER_VERBOSE("Test point process (from {}) finished with exit code {}.", compiledPath, process->ExitCode);
		result->mutable_usage()->set_memory(process->MemoryUsed);
		result->mutable_usage()->set_time(process->TimeUsed);
		if (process->Type != Process::ExecutionResultType::Success)
		{
			switch (process->Type)
			{
				case Process::ExecutionResultType::TimeLimitExceeded:
					result->set_status(judge::TIME_LIMIT_EXCEEDED);
					break;
				case Process::ExecutionResultType::MemoryLimitExceeded:
					result->set_status(judge::MEMORY_LIMIT_EXCEEDED);
					break;
				case Process::ExecutionResultType::UnknownError:
					result->set_status(judge::RUNTIME_ERROR);
					break;
				// This should not happen
				[[unlikely]] default:
					result->set_status(judge::INTERNAL_ERROR);
					break;
			}
			break;
		}

		fclose(inputStream);
		inputStream = nullptr;
		fclose(outputStream);
		result->set_output(outputPath.string());

		if (utils::OpenStdFile(&outputStream, outputPath, "r") != 0)
			FATAL_ERROR_BREAK(success, "Failed to open output file for read.");
		if (utils::OpenStdFile(&expectedStream, expected, "r") != 0)
			FATAL_ERROR_BREAK(success, "Failed to open expected file.");
		auto comparer = ComparerBuilder::Get(ComparerMode::Line);
		if (comparer == nullptr)
			FATAL_ERROR_BREAK(success, "Failed to create comparer.");

		auto diff = comparer->Compare(outputStream, expectedStream);
		if (!diff.has_value())
			FATAL_ERROR_BREAK(success, "Failed to compare output and expected.");
		if (diff->pos() != 0)
		{
			result->set_status(judge::WRONG_ANSWER);
			*result->mutable_diff() = *diff;
		}
	} while (false);

	if (inputStream != nullptr)
		fclose(inputStream);
	if (outputStream != nullptr)
		fclose(outputStream);
	if (expectedStream != nullptr)
		fclose(expectedStream);
	if (!success)
		return std::nullopt;

	return result;
}

void JudgeWin32ProcedureImpl::SetCompileFailed(judge::JudgeResult &result,
											   const judge::JudgeTask &task,
											   const CompileResult &compileResult)
{
	result.set_status(judge::COMPILE_ERROR);
	result.set_message(compileResult.Message);
	result.set_commitid(task.commitid());
	for (const auto &testPoint : task.testpoints())
	{
		auto pointRes = result.add_testpointresults();
		pointRes->set_status(judge::SKIPPED);
		pointRes->set_testpointid(testPoint.testpointid());
	}
}

}// namespace himu