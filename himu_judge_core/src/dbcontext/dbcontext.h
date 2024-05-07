#ifndef JUDGECORESERVER_DBCONTEXT_H
#define JUDGECORESERVER_DBCONTEXT_H

#include "shared/config/dbcontext_config.h"
#include "shared/dto/testpoint.h"
#include "shared/dto/testpoint_result.h"
#include "shared/dto/user_commit.h"
#include "shared/dto/problem_limilt.h"

#include <optional>
#include <vector>

namespace himu::dbcontext
{
class BaseDbContext
{
public:
	/**
	 * Get test points from database.
	 * @param commitId The id of the commit.
	 */
	[[nodiscard]]
	virtual std::optional<std::vector<himu::dto::TestPointTask>> getCommitTasks(long commitId) = 0;

	/**
	 * Get commit from database.
	 * @param commitId The id of the commit.
	*/
	[[nodiscard]]
	virtual std::unique_ptr<himu::dto::UserCommit> getCommit(long commitId) = 0;

	/**
	 * @brief Get the problem limit from the database.
	 * @param problemId problem id
	 */
	[[nodiscard]]
	virtual std::unique_ptr<himu::dto::ProblemLimit> getProblemLimit(long problemId) = 0;

	/**
	 * @brief update the context of the database.
	*/
	virtual void updateContext(const dto::UserCommit &commit,
							   const std::vector<dto::TestPointTask> &tasks) = 0;

	[[nodiscard]]
	virtual bool connected() const noexcept = 0;

	virtual ~BaseDbContext() = default;
};

/**
 * @brief DbContextFactory creates a new instance of the DbContext according to config.
 */
class DbContextFactory
{
public:
	explicit DbContextFactory(const char *provider, config::DbContextConfig &dbConfig);

	/**
	 * @brief create a new context with a new connection to database.
	 * @return a new instance of the DbContext. If the connection fails, it will return nullptr.
	 */
	[[nodiscard]]
	std::unique_ptr<BaseDbContext> getContext();

private:
	config::DbContextConfig & _dbConfig;
	std::string _provider;
	std::mutex _mutex;
};

}// namespace himu::dbcontext

#endif//! JUDGECORESERVER_DBCONTEXT_H