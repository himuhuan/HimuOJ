#include "user_commit.h"

namespace himu::dto
{

UserCommit::UserCommit(UserCommit &&other) noexcept
	: id(other.id), sourceUri(std::move(other.sourceUri)), status(other.status),
	  compiler(std::move(other.compiler)), compileMessage(std::move(other.compileMessage))
{
}

UserCommit &UserCommit::operator=(UserCommit &&other) noexcept
{
	id             = other.id;
	sourceUri      = std::move(other.sourceUri);
	status         = other.status;
	compiler       = std::move(other.compiler);
	compileMessage = std::move(other.compileMessage);
	return *this;
}

}// namespace himu::dto