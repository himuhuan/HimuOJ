#include "mysql_dbcontext.h"
#include "shared/error_code.h"
#include "shared/logger.h"
#include "shared/models/mysql/point_result.h"
#include "shared/models/mysql/problem.h"
#include "shared/models/mysql/test_point.h"
#include "shared/models/mysql/user_commit.h"
#include <sqlpp11/sqlpp11.h>

SQLPP_ALIAS_PROVIDER(testPointId)
SQLPP_ALIAS_PROVIDER(commitId)

namespace himu::dbcontext
{

	std::optional<std::vector<himu::dto::TestPointTask>> MySqlDbContext::getCommitTasks(long commitId)
	{
		std::vector<himu::dto::TestPointTask> tasks;
		sql_models::Pointresults pointresults {};
		sql_models::Testpoints testpoints {};
		sql_models::Usercommits usercommits {};
		// clang-format off
	auto testpointTab = pointresults
		.left_outer_join(testpoints).on(pointresults.TestPointId == testpoints.Id);
		// clang-format on
		auto taskQuery = sql::select(
							 pointresults.Id, pointresults.CommitId, pointresults.TestPointId, pointresults.TestStatus,
							 testpoints.Input, testpoints.Expected, testpoints.ProblemId, testpoints.CaseName)
							 .from(testpointTab)
							 .where(pointresults.CommitId == commitId);
		auto res = _database->run(taskQuery);
		for (const auto &re : res)
		{
			himu::dto::TestPointTask result;
			result.id          = re.Id.value();
			result.commitId    = commitId;
			result.testpointId = re.TestPointId.value();
			result.status      = himu::dto::toProgramExecutionStatus(re.TestStatus.value());

			// clang-format off
		result.testPoint   = std::make_unique<dto::TestPoint>(
            re.TestPointId.value(),
			re.Input.value(),
			re.Expected.value(),
			re.ProblemId.value(),
            re.CaseName.value()
		);
			// clang-format on
			tasks.push_back(std::move(result));
		}

		if (tasks.empty())
		{
			return std::nullopt;
		}
		return tasks;
	}

	MySqlDbContext::MySqlDbContext(const config::DbContextConfig &config) : _isConnected(false)
	{
		_connectionConfig                 = std::make_unique<mysql::connection_config>();
		_connectionConfig->host           = config.host;
		_connectionConfig->user           = config.user;
		_connectionConfig->password       = config.password;
		_connectionConfig->database       = config.database;
		_connectionConfig->port           = config.port;
		_connectionConfig->auto_reconnect = false;
		try
		{
			_database    = std::make_unique<mysql::connection>(_connectionConfig);
			_isConnected = true;
		}
		catch (const sql::exception &e)
		{
			_isConnected = false;
			MyLogger.error("failed to connect to database: {}", e.what());
		}
	}

	bool MySqlDbContext::connected() const noexcept
	{
		return _isConnected;
	}

	std::unique_ptr<himu::dto::UserCommit> MySqlDbContext::getCommit(long commitId)
	{
		sql_models::Usercommits usercommits {};
		auto query = sql::select(
						 usercommits.Id, usercommits.SourceUri, usercommits.Status, usercommits.ProblemId,
						 usercommits.CompilerInformationCompilerName)
						 .from(usercommits)
						 .where(usercommits.Id == commitId);
		for (const auto &res : _database->run(query))
		{
			auto commit       = std::make_unique<himu::dto::UserCommit>();
			commit->id        = commitId;
			commit->status    = himu::dto::toProgramExecutionStatus(res.Status.value());
			commit->sourceUri = res.SourceUri.value();
			commit->problemId = res.ProblemId.value();
			commit->compiler  = res.CompilerInformationCompilerName.value();
			return commit;
		}
		return nullptr;
	}

	std::unique_ptr<himu::dto::ProblemLimit> MySqlDbContext::getProblemLimit(long problemId)
	{
		sql_models::Problemset problemset {};
		auto query = sql::select(problemset.DetailMaxMemoryLimitByte, problemset.DetailMaxExecuteTimeLimit)
						 .from(problemset)
						 .where(problemset.Id == problemId);
		for (const auto &res : _database->run(query))
		{
			return std::make_unique<himu::dto::ProblemLimit>(
				res.DetailMaxMemoryLimitByte, res.DetailMaxExecuteTimeLimit);
		}
		return nullptr;
	}

	void MySqlDbContext::updateContext(const dto::UserCommit &commit, const std::vector<dto::TestPointTask> &tasks)
	{
		auto &database = *_database;
		MyLogger.trace("starting update database for commit@{0}", commit.id);
		auto updateTransaction = sql::start_transaction(database);
		try
		{
			sql_models::Usercommits usercommits {};
			sql_models::Pointresults pointresults {};
			if (commit.status == dto::ProgramExecutionStatus::RUNNING)
			{
				database(update(usercommits).set(usercommits.Status = "RUNNING").where(usercommits.Id == commit.id));
				database(update(pointresults)
							 .set(pointresults.TestStatus = "RUNNING")
							 .where(pointresults.CommitId == commit.id));
			}
			else if (commit.status == dto::ProgramExecutionStatus::INTERNAL_ERROR)
			{
				database(
					update(usercommits).set(usercommits.Status = "INTERNAL_ERROR").where(usercommits.Id == commit.id));
				database(update(pointresults)
							 .set(pointresults.TestStatus = "INTERNAL_ERROR")
							 .where(pointresults.CommitId == commit.id));
			}
			else if (commit.status == dto::ProgramExecutionStatus::COMPILE_ERROR)
			{
				database(
					update(usercommits).set(usercommits.Status = "COMPILE_ERROR").where(usercommits.Id == commit.id));
				database(update(usercommits)
							 .set(usercommits.CompilerInformationMessageFromCompiler = *commit.compileMessage)
							 .where(usercommits.Id == commit.id));
				database(update(pointresults)
							 .set(pointresults.TestStatus = "COMPILE_ERROR")
							 .where(pointresults.CommitId == commit.id));
			}
			else
			{
				std::string status = fmt::format("{}", commit.status);
				database(
					update(usercommits).set(usercommits.Status = status.c_str()).where(usercommits.Id == commit.id));
				for (auto &task : tasks)
				{
					status = fmt::format("{}", task.status);
					database(update(pointresults)
								 .set(
									 pointresults.TestStatus        = status.c_str(),
									 pointresults.RunResultExitCode = task.runResult->exitCode,
									 pointresults.RunResultMessage  = task.runResult->message,
									 pointresults.UsageMemoryByteUsed = task.resourceUsage->memory,
									 pointresults.UsageTimeUsed = task.resourceUsage->time)
								 .where(pointresults.Id == task.id));
					// Only WRONG_ANSWER status has difference information
					if (task.status == dto::ProgramExecutionStatus::WRONG_ANSWER)
					{
						database(update(pointresults)
									 .set(
										 pointresults.DifferenceActual   = task.diffResult->actual,
										 pointresults.DifferenceExpected = task.diffResult->expected,
										 pointresults.DifferencePosition = task.diffResult->position)
									 .where(pointresults.Id == task.id));
					}
				}
			}
			MyLogger.trace("commit@{0} has updated to database", commit.id);
			updateTransaction.commit();
		}
		catch (const sql::exception &e)
		{
			MyLogger.error(
				"failed to update database for commit@{0}: {1}, transaction has rolled "
				"back",
				commit.id, e.what());
			updateTransaction.rollback();
		}
	}

}// namespace himu::dbcontext
