#ifndef JUDGECORESERVER_SHARED_DBCONTEXT_CONFIG_H
#define JUDGECORESERVER_SHARED_DBCONTEXT_CONFIG_H

#include <string>

namespace himu::config
{

struct DbContextConfig
{
	int port;
	std::string host;
	std::string user;
	std::string password;
	std::string database;
	/// The maximum number of connections that can be created
	size_t maxConnetions;

	DbContextConfig()                        = default;
	DbContextConfig(const DbContextConfig &) = default;
	DbContextConfig(int port, const std::string &host, const std::string &user,
					const std::string &password, const std::string &database,
					size_t maxConnection = 20)
		: port(port), host(host), user(user), password(password), database(database),
		  maxConnetions(maxConnection)
	{
	}
};

}// namespace himu::config

#endif//JUDGECORESERVER_SHARED_DBCONTEXT_CONFIG_H