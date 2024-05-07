#ifndef JUDGECORESERVER_DTO_PROGRAM_EXECUTION_STATUS_H_
#define JUDGECORESERVER_DTO_PROGRAM_EXECUTION_STATUS_H_

#include <fmt/format.h>

namespace himu::dto
{

enum class ProgramExecutionStatus
{
	ACCEPTED,
	PENDING,
	RUNNING,
	WRONG_ANSWER,
	TIME_LIMIT_EXCEEDED,
	MEMORY_LIMIT_EXCEEDED,
	RUNTIME_ERROR,
	/// \brief only use in compile stage
	COMPILE_ERROR,
	INTERNAL_ERROR
};

ProgramExecutionStatus toProgramExecutionStatus(std::string str);

}// namespace himu::dto

/**
 * @brief Format a ProgramExecutionStatus enum value for debugging.
 */
template<> struct fmt::formatter<himu::dto::ProgramExecutionStatus> : formatter<string_view>
{
	// parse is inherited from formatter<string_view>.
	auto format(himu::dto::ProgramExecutionStatus status, format_context &ctx) const
	{
		using namespace himu::dto;
		std::string_view name;
		switch (status)
		{
		case ProgramExecutionStatus::ACCEPTED:
			name = "ACCEPTED";
			break;
		case ProgramExecutionStatus::RUNNING:
			name = "RUNNING";
			break;
		case ProgramExecutionStatus::WRONG_ANSWER:
			name = "WRONG_ANSWER";
			break;
		case ProgramExecutionStatus::TIME_LIMIT_EXCEEDED:
			name = "TIME_LIMIT_EXCEEDED";
			break;
		case ProgramExecutionStatus::MEMORY_LIMIT_EXCEEDED:
			name = "MEMORY_LIMIT_EXCEEDED";
			break;
		case ProgramExecutionStatus::RUNTIME_ERROR:
			name = "RUNTIME_ERROR";
			break;
		case ProgramExecutionStatus::COMPILE_ERROR:
			name = "COMPILE_ERROR";
			break;
		case ProgramExecutionStatus::INTERNAL_ERROR:
			name = "INTERNAL_ERROR";
			break;
		case ProgramExecutionStatus::PENDING:
			name = "PENDING";
			break;
		default:
			name = "UNKNOWN_ERROR";
			break;
		}
		return formatter<string_view>::format(name, ctx);
	}
};

#endif