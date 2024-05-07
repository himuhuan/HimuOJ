#ifndef JUDGECORESERVER_APP_CONFIG_H_
#define JUDGECORESERVER_APP_CONFIG_H_

#include <memory>
#include <string>

#include "dbcontext_config.h"

namespace himu::config
{

struct AppConfig
{
	std::unique_ptr<DbContextConfig> database;

	explicit AppConfig(const char *configPath);
};

}// namespace himu::config

#endif// JUDGECORESERVER_APP_CONFIG_H_