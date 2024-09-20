#include "pch.h"
#include <vector>
#include <rapidjson/document.h>
#include <rapidjson/filereadstream.h>

constexpr auto DEFAULT_CONFIG_FILE   = "appsettings.json";
constexpr auto DEFAULT_LOG_FILE_PATH = "logs/himuoj.log";

namespace himu
{

bool Globals::Initialize()
{
	bool useDefaultConfig = true;

#if defined(_WIN32) || defined(_WIN64)
	FILE *configFile = ::fopen(DEFAULT_CONFIG_FILE, "rb");
#else
	FILE *configFile = ::fopen(DEFAULT_CONFIG_FILE, "r");
#endif// ^^ _WIN32 ^^

	if (configFile != nullptr)
	{
		char buffer[BUFSIZ << 3] {};
		rapidjson::FileReadStream fileStream {configFile, buffer, sizeof(buffer)};
		rapidjson::Document document;
		document.ParseStream(fileStream);
		if (document.HasParseError())
		{
			fmt::println(stderr, "Failed to parse configuration file: {}", DEFAULT_CONFIG_FILE);
			::fclose(configFile);
			return false;
		}
		else
		{
			useDefaultConfig = false;
			
			if (document.HasMember("Host"))
				Configuration.Host = document["Host"].GetString();
			if (document.HasMember("Port"))
				Configuration.Port = document["Port"].GetString();
			if (document.HasMember("LogLevel"))
				Configuration.LogLevel = (LoggerLevel) document["LogLevel"].GetInt();

			if (document.HasMember("KeepExecutableAfterJudge"))
				Configuration.KeepExecutableAfterJudge = document["KeepExecutableAfterJudge"].GetBool();
			
			// MaxCompilerProcessMemoryLimit
			if (document.HasMember("MaxCompilerProcessMemoryLimit"))
				Configuration.MaxCompilerProcessMemoryLimit = document["MaxCompilerProcessMemoryLimit"].GetInt64();
			
			// MaxRedirectionOutputBufferLength
			if (document.HasMember("MaxRedirectionOutputBufferLength"))
			{
				Configuration.MaxRedirectionOutputBufferLength =
					document["MaxRedirectionOutputBufferLength"].GetInt64();
			}
			
			// MaxRedirectionOutputFileLength
			if (document.HasMember("MaxRedirectionOutputFileLength"))
			{
				Configuration.MaxRedirectionOutputFileLength = document["MaxRedirectionOutputFileLength"].GetInt64();
			}
		}
		::fclose(configFile);
	}

	spdlog::init_thread_pool(8192, 1);
	auto consoleSink = std::make_shared<spdlog::sinks::stdout_color_sink_st>();
	auto fileSink    = std::make_shared<spdlog::sinks::daily_file_format_sink_st>(DEFAULT_LOG_FILE_PATH, 0, 0);
	if (consoleSink == nullptr || fileSink == nullptr)
	{
		fmt::println(stderr, "fatal: Failed to create sinks for logger");
		return false;
	}
	switch (Configuration.LogLevel)
	{
		case LoggerLevel::Verbose:
			consoleSink->set_level(spdlog::level::trace);
			fileSink->set_level(spdlog::level::trace);
			break;
		case LoggerLevel::OnlyError:
			consoleSink->set_level(spdlog::level::err);
			fileSink->set_level(spdlog::level::warn);
			break;
		default:
		case LoggerLevel::Normal:
			consoleSink->set_level(spdlog::level::warn);
			fileSink->set_level(spdlog::level::info);
			break;
	}

	std::vector<spdlog::sink_ptr> sinks {consoleSink, fileSink};
	Log = std::make_shared<spdlog::async_logger>("HimuOJ", sinks.begin(), sinks.end(), spdlog::thread_pool(),
												 spdlog::async_overflow_policy::block);
	if (Log == nullptr)
	{
		fmt::println(stderr, "fatal: Failed to create logger");
		return false;
	}
#if _DEBUG
	Log->set_level(spdlog::level::trace);
#else
	Log->set_level(spdlog::level::info);
#endif
	spdlog::register_logger(Log);

	Log->info("============= HimuOJ Server initialized. Now Starting =============");
	if (useDefaultConfig)
	{
		Log->warn("Using default configuration");
	}
	else
	{
		Log->info("Configuration loaded from file: {}", DEFAULT_CONFIG_FILE);
	}

	LOGGER->info("Current Working Directory: {0}", std::filesystem::current_path().string());

	return true;
}

void Globals::Cleanup()
{
	Log->info("============= HimuOJ Server shutting down =============");
	Log->flush();
	spdlog::shutdown();
}

}// namespace himu