#ifndef JUDGECORESERVER_DTO_RESOURCE_USAGE_H_
#define JUDGECORESERVER_DTO_RESOURCE_USAGE_H_

#include <fmt/format.h>

namespace himu::dto
{

	struct ResourceUsage
	{
		long time   = -1;///< The time used by the program, in milliseconds
		long memory = -1;///< The memory used by the program, in bytes

		ResourceUsage() = default;
		ResourceUsage(long time, long memory) : time(time), memory(memory)
		{
		}
		ResourceUsage(const ResourceUsage &) = default;
	};

}// namespace himu::dto

/**
 * @brief Format a ResourceUsage enum value for debugging.
 */
template<>
struct fmt::formatter<himu::dto::ResourceUsage> : formatter<string_view>
{
	// parse is inherited from formatter<string_view>.
	auto format(const himu::dto::ResourceUsage &resource_usage, format_context &ctx) const
	{
		// clang-format off
		return formatter<string_view>::format(
			fmt::format(R"(ResourceUsage{{time: {0}, memory: {1}}})",
						resource_usage.time, resource_usage.memory),
			ctx
		);
		// clang-format on
	}
};

#endif