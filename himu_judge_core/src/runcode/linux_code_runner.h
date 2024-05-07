//
// Created by LiuHuan on 2023/11/16.
//

#ifndef JUDGECORESERVER_LINUX_CODE_RUNNER_H_
#define JUDGECORESERVER_LINUX_CODE_RUNNER_H_

#include "basic_code_runner.h"
#include "shared/global_variables.h"

namespace himu::code_runner
{
	/**
	 * @brief Code runner implementation for Linux platform
	 * Provide the ability to run the program on Linux platform. Also provide the platform information.
	 */
	class LinuxCodeRunner : public BaseCodeRunner
	{
	public:
		[[nodiscard]]
		std::string_view platform() const override
		{
			return "Linux";
		}

		/**
		 * @brief Run the program with the given input and output file
		 * @param programPath path of the program
		 * @param inputPath input file path
		 * @param outputPath output file path
		 * @return CodeRunnerResult result of the running, 
		 * including exit code, time used and memory used
		 */
		[[nodiscard]]
		CodeRunnerResult runOnFile(
			const fs::path &programPath, const fs::path &inputPath,
			const fs::path &outputPath) noexcept override;

		explicit LinuxCodeRunner(const CodeRunnerLimit &limit)
			: _runnerLimit(limit), _successCreated(true)
		{
			if (limit.maxExecuteTimeLimitTick <= 0
				|| limit.maxExecuteTimeLimitTick > himu::system_limit::runner::MaxExecTimeMs * 10000L
				|| limit.maxMemoryLimitByte <= 0
				|| limit.maxMemoryLimitByte > himu::system_limit::runner::MaxExecMemoryByte)
			{
				MyLogger.error(
					"Failed to created code runner: illegal limit ({0}, {1})\n",
					limit.maxExecuteTimeLimitTick, limit.maxMemoryLimitByte);
				_successCreated = false;
			}
		}

		virtual bool successCreated() const noexcept override
		{
			return _successCreated;
		}

		[[nodiscard]]
		virtual himu::dto::ProgramExecutionStatus getExecStatusFromSignal(int signal) const noexcept override;
	private:
		struct _RunForkResult
		{
			int status = 0;
			int signal = 0;
			long runtime = 0;
			long memory = 0;
		};

		_RunForkResult
		_runFork(const char *inputFile, const char *outputFile, const char *programPath) const;

		himu::dto::ProblemLimit _runnerLimit;
		bool _successCreated = true;
	};

}// namespace himu::code_runner
#endif// JUDGECORESERVER_LINUX_CODE_RUNNER_H_
