#include "shared/dto/program_execution_status.h"
#include <unordered_map>

namespace himu::dto
{
// clang-format off
static const std::unordered_map<std::string, himu::dto::ProgramExecutionStatus>
	strToProgramExecutionStatusMap = 
    {
		{"ACCEPTED", himu::dto::ProgramExecutionStatus::ACCEPTED},
		{"PENDING", himu::dto::ProgramExecutionStatus::PENDING},
		{"RUNNING", himu::dto::ProgramExecutionStatus::RUNNING},
		{"WRONG_ANSWER", himu::dto::ProgramExecutionStatus::WRONG_ANSWER},
		{"TIME_LIMIT_EXCEEDED", himu::dto::ProgramExecutionStatus::TIME_LIMIT_EXCEEDED},
		{"MEMORY_LIMIT_EXCEEDED", himu::dto::ProgramExecutionStatus::MEMORY_LIMIT_EXCEEDED},
		{"RUNTIME_ERROR", himu::dto::ProgramExecutionStatus::RUNTIME_ERROR},
		{"COMPILE_ERROR", himu::dto::ProgramExecutionStatus::COMPILE_ERROR},
		{"INTERNAL_ERROR", himu::dto::ProgramExecutionStatus::INTERNAL_ERROR},
    };
// clang-format on

ProgramExecutionStatus toProgramExecutionStatus(std::string str)
{
	for (auto &c : str)
	{
		c = toupper(c);
	}
	return strToProgramExecutionStatusMap.at(str);
}

}// namespace himu::dto