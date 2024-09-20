#pragma once

#ifndef HIMU_JUDGE_CORE_PROCESS_H
#define HIMU_JUDGE_CORE_PROCESS_H

#include "ProcessStartInfo.h"
#include "ProcessStartInfoBuilder.h"

namespace himu
{

struct Process
{
	enum class ExecutionResultType
	{
		Success,
		TimeLimitExceeded,
		MemoryLimitExceeded,
		UnknownError
	};

	ExecutionResultType Type = ExecutionResultType::Success;
	int ExitCode             = 0;
	int64_t TimeUsed         = 0; /* in milliseconds */
	int64_t MemoryUsed       = 0; /* in bytes */
	/* the output of the process, empty if RedirectStandardOutput is false or OutputRedirection is not null */
	std::string StdOutput;
	/* the error of the process */
	std::string StdError;

	/**
	 * @brief Start a process with the given information.
	 * This function will block until the process is terminated.
	 * info may be modified by this function.
	 * 
	 * @param info The information of the process to start.
	 * @param result The result of the process.
	 * @return Process The result of the process. std::nullopt if the process cannot be started.
	 */
	static std::optional<Process> StartProcess(ProcessStartInfo &info);

	/**
	 * @brief Start a process with the given information.
	 * This function will block until the process is terminated.
	 * 
	 * @param builder The builder of the information of the process to start.
	 * @return Process The result of the process. std::nullopt if the process cannot be started.
	 */
	static std::optional<Process> StartProcess(std::function<void(ProcessStartInfoBuilder &)> builder);
};

}// namespace himu::stage

#endif//! HIMU_JUDGE_CORE_PROCESS_H
