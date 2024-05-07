#include "gcc_compiler_provider.h"

#include <fmt/ranges.h>
#include <sys/wait.h>
#include <unistd.h>

#include "shared/logger.h"

namespace himu::compiler
{

CompileResult GccCompilerProvider::compile(fs::path sourcePath)
{
	char **argv               = new char *[_compilerArgs.size() + 5];
	argv[0]                   = strdup(_compilerPath.c_str());
	argv[1]                   = strdup(sourcePath.c_str());
	argv[2]                   = strdup("-o");
	std::string outputProgram = sourcePath.replace_extension("").string();
	argv[3]                   = strdup(outputProgram.c_str());
	for (int i = 0; i < _compilerArgs.size(); ++i)
	{
		argv[i + 4] = strdup(_compilerArgs[i].c_str());
	}
	argv[_compilerArgs.size() + 4] = nullptr;

	/* start compiler by fork */
	auto message =
		fmt::format("Starting compile: {}", fmt::join(argv, argv + _compilerArgs.size() + 4, " "));
	MyLogger.debug(message);

	int pfd[2];
	if (pipe(pfd) == -1)
	{
		throw std::runtime_error("pipe failed");
	}
	int pid = fork();
	if (pid == -1)
	{
		throw std::runtime_error("fork failed");
	}
	else if (pid == 0)
	{
		dup2(pfd[1], STDOUT_FILENO);
		dup2(pfd[1], STDERR_FILENO);
		close(pfd[0]);
		if (execvp(_compilerPath.c_str(), argv) == -1)
		{
			std::string info = fmt::format("execv g++ failed", strerror(errno));
			throw std::system_error(errno, std::system_category(), info);
		}
	}
	else
	{
		close(pfd[1]);
		char buf[1024];

		std::string compilerOutput;
		compilerOutput.reserve(BUFSIZ);

		ssize_t n;
		while ((n = read(pfd[0], buf, sizeof(buf))) > 0)
		{
			compilerOutput.append(buf, n);
		}

		close(pfd[0]);
		int status;
		waitpid(pid, &status, 0);

		for (int i = 0; i < _compilerArgs.size() + 4; ++i)
		{
			free(argv[i]);
		}

		if (WIFEXITED(status)) [[likely]]
		{
			if (status != 0)
				return {WEXITSTATUS(status), compilerOutput, std::nullopt};
			else
				return {0, "ok", outputProgram};
		}
		else if (WIFSIGNALED(status)) [[unlikely]]
		{
			return {WTERMSIG(status), compilerOutput, std::nullopt};
		}
		else
		{
			return {status, compilerOutput, std::nullopt};
		}
	}

	return {0, "ok", outputProgram};
}

GccCompilerProvider::GccCompilerProvider(const fs::path &compilerPath,
										 const std::initializer_list<std::string> &compilerArgs)
	: BaseCompilerProvider(compilerPath, compilerArgs)
{
}

}// namespace himu::compiler