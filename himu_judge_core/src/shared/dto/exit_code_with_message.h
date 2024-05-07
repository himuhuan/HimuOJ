#ifndef JUDGECORESERVER_SHARED_EXIT_CODE_WITH_MESSAGE_H_
#define JUDGECORESERVER_SHARED_EXIT_CODE_WITH_MESSAGE_H_

#include <fmt/format.h>
#include <string>

namespace himu::dto
{

/// \brief (exit code, message)
struct ExitCodeWithMessage
{
	int exitCode;
	std::string message;

	ExitCodeWithMessage(int exitCode, std::string message) noexcept
		: exitCode(exitCode), message(std::move(message))
	{
	}

	ExitCodeWithMessage() noexcept : exitCode(0), message("DONE")
	{
	}

	explicit ExitCodeWithMessage(const std::pair<int, std::string> &pair) noexcept
		: exitCode(pair.first), message(pair.second)
	{
	}

	explicit ExitCodeWithMessage(std::pair<int, std::string> &&pair) noexcept
		: exitCode(pair.first), message(std::move(pair.second))
	{
	}
};

}// namespace himu::dto
/**
 * @brief Format a ExitCodeWithMessage for debugging.
 */
template<> struct fmt::formatter<himu::dto::ExitCodeWithMessage> : formatter<string_view>
{
	// parse is inherited from formatter<string_view>.
	auto format(const himu::dto::ExitCodeWithMessage & exit_code_with_message, format_context &ctx) const
	{
		// clang-format off
		return formatter<string_view>::format(
			fmt::format(R"(ExitCodeWithMessage{{(code = {0}): "{1}"}})",
						exit_code_with_message.exitCode, exit_code_with_message.message),
			ctx
		);
		// clang-format on
	}
};

#endif// JUDGECORESERVER_SHARED_EXIT_CODE_WITH_MESSAGE_H_