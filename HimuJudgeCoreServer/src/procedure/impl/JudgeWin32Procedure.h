#pragma once

#ifndef HIMU_JUDGE_CORE_JUDGEWIN32PROCEDURE_H
#define HIMU_JUDGE_CORE_JUDGEWIN32PROCEDURE_H

namespace himu
{

class JudgeWin32ProcedureImpl;

class JudgeWin32Procedure : public JudgeProcedureBase<JudgeWin32Procedure>
{
public:
	std::optional<judge::JudgeResult> Execute(const judge::JudgeTask &task);
	const char *Platform() const;

private:
	std::unique_ptr<JudgeWin32ProcedureImpl> _impl;
};

}// namespace himu

#endif// !HIMU_JUDGE_CORE_JUDGEWIN32PROCEDURE_H
