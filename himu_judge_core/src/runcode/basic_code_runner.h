#ifndef JUDGECORESERVER_CODERUNNER_H_
#define JUDGECORESERVER_CODERUNNER_H_

#include "shared/bits.h"
#include "shared/dto/run_code_case_result.h"
#include "shared/dto/problem_limilt.h"
#include "shared/dto/program_execution_status.h"

namespace himu
{
	using CodeRunnerLimit = himu::dto::ProblemLimit;

	class BaseCodeRunner
	{
	public:
		/// Maximum time to wait for the program to finish (in milliseconds)
		static constexpr int MAX_RUNTIMEMS = 5000;

		/// Maximum memory that the program can use (in bytes)
		static constexpr int MAX_MEMORYBYTE = 1024 * 1024 * 1024;

		virtual ~BaseCodeRunner() = default;

		/**
     * @brief Run the program, redirecting stdin and stdout to the program.
     * @remark This function should be called after the compilation is done
     * @param programPath The path of the program
     * @param inputPath The path of input files directory to redirect stdin
     * @param outputPath The path of output files directory to redirect stdout
     * @return The result of the code runner
     */
		[[nodiscard]]
		virtual CodeRunnerResult
		runOnFile(const fs::path &programPath, const fs::path &inputPath, const fs::path &outputPath) noexcept = 0;

		[[nodiscard]]
		virtual std::string_view platform() const = 0;

		[[nodiscard]]
		virtual bool successCreated() const noexcept
		{
			return true;
		}

		/**
		 * @brief If the program is killed by signal, get the status from the signal.
		 * Different platform may have different signal number.
		 */
		[[nodiscard]]
		virtual himu::dto::ProgramExecutionStatus getExecStatusFromSignal(int signal) const noexcept = 0;
	};

}// namespace himu

#endif// JUDGECORESERVER_CODERUNNER_H_
