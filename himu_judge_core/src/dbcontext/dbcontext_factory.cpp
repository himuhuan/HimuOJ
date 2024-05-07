#include "dbcontext.h"
#include "mysql_dbcontext.h"

namespace himu::dbcontext
{

DbContextFactory::DbContextFactory(const char *provider, config::DbContextConfig &dbConfig)
	: _provider(provider), _dbConfig(dbConfig)
{
}

std::unique_ptr<BaseDbContext> DbContextFactory::getContext()
{
	std::unique_lock<std::mutex> lock(_mutex);
	if (_provider == "mysql")
	{
		auto context = std::make_unique<MySqlDbContext>(_dbConfig);
		if (!context->connected())
		{
			return nullptr;
		}
		return context;
	}
	return nullptr;
}

}// namespace himu::dbcontext