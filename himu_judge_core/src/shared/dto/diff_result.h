#ifndef JUDGECORESERVER_DTO_DIFF_RESULT_H_
#define JUDGECORESERVER_DTO_DIFF_RESULT_H_

#include <fmt/format.h>
#include <string>

namespace himu::dto
{

struct DiffResult
{
	std::string expected;
	std::string actual;
	long position;
};

}// namespace himu::dto
/**
 * @brief Format a DiffResult enum value for debugging.
 */
template<> struct fmt::formatter<himu::dto::DiffResult> : formatter<string_view>
{
	// parse is inherited from formatter<string_view>.
	auto format(const himu::dto::DiffResult & diff_result, format_context &ctx) const
	{
		using namespace himu::dto;
		// clang-format off
		return formatter<string_view>::format(
			fmt::format(R"(DiffResult{{"{0}" (expected) != "{1}" (actual), position: {2}}})",
						diff_result.expected, diff_result.actual,
						diff_result.position),
			ctx
		);
		// clang-format on
	}
};
#endif