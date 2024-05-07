#include "testpoint_result.h"
#include "shared/logger.h"

namespace himu::dto
{

TestPointTask::TestPointTask(TestPointTask &&other) noexcept
	: id(other.id), commitId(other.commitId), testpointId(other.testpointId), status(other.status),
	  resourceUsage(std::move(other.resourceUsage)), runResult(std::move(other.runResult)),
	  diffResult(std::move(other.diffResult)), testPoint(std::move(other.testPoint))
{
}

TestPointTask &TestPointTask::operator=(TestPointTask &&other) noexcept
{
	id            = other.id;
	commitId      = other.commitId;
	testpointId   = other.testpointId;
	status        = other.status;
	resourceUsage = std::move(other.resourceUsage);
	runResult     = std::move(other.runResult);
	diffResult    = std::move(other.diffResult);
	testPoint     = std::move(other.testPoint);
	return *this;
}

}// namespace himu::dto