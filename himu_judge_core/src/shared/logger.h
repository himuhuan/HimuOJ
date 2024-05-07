#ifndef JUDGECORESERVER_SHARED_LOGGER_H_
#define JUDGECORESERVER_SHARED_LOGGER_H_

#include <spdlog/async.h>
#include <spdlog/sinks/daily_file_sink.h>
#include <spdlog/sinks/stdout_color_sinks.h>
#include <spdlog/spdlog.h>

#include "commandline/command_line_options.h"
#include "shared/bits.h"
#include "shared/singleton.h"

class ServerLogger : public ISingleton
{
private:
	std::shared_ptr<spdlog::async_logger> m_logger;
	static std::string m_taskName;
	static bool m_initialized;
	static bool m_isDebug;

	explicit ServerLogger()
	{
		spdlog::init_thread_pool(8192, 1);

		auto consoleSink = std::make_shared<spdlog::sinks::stdout_color_sink_st>();
#ifdef _DEBUG
		consoleSink->set_level(spdlog::level::debug);
#else
		consoleSink->set_level(spdlog::level::info);
#endif

		fs::path logPath = fs::path("logs") / fmt::format("{}.log", m_taskName);
		auto fileSink    = std::make_shared<spdlog::sinks::daily_file_sink_st>(logPath, 0, 0);
#if _DEBUG
		fileSink->set_level(spdlog::level::trace);
#else
		fileSink->set_level(spdlog::level::debug);
#endif // ^^^ _DEBUG ^^^
		std::vector<spdlog::sink_ptr> sinks{consoleSink, fileSink};
		m_logger = std::make_shared<spdlog::async_logger>("himu-judge-core", sinks.begin(),
														  sinks.end(), spdlog::thread_pool(),
														  spdlog::async_overflow_policy::block);
#if _DEBUG
		m_logger->set_level(spdlog::level::trace);
#else
		m_logger->set_level(spdlog::level::debug);
#endif
		spdlog::register_logger(m_logger);
	}

	~ServerLogger() override = default;

public:
	static void initialize(std::string_view taskName, const CommandLineOptions *options)
	{
		static std::once_flag flag;
		std::call_once(flag, [&taskName, options]() {
			m_taskName    = taskName;
			m_initialized = true;
			if (options != nullptr)
				m_isDebug = options->isDebugMode();
		});
	}

	static ServerLogger &getInstance()
	{
		if (!m_initialized)
		{
			throw std::runtime_error("ServerLogger is not initialized");
		}
		static ServerLogger instance;
		return instance;
	}

	template<typename... Args>
	[[maybe_unused]]
	void debug(fmt::format_string<Args...> fmt, const Args &...args)
	{
		m_logger->debug(fmt, args...);
		m_logger->flush();
	}

	template<typename... Args>
	[[maybe_unused]] void trace(fmt::format_string<Args...> fmt, const Args &... args)
	{
#if _DEBUG
		m_logger->trace(fmt, args...);
		m_logger->flush();
#endif // ^^^ _DEBUG ^^^
	}

	template<typename... Args>
	void info(fmt::format_string<Args...> fmt, const Args &...args)
	{
		m_logger->info(fmt, args...);
		m_logger->flush();
	}

	template<typename... Args>
	[[maybe_unused]]
	void warn(fmt::format_string<Args...> fmt, const Args &...args)
	{
		m_logger->warn(fmt, args...);
		m_logger->flush();
	}

	template<typename... Args>
	[[maybe_unused]]
	void error(fmt::format_string<Args...> fmt, const Args &...args)
	{
		m_logger->error(fmt, args...);
		m_logger->flush();
	}
};

#define MyLogger ServerLogger::getInstance()

#endif// JUDGECORESERVER_SHARED_LOGGER_H_
