#ifndef HIMU_JUDGE_CORE_SERVER_SRC_DBCONTEXT_MYSQL_DBCONTEXT_H
#define HIMU_JUDGE_CORE_SERVER_SRC_DBCONTEXT_MYSQL_DBCONTEXT_H

#include "dbcontext.h"
#include "shared/config/dbcontext_config.h"
#include <sqlpp11/mysql/mysql.h>
#include <sqlpp11/sqlpp11.h>

namespace sql   = sqlpp;
namespace mysql = sql::mysql;

namespace himu::dbcontext
{

class MySqlDbContext : public BaseDbContext
{
private:
	std::shared_ptr<mysql::connection_config> _connectionConfig;
	std::unique_ptr<sql::mysql::connection> _database;
	bool _isConnected;
public:
	[[nodiscard]]
	std::optional<std::vector<himu::dto::TestPointTask>> getCommitTasks(long commitId) override;

	[[nodiscard]]
	std::unique_ptr<himu::dto::UserCommit> getCommit(long commitId) override;

	[[nodiscard]]
	std::unique_ptr<himu::dto::ProblemLimit> getProblemLimit(long problemId) override;

	void updateContext(const dto::UserCommit &commit,
					   const std::vector<dto::TestPointTask> &tasks) override;

	explicit MySqlDbContext(const config::DbContextConfig &config);

	[[nodiscard]]
	bool connected() const noexcept override;
};

}// namespace himu::dbcontext

#endif//HIMU_JUDGE_CORE_SERVER_SRC_DBCONTEXT_MYSQL_DBCONTEXT_H
