/**
 * @file linux_code_runner.cpp
 * @brief Code runner implementation for Linux platform
 * This file is a part of HimuOnlineJudge project.
 * @version 1.1
 * Copyright(c) 2024 Himu, all rights reserved.
 * 
 * Update notes
 * Version 1.1 (2024/04/25):
 *  - Add a watcher to monitor the running time of the program, 
 *  and kill the program if it exceeds the limit
 *  - setrlimit is used to set the resource limit of the program
*/

#include <chrono>
#include <fmt/std.h>
#include <pthread.h>
#include <sys/resource.h>
#include <sys/wait.h>
#include <unistd.h>

#include "linux_code_runner.h"
#include "shared/logger.h"

namespace
{
	/**
	 * @brief Watcher thread to monitor the running time of the program, 
	 * kill the program if it exceeds the limit
	 */
	void *watcherProcess(void *param)
	{
		pid_t pidToWatch = *static_cast<pid_t *>(param);
		int maxTimeout   = himu::BaseCodeRunner::MAX_RUNTIMEMS / 1000;
		// MyLogger.debug("Watcher is watching program@{} (waiting {} seconds)\n", pidToWatch, maxTimeout);
		int sleeped = sleep(maxTimeout);
		if (kill(pidToWatch, 0) == 0)
		{
			MyLogger.info("Program (pid @{}) killed: timeout after {}s", pidToWatch, sleeped);
			kill(pidToWatch, SIGKILL);
		}
		pthread_exit(nullptr);
		return nullptr;
	}

	/**
	 * @brief Get the signal message by signal number
	 * @param sig signal number
	 * @return signal message
	 */
	std::string_view getSignalMessage(int sig)
	{
		const static std::unordered_map<int, std::string_view> signalMap = {
			{SIGHUP, "Hangup"},
			{SIGINT, "Interrupt by user input"},
			{SIGQUIT, "Quit"},
			{SIGILL, "Illegal instruction"},
			{SIGTRAP, "Trace/breakpoint trap"},
			{SIGABRT, "Aborted"},
			{SIGBUS, "Bus error"},
			{SIGFPE, "Floating point exception"},
			// The program was killed by the watcher
			{SIGKILL, "Killed by watcher: Program ran timed out"},
			// May be caused by the program exceeding the memory limit
			{SIGSEGV, "Segmentation fault: Access to illegal memory or memory overruns"},
			{SIGPIPE, "Broken pipe"},
			{SIGALRM, "Alarm clock"},
			{SIGTERM, "Terminated"},
			{SIGSTKFLT, "Stack fault"},
			{SIGCHLD, "Child exited"},
			{SIGCONT, "Continued"},
			{SIGSTOP, "Stopped (signal)"},
			{SIGTSTP, "Stopped"},
			{SIGTTIN, "Stopped (tty input)"},
			{SIGTTOU, "Stopped (tty output)"},
			{SIGURG, "Urgent I/O condition"},
			{SIGXCPU, "CPU time limit exceeded"},
			{SIGXFSZ, "File size limit exceeded"},
			{SIGVTALRM, "Virtual timer expired"},
			{SIGPROF, "Profiling timer expired"},
			{SIGIO, "I/O possible"},
			{SIGPWR, "Power failure"},
			{SIGSYS, "Bad system call"},
		};

		if (signalMap.count(sig))
		{
			return signalMap.at(sig);
		}
		return "Unknown signal";
	}

}// namespace

namespace himu::code_runner
{

	LinuxCodeRunner::_RunForkResult
	LinuxCodeRunner::_runFork(const char *inputFile, const char *outputFile, const char *programPath) const
	{
		using namespace std::literals;

		std::chrono::high_resolution_clock::time_point start = std::chrono::high_resolution_clock::now();
		FILE *inputStream                                    = fopen(inputFile, "r");
		FILE *outputStream                                   = fopen(outputFile, "w");

		if (inputStream == nullptr || outputStream == nullptr)
		{
			perror("fopen");
			exit(1);
		}

		int inputFd  = fileno(inputStream);
		int outputFd = fileno(outputStream);
		int pid      = fork();
		if (pid == -1)
		{
			MyLogger.error("fatal error: cannot fork process: {}", strerror(errno));
			exit(EXIT_FAILURE);
		}
		else if (pid == 0)
		{
			// redirect input and output
			if (dup2(inputFd, STDIN_FILENO) < 0 || dup2(outputFd, STDOUT_FILENO) < 0)
			{
				MyLogger.error("fatal: dup2 error: {}", strerror(errno));
				std::exit(EXIT_FAILURE);
			}

			if (execl(programPath, programPath, nullptr) < 0)
			{
				MyLogger.error("Cannot execute program: {}", programPath);
				std::exit(EXIT_FAILURE);
			}
			std::exit(0);
		}
		else
		{
			fsync(outputFd);
			close(outputFd);
			close(inputFd);
			// set resource limit
			rlimit limit = {
				.rlim_cur = himu::BaseCodeRunner::MAX_MEMORYBYTE,
				.rlim_max = himu::BaseCodeRunner::MAX_MEMORYBYTE,
			};
			if (prlimit(pid, RLIMIT_AS, &limit, nullptr) < 0)
			{
				MyLogger.error("prlimit error: {}", strerror(errno));
				exit(EXIT_FAILURE);
			}

			// start watcher thread
			pthread_t watcher;
			pthread_create(&watcher, nullptr, watcherProcess, &pid);
			// collect result and clean up
			int status;
			_RunForkResult result = {};
			rusage usage          = {};
			wait4(pid, &status, 0, &usage);
			pthread_cancel(watcher);
			pthread_join(watcher, nullptr);

			// normal exit
			if (WIFEXITED(status))
			{
				std::chrono::high_resolution_clock::time_point end = std::chrono::high_resolution_clock::now();
				long runtime = std::chrono::duration_cast<std::chrono::milliseconds>(end - start).count();
				long memory  = usage.ru_maxrss;
				MyLogger.trace(
					"program (@pid = {}) exited with status={}, runtime={}, memory={}\n", pid, WEXITSTATUS(status),
					runtime, memory);
				result = {.status = WEXITSTATUS(status), .signal = 0, .runtime = runtime, .memory = memory};
				return result;
			}
			// killed by signal
			else if (WIFSIGNALED(status))
			{
				MyLogger.trace("program (@pid = {}) killed by signal {}\n", pid, WTERMSIG(status));
				result = {.status = -1, .signal = WTERMSIG(status), .runtime = 0, .memory = 0};
				return result;
			}
			// should not reach here
			else [[unlikely]]
			{
				MyLogger.trace("program (@pid = {}) exited with status {}\n", pid, status);
				result = {.status = status, .signal = 0, .runtime = 0, .memory = 0};
				return result;
			}
		}
	}

	CodeRunnerResult LinuxCodeRunner::runOnFile(
		const fs::path &programPath, const fs::path &inputPath, const fs::path &outputPath) noexcept
	{
		const auto &result = _runFork(inputPath.c_str(), outputPath.c_str(), programPath.c_str());

		CodeRunnerResult codeRunnerResult {};
		if (result.signal != 0)
		{
			codeRunnerResult.exitCode = result.signal;
			codeRunnerResult.message  = getSignalMessage(result.signal);
		}
		else
		{
			codeRunnerResult.exitCode             = result.status;
			codeRunnerResult.resourceUsage.memory = result.memory;
			codeRunnerResult.resourceUsage.time   = result.runtime;
		}
		return codeRunnerResult;
	}

	himu::dto::ProgramExecutionStatus LinuxCodeRunner::getExecStatusFromSignal(int sig) const noexcept
	{
		switch (sig)
		{
		case SIGKILL:
		case SIGXCPU:
			return himu::dto::ProgramExecutionStatus::TIME_LIMIT_EXCEEDED;
		case SIGSEGV:
		case SIGXFSZ:
			return himu::dto::ProgramExecutionStatus::MEMORY_LIMIT_EXCEEDED;
		default:
			return himu::dto::ProgramExecutionStatus::RUNTIME_ERROR;
		}
	}

}// namespace himu::code_runner