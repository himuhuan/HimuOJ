#include "pch.h"
#include "Process.h"
#include <chrono>

#if _WIN32

#include <future>

namespace
{

constexpr size_t kMaxCommandLength = MAX_PATH;
constexpr size_t kBufferSize       = 4096;

DWORD ReadFromPipeAsString(HANDLE pipe, std::string &out, size_t maxLength)
{
	DWORD byteRead, totalRead {0};
	CHAR chBuf[kBufferSize] {};
	BOOL bSuccess = false;

	if (maxLength == 0)
		maxLength = LLONG_MAX;

	while (true)
	{
		bSuccess = ReadFile(pipe, chBuf, kBufferSize, &byteRead, NULL);
		if (!bSuccess || byteRead == 0)
			break;
		out.append(chBuf, byteRead);
		totalRead += byteRead;
		if (totalRead >= maxLength)
			break;
	}
	return totalRead;
}

DWORD ReadFromPipeToFile(HANDLE pipe, FILE *outputFile, size_t maxBytes)
{
	bool success;
	DWORD bytesRead;
	CHAR buffer[kBufferSize];
	size_t totalBytesRead = 0;

	if (maxBytes == 0)
		maxBytes = LLONG_MAX;

	for (;;)
	{
		if (!ReadFile(pipe, buffer, kBufferSize, &bytesRead, nullptr) || bytesRead == 0)
			break;
		// truncate if necessary
		if (totalBytesRead + bytesRead > maxBytes)
			bytesRead = maxBytes - totalBytesRead;
		if (fwrite(buffer, 1, bytesRead, outputFile) != bytesRead)
			FATAL_ERROR_BREAK(success, "fwrite Failed");
		totalBytesRead += bytesRead;
		if (totalBytesRead >= maxBytes)
			break;
	}
	return totalBytesRead;
}

DWORD WriteToPipe(HANDLE pipe, FILE *inputFile)
{
	bool success;
	DWORD bytesWritten;
	CHAR buffer[kBufferSize];
	DWORD totalBytesWritten = 0;

	for (;;)
	{
		DWORD bytesRead = fread_s(buffer, kBufferSize, 1, kBufferSize, inputFile);
		if (bytesRead == 0)
			FATAL_ERROR_BREAK(success, "fread_s Failed");
		if (!WriteFile(pipe, buffer, bytesRead, &bytesWritten, nullptr))
			FATAL_ERROR_BREAK(success, "WriteFile Failed");
		totalBytesWritten += bytesWritten;
		if (feof(inputFile))
			break;
	}
	CloseHandle(pipe);
	return totalBytesWritten;
}

bool ConvertToWideString(const std::string &raw, WCHAR *outputBuffer, size_t bufferSize)
{
	auto rawStr       = raw.c_str();
	auto rawLength    = (int) raw.size();
	int convertResult = MultiByteToWideChar(CP_UTF8, 0, rawStr, rawLength, outputBuffer, bufferSize);
	return (convertResult > 0);
}

}// namespace

namespace himu
{

std::optional<Process> Process::StartProcess(ProcessStartInfo &info)
{
	Process result;

#if _DEBUG
	LOGGER->debug("Starting process {0}", info.to_string());
#endif
	HANDLE chlidStdOutRead    = NULL;
	HANDLE chlidStdOutWrite   = NULL;
	HANDLE childStdErrorRead  = NULL;
	HANDLE childStdErrorWrite = NULL;
	HANDLE childStdInRead     = NULL;
	HANDLE childStdInWrite    = NULL;

	HANDLE hJob      = nullptr;
	bool success     = true;
	DWORD resultCode = 0;
	std::future<DWORD> stdinFuture, stdoutFuture, stderrFuture;
	std::chrono::high_resolution_clock::time_point start, end;

	// job object
	hJob = CreateJobObject(nullptr, nullptr);
	if (hJob == nullptr)
	{
		LOGGER->error("CreateJobObject Failed! ({0})", GetLastError());
		return std::nullopt;
	}
	// Because JOBOBJECT_EXTENDED_LIMIT_INFORMATION specifies the user's time, it's not accurate to limit the entire time directly.
	// For us, it's important to focus on the time from the time the entire process resumes running to the time it terminates.
	JOBOBJECT_EXTENDED_LIMIT_INFORMATION jobInfo = {0};
	jobInfo.JobMemoryLimit                       = info.MemoryLimit;
	jobInfo.BasicLimitInformation.LimitFlags     = JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE | JOB_OBJECT_LIMIT_JOB_MEMORY;
	if (!SetInformationJobObject(_Notnull_ hJob, JobObjectExtendedLimitInformation, &jobInfo, sizeof(jobInfo)))
		FATAL_ERROR_GOTO(success, "SetInformationJobObject Failed!", clean_job);

	SECURITY_ATTRIBUTES saAttr;
	saAttr.bInheritHandle       = true;
	saAttr.lpSecurityDescriptor = nullptr;
	saAttr.nLength              = sizeof(SECURITY_ATTRIBUTES);
	if (info.RedirectStandardOutput)
	{
		if (!CreatePipe(&chlidStdOutRead, &chlidStdOutWrite, &saAttr, 0))
			FATAL_ERROR_GOTO(success, "CreatePipe Failed", clean_pipes);
		if (!SetHandleInformation(chlidStdOutRead, HANDLE_FLAG_INHERIT, 0))
			FATAL_ERROR_GOTO(success, "SetHandleInformation Failed", clean_pipes);
	}
	if (info.RedirectStandardError)
	{
		if (!CreatePipe(&childStdErrorRead, &childStdErrorWrite, &saAttr, 0))
			FATAL_ERROR_GOTO(success, "CreatePipe Failed", clean_pipes);
		if (!SetHandleInformation(childStdErrorRead, HANDLE_FLAG_INHERIT, 0))
			FATAL_ERROR_GOTO(success, "SetHandleInformation Failed", clean_pipes);
	}
	if (info.InputRedirection != nullptr)
	{
		if (!CreatePipe(&childStdInRead, &childStdInWrite, &saAttr, 0))
			FATAL_ERROR_GOTO(success, "CreatePipe Failed", clean_pipes);
		if (!SetHandleInformation(childStdInWrite, HANDLE_FLAG_INHERIT, 0))
			FATAL_ERROR_GOTO(success, "SetHandleInformation Failed", clean_pipes);
	}

	PROCESS_INFORMATION procInfo;
	STARTUPINFO startupInfo;
	ZeroMemory(&procInfo, sizeof(PROCESS_INFORMATION));
	ZeroMemory(&startupInfo, sizeof(STARTUPINFO));
	startupInfo.cb = sizeof(STARTUPINFO);
	if (info.RedirectStandardOutput || info.RedirectStandardError || info.InputRedirection != nullptr)
	{
		startupInfo.dwFlags |= STARTF_USESTDHANDLES;
		if (info.RedirectStandardError)
			startupInfo.hStdError = childStdErrorWrite;
		if (info.RedirectStandardOutput)
			startupInfo.hStdOutput = chlidStdOutWrite;
		if (info.InputRedirection != nullptr)
			startupInfo.hStdInput = childStdInRead;
	}

	TCHAR commandLine[kMaxCommandLength];
	ZeroMemory(commandLine, sizeof(commandLine));
	if (!ConvertToWideString(info.Command, commandLine, kMaxCommandLength))
		FATAL_ERROR_GOTO(success, "ConvertToWideString Failed", clean_pipes);

	success = CreateProcess(nullptr, commandLine, nullptr, nullptr, true, CREATE_SUSPENDED, nullptr, nullptr,
							&startupInfo, &procInfo);
	if (!success)
	{
		FATAL_ERROR_GOTO(success, "CreateProcess Failed", clean_pipes);
	}

	if (!AssignProcessToJobObject(hJob, procInfo.hProcess))
	{
		TerminateProcess(procInfo.hProcess, 1);
		FATAL_ERROR_GOTO(success, "AssignProcessToJobObject Failed", clean_process);
	}

	// record real clock time, may more than actual time
	start = std::chrono::high_resolution_clock::now();
	ResumeThread(procInfo.hThread);
	if (chlidStdOutWrite != nullptr)
	{
		CloseHandle(chlidStdOutWrite);
		chlidStdOutWrite = nullptr;
	}
	if (childStdErrorWrite != nullptr)
	{
		CloseHandle(childStdErrorWrite);
		childStdErrorWrite = nullptr;
	}
	if (childStdInRead != nullptr)
	{
		CloseHandle(childStdInRead);
		childStdInRead = nullptr;
	}

	if (info.InputRedirection != nullptr)
	{
		// when process ready, write to pipe
		WaitForInputIdle(procInfo.hProcess, INFINITE);
		stdinFuture = std::async(std::launch::async, WriteToPipe, childStdInWrite, info.InputRedirection);
	}
	if (info.RedirectStandardOutput)
	{
		if (info.OutputRedirection)
		{
			stdoutFuture = std::async(std::launch::async, ReadFromPipeToFile, chlidStdOutRead, info.OutputRedirection,
									  info.OutputLimit);
		}
		else
		{
			stdoutFuture = std::async(std::launch::async, ReadFromPipeAsString, chlidStdOutRead,
									  std::ref(result.StdOutput), info.OutputLimit);
		}
	}
	if (info.RedirectStandardError)
	{
		stderrFuture = std::async(std::launch::async, ReadFromPipeAsString, childStdErrorRead,
								  std::ref(result.StdError), info.OutputLimit);
	}

	resultCode = WaitForSingleObject(procInfo.hProcess, info.TimeLimit);
	end        = std::chrono::high_resolution_clock::now();
	if (resultCode == WAIT_TIMEOUT)
	{
		LOGGER->info("process {0} (from {1}) was killed: WAIT_TIMEOUT", procInfo.dwProcessId, info.Command);
		result.Type = Process::ExecutionResultType::TimeLimitExceeded;
		TerminateJobObject(hJob, 3);
	}
	else
	{
		if (info.RedirectStandardOutput)
		{
			DWORD bytes = stdoutFuture.get();
			LOGGER_VERBOSE("read {0} bytes from process {1} stdout (from {2})", bytes, procInfo.dwProcessId,
						   info.Command);
		}
		if (info.RedirectStandardError)
		{
			DWORD bytes = stderrFuture.get();
			LOGGER_VERBOSE("read {0} bytes from process {1} stderr (from {2})", bytes, procInfo.dwProcessId,
						   info.Command);
		}
		if (info.InputRedirection != nullptr)
		{
			DWORD writtenBytes = stdinFuture.get();
			LOGGER_VERBOSE("wrote {0} bytes to process {1} (to {2})", writtenBytes, procInfo.dwProcessId, info.Command);
		}
	}
	GetExitCodeProcess(procInfo.hProcess, &resultCode);
clean_process:
	CloseHandle(procInfo.hProcess);
	CloseHandle(procInfo.hThread);
clean_pipes:
	if (chlidStdOutWrite != nullptr)
		CloseHandle(chlidStdOutWrite);
	if (chlidStdOutRead != nullptr)
		CloseHandle(chlidStdOutRead);
	if (childStdErrorRead != nullptr)
		CloseHandle(childStdErrorRead);
	if (childStdErrorWrite != nullptr)
		CloseHandle(childStdErrorWrite);
clean_job:
	DWORD querySize;
	if (!QueryInformationJobObject(hJob, JobObjectExtendedLimitInformation, &jobInfo, sizeof(jobInfo), &querySize))
	{
		FATAL_ERROR_DEBUG("QueryInformationJobObject Failed!");
	}
	else
	{
		result.ExitCode   = resultCode;
		result.MemoryUsed = jobInfo.PeakJobMemoryUsed;
		result.TimeUsed   = std::chrono::duration_cast<std::chrono::milliseconds>(end - start).count();
		if (jobInfo.PeakJobMemoryUsed > info.MemoryLimit)
		{
			LOGGER->info("process {0} (from {1}) was killed: MemoryLimit exceeded", procInfo.dwProcessId, info.Command);
			result.Type = Process::ExecutionResultType::MemoryLimitExceeded;
		}
		else if (result.Type != Process::ExecutionResultType::TimeLimitExceeded)
		{
			LOGGER->debug("process {0} (from {1}) was normally exited: ExitCode = {2}", procInfo.dwProcessId,
						  info.Command, resultCode);
			result.Type =
				(resultCode != 0) ? Process::ExecutionResultType::UnknownError : Process::ExecutionResultType::Success;
		}
	}
	CloseHandle(hJob);
	return result;
}

std::optional<Process> Process::StartProcess(std::function<void(ProcessStartInfoBuilder &)> builder)
{
	ProcessStartInfo info;
	ProcessStartInfoBuilder b {info};
	builder(b);
	return StartProcess(info);
}

}// namespace himu::stage
#endif