//
// File: Shared.h
// Configuration, Global Variables, typedefs
//

#pragma once

#ifndef HIMU_JUDGE_CORE_SHARED_H
#define HIMU_JUDGE_CORE_SHARED_H

namespace himu
{

enum LoggerLevel
{
	Verbose   = 0,
	Normal    = 1,
	OnlyError = 2
};

struct ServerConfiguration
{
	std::string Host;
	std::string Port;
	LoggerLevel LogLevel;
	bool KeepExecutableAfterJudge;
	int64_t MaxCompilerProcessMemoryLimit;
	int64_t MaxRedirectionOutputBufferLength;
	int64_t MaxRedirectionOutputFileLength;

	/**
	 * @brief Use default values for the configuration here.
	 */

	ServerConfiguration()
		: Host("localhost"), Port("7721"), KeepExecutableAfterJudge(false),
		  MaxCompilerProcessMemoryLimit(1024 * 1024 * 1024),// 1GB
		  MaxRedirectionOutputBufferLength(1024 * 10),      // 10KB
		  MaxRedirectionOutputFileLength(1024 * 1024)       // 1MB
	{
#if _DEBUG
		LogLevel = LoggerLevel::Verbose;
#else
		LogLevel = LoggerLevel::Normal;
#endif// ^^ _DEBUG ^^
	}
};

struct Globals
{
	ServerConfiguration Configuration;
	std::shared_ptr<spdlog::async_logger> Log;

	bool Initialize();
	void Cleanup();
};

}// namespace himu

extern himu::Globals gGlobals;

#define LOGGER gGlobals.Log

#if _DEBUG
#define LOGGER_VERBOSE(message, ...) LOGGER->debug(message, __VA_ARGS__)
#else
#define LOGGER_VERBOSE(message, ...)
#endif

#if defined(_WIN32) && defined(_DEBUG)

#define FATAL_ERROR_DEBUG(message, ...)                                                                                \
	{                                                                                                                  \
		LOGGER->error("{0}:{1}: ({2}) {3}", __FILE__, __LINE__, GetLastError(), message, __VA_ARGS__);                 \
	}

#define FATAL_ERROR_RETURN(message, exitCode)                                                                          \
	{                                                                                                                  \
		LOGGER->error("{0}:{1}: {2} ({3})", __FILE__, __LINE__, message, GetLastError());                              \
		return (exitCode);                                                                                             \
	}

#define FATAL_ERROR_GOTO(status, message, label)                                                                       \
	{                                                                                                                  \
		status = false;                                                                                                \
		LOGGER->error("{0}:{1}: {2} ({3})", __FILE__, __LINE__, message, GetLastError());                              \
		goto label;                                                                                                    \
	}

#define FATAL_ERROR_BREAK(status, message)                                                                             \
	{                                                                                                                  \
		status = false;                                                                                                \
		LOGGER->error("{0}:{1}: {2} ({3})", __FILE__, __LINE__, message, GetLastError());                              \
		break;                                                                                                         \
	}

#endif

#endif// !HIMU_JUDGE_CORE_SHARED_H
