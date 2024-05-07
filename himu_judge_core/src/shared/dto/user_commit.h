#ifndef JUDGECORESERVER_DTO_USER_COMMIT_H_
#define JUDGECORESERVER_DTO_USER_COMMIT_H_

#include <memory>
#include <string>

#include "shared/dto/program_execution_status.h"

namespace himu::dto
{
	struct UserCommit
	{
		long id {};
		long problemId {};
		std::string sourceUri;
		ProgramExecutionStatus status = ProgramExecutionStatus::PENDING;
		std::string compiler;

		std::unique_ptr<std::string> compileMessage = nullptr;

		UserCommit()                        = default;
		UserCommit(const UserCommit &other) = delete;
		UserCommit(UserCommit &&other) noexcept;
		UserCommit &operator=(UserCommit &&other) noexcept;
	};

}// namespace himu::dto

#endif// JUDGECORESERVER_DTO_USER_COMMIT_H_