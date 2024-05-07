#include "app_config.h"
#include "shared/error_code.h"
#include "shared/logger.h"

#include "rapidjson/document.h"
#include "rapidjson/filereadstream.h"

namespace
{

bool _initDatabaseConfig(himu::config::AppConfig &config, const rapidjson::Document &document)
{
	// check if the config file is valid
	if (!document.HasMember("database"))
	{
		return false;
	}

	const auto &database = document["database"];
	if (!database.IsObject())
	{
		return false;
	}

	auto databaseHost     = database.FindMember("host");
	auto databasePort     = database.FindMember("port");
	auto databaseUser     = database.FindMember("user");
	auto databasePassword = database.FindMember("password");
	auto databaseName     = database.FindMember("name");
	// clang-format off
	if (databaseHost == database.MemberEnd()        || !databaseHost->value.IsString() 
		|| databasePort == database.MemberEnd()     || !databasePort->value.IsUint() 
		|| databaseUser == database.MemberEnd()     || !databaseUser->value.IsString() 
		|| databasePassword == database.MemberEnd() || !databasePassword->value.IsString() 
		|| databaseName == database.MemberEnd()     || !databaseName->value.IsString())
	{
		return false;
	}

	config.database = std::make_unique<himu::config::DbContextConfig>(
		databasePort->value.GetInt(),
		databaseHost->value.GetString(), 
		databaseUser->value.GetString(), 
		databasePassword->value.GetString(),
		databaseName->value.GetString()
	);
	// clang-format on
	return true;
}

}// namespace

namespace himu::config
{

AppConfig::AppConfig(const char *configPath)
{
#if _WIN32
	FILE *fp = fopen(configPath, "rb");
#else
	FILE *fp = fopen(configPath, "r");
#endif

	if (fp == nullptr)
	{
		MyLogger.error("Failed to open config file: {}", configPath);
		std::exit(error_code::RESOURCE_NOT_FOUND);
	}

	char readBuffer[BUFSIZ * 8]{};
	rapidjson::FileReadStream stream(fp, readBuffer, sizeof(readBuffer));
	rapidjson::Document document;
	document.ParseStream(stream);

	if (document.HasParseError() || !_initDatabaseConfig(*this, document))
	{
		MyLogger.error("Failed to parse config file: {}", configPath);
		std::exit(error_code::INTERNAL_ERROR);
	}

	fclose(fp);

	MyLogger.info("Config file loaded: {}", configPath);
}

}// namespace himu::config