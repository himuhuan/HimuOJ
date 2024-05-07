#ifndef JUDGECORESERVER_DTO_TESTPOINT_RESULT_H_
#define JUDGECORESERVER_DTO_TESTPOINT_RESULT_H_

#include <fmt/format.h>
#include <memory>
#include <mutex>
#include <string>

#include "shared/dto/diff_result.h"
#include "shared/dto/exit_code_with_message.h"
#include "shared/dto/program_execution_status.h"
#include "shared/dto/resource_usage.h"
#include "shared/dto/testpoint.h"

namespace himu::dto
{

struct TestPointTask
{
	long id{};
	long commitId{};
	long testpointId{};
	std::mutex testpointMutex;

	ProgramExecutionStatus status                  = ProgramExecutionStatus::PENDING;
	std::unique_ptr<ResourceUsage> resourceUsage   = nullptr;
	std::unique_ptr<ExitCodeWithMessage> runResult = nullptr;
	std::unique_ptr<DiffResult> diffResult         = nullptr;
	std::unique_ptr<TestPoint> testPoint           = nullptr;

	TestPointTask() = default;

	TestPointTask(const TestPointTask &other) = delete;

	TestPointTask(TestPointTask &&other) noexcept;

	TestPointTask &operator=(TestPointTask &&other) noexcept;
};

}// namespace himu::dto

/**
 * @brief Format a TestPointTask enum value for debugging.
 */
template<> struct fmt::formatter<himu::dto::TestPointTask> : formatter<std::string_view>
{
	// parse is inherited from formatter<string_view>.
	auto format(const himu::dto::TestPointTask &testpoint_result, format_context &ctx) const
	{
		// clang-format off
		return formatter<std::string_view>::format(
			fmt::format("TestPointTask@{0}{{commitId: {1}, testpoint: {2}, status: {3} }}",
						testpoint_result.id,
						testpoint_result.commitId,
			            *(testpoint_result.testPoint),
						testpoint_result.status),
			ctx
		);
		// clang-format on
	}
};

#endif