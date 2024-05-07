#ifndef JUDGECORESERVER_COMPILER_COMPILERESULT_H
#define JUDGECORESERVER_COMPILER_COMPILERESULT_H

#include <optional>
#include <string>

/**
 * @brief CompileResult is the result of compiling a source code.
 */
struct CompileResult
{
	//! @brief Exit code of the compiler
	int exitCode{};
	//! @brief Output of the compiler
	std::string message;
	//! @brief Path to the output program, null if compile failed
	std::optional<std::string> executablePath;

	CompileResult(int exitCode, std::string message, std::optional<std::string> outputProgram)
		: exitCode(exitCode), message(std::move(message)), executablePath(std::move(outputProgram))
	{
	}

	CompileResult() = default;
};

#endif//JUDGECORESERVER_COMPILER_COMPILERESULT_H
