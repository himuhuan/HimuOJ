#ifndef JUDGECORESERVER_DTO_TESTPOINT_H_
#define JUDGECORESERVER_DTO_TESTPOINT_H_

#include <fmt/format.h>
#include <string>

namespace himu::dto
{
struct TestPoint
{
	long id;
	std::string input;
	std::string expected;
	long problemId;
	std::string caseName;

	TestPoint() = default;

	TestPoint(long id, std::string input, std::string expected, long problemId,
			  std::string caseName)
		: id(id), input(std::move(input)), expected(std::move(expected)), problemId(problemId),
		  caseName(std::move(caseName))
	{

	}
};

}// namespace himu::dto

// fmt::formatter specialization for TestPoint
template<> struct fmt::formatter<himu::dto::TestPoint> : formatter<string_view>
{
	// parse is inherited from formatter<string_view>.
	auto format(const himu::dto::TestPoint & test_point, format_context &ctx) const
	{
		// clang-format off
		return formatter<string_view>::format(
			fmt::format(R"(TestPoint@{0}("{4}"){{input: "{1}", expected: "{2}", problemId: {3}}})",
						test_point.id,
						test_point.input,
						test_point.expected,
						test_point.problemId,
						test_point.caseName),
			ctx
		);
		// clang-format on
	}
};
#endif
