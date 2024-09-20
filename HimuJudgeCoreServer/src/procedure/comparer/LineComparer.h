#pragma once

#ifndef HIMU_JUDGE_CORE_LINECOMPARER_H
#define HIMU_JUDGE_CORE_LINECOMPARER_H

#include "Comparer.h"

namespace himu
{

class LineComparer final : public BaseComparer
{
public:
	explicit LineComparer(int mode) : BaseComparer(mode)
	{
	}

	int GetToken(FILE *file, char *buffer, int maxSize) const override;

	std::optional<judge::JudgeDifference> Compare(FILE *outputStream, FILE *expectedStream) const override;
};

}// namespace himu

#endif// HIMU_JUDGE_CORE_CHARCOMPARER_H