#pragma once

namespace himu
{

struct ProcessStartInfo
{
	std::string Command;
	int64_t TimeLimit       = 0;       /* in milliseconds */
	int64_t MemoryLimit     = 0;       /* in bytes */
	FILE *InputRedirection  = nullptr; /* Input only can be redirected from a file */
	FILE *OutputRedirection = nullptr; /* if not null, the output will be written to this file */
	// FILE *ErrorRedirection   = nullptr; 	/* Not Implemented */
	bool RedirectStandardOutput = false;
	bool RedirectStandardError  = false;
	/* Input only can be redirected from a file, so this is not needed */
	// bool RedirectStandardInput  = false;

	/* The limit of the output of the process, 0 means no limit */
	int64_t OutputLimit = 0; /* in bytes */

#if _DEBUG
	std::string to_string() const
	{
		std::string result = "ProcessStartInfo {";
		result += "Command: " + Command;
		result += ", TimeLimit: " + std::to_string(TimeLimit) + " ms";
		result += ", MemoryLimit: " + std::to_string(MemoryLimit) + " bytes";
		result += std::string(", RedirectStandardOutput: ") + (RedirectStandardOutput ? "true" : "false");
		result += std::string(", RedirectStandardError: ") + (RedirectStandardError ? "true" : "false");
		result += "}";
		return result;
	}
#endif
};

}// namespace himu::stage