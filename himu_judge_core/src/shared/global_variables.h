#ifndef JUDGECORESERVER_GLOBAL_VARIABLES_H
#define JUDGECORESERVER_GLOBAL_VARIABLES_H

#include "dbcontext/dbcontext.h"
#include "launcher/launcher.h"
#include "shared/config/app_config.h"
#include "shared/logger.h"

///////////////////////////////////
// Global variables
///////////////////////////////////

/// version of the judge core server
constexpr const char *JUDGE_CORE_SERVER_VERSION = "1.2.0";

/// Tick convert to Millisecond. In judge core server, 1 ms = 10000 ticks
constexpr int TICKS_PER_MS = 10000;

/// the config of the judge core server
extern std::unique_ptr<himu::config::AppConfig> gAppConfig;

/// the database context of the judge core server
extern std::unique_ptr<himu::dbcontext::DbContextFactory> gDbContextFactory;

/// launcher of the judge core server
extern std::unique_ptr<himu::launcher::Launcher> gLauncher;

/// Limits used in himu_judge_core
namespace himu::system_limit
{
	
	namespace runner
	{
		// 10s
		constexpr int MaxExecTimeMs = 10'000;
		// 1GiB
		constexpr int MaxExecMemoryByte = 1024 * 1024 * 1024;
	}// namespace runner

}// namespace himu::system_limit

#endif