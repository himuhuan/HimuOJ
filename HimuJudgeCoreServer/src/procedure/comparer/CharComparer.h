#pragma once

#ifndef HIMU_JUDGE_CORE_CHARCOMPARER_H
#define HIMU_JUDGE_CORE_CHARCOMPARER_H

#include "Comparer.h"

namespace himu
{

class CharComparer final : public BaseComparer
{
public:
	// Trim is not supported in Char mode
	explicit CharComparer(int mode) 
		: BaseComparer(mode & ~(ComparerMode::Trim))
	{
	}

	int GetToken(FILE *file, char *buffer, int maxSize) const override;

	std::optional<judge::JudgeDifference> Compare(FILE *outputStream, FILE *expectedStream) const override;
};

}// namespace himu

#endif// HIMU_JUDGE_CORE_CHARCOMPARER_H