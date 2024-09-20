#pragma once

#ifndef HIMU_JUDGE_CORE_JUDGEPROCEDURE_H
#define HIMU_JUDGE_CORE_JUDGEPROCEDURE_H

#include "protos/Judge.pb.h"

namespace himu
{

// clang-format off
template<typename T>
concept ProcedureImplementable = requires 
{ 
	requires requires(T &impl, const judge::JudgeTask &task) 
	{
		{impl.Execute(task)} -> std::same_as<std::optional<judge::JudgeResult>>;
	};

	requires requires(T &impl) 
	{
		{impl.Platform()} -> std::convertible_to<const char *>;
	};
};
// clang-format on

template<typename Implementation>
class JudgeProcedureBase
{
private:
	const judge::JudgeTask &_task;

public:
	JudgeProcedureBase(const judge::JudgeTask &task) : _task(task)
	{
		static_assert(himu::ProcedureImplementable<Implementation> &&
						  std::derived_from<Implementation, JudgeProcedureBase>,
					  "Implementation does not meet the ProcedureImplementable requirements.");
	}

	[[nodiscard]]
	std::optional<judge::JudgeResult> Execute()
	{
		return static_cast<Implementation *>(this)->Execute(_task);
	}

	static JudgeProcedureBase<Implementation> Create(const judge::JudgeTask &task)
	{
		return JudgeProcedureBase<Implementation>(task);
	}
};

struct CompileResult
{
	std::filesystem::path TargetPath;
	std::string Message;
	int ExitCode = 0;

	CompileResult() = default;
};

}// namespace himu
#if _WIN32
#include "procedure/impl/JudgeWin32Procedure.h"
using JudgeProcedure = himu::JudgeProcedureBase<himu::JudgeWin32Procedure>;
#else
static_assert(false, "No implementation for JudgeProcedure.");
#endif

#endif//! HIMU_JUDGE_CORE_JUDGEPROCEDURE_H