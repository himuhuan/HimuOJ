#pragma once

#ifndef HIMU_JUDGE_CORE_COMPARERBASE_H
#define HIMU_JUDGE_CORE_COMPARERBASE_H

#include "protos/Judge.pb.h"
#include "utils/Utils.h"

namespace himu
{

enum ComparerMode
{
	Char = 0,
	Line = 1,
	Word = 2,

	// 强制为小写形式
	IgnoreCaseInsensitive = 4,
	// 忽略空白字符, 仅对 Line 有效
	Trim = 8,

	TokenMarks = Char | Line | Word,
};

constexpr int kMaxComparerLineLength = 4096;
constexpr int kMaxComparerWordSize   = 1024;

class BaseComparer
{
protected:
	int _mode;

public:
	explicit BaseComparer(int mode) : _mode(mode)
	{
	}

	~BaseComparer() = default;

	[[nodiscard]]
	virtual std::optional<judge::JudgeDifference> Compare(FILE *outputStream, FILE *expectedStream) const = 0;

	/**
	 * @brief Read a token from the file
	 * @return 0 if EOF, -1 if error, else the number of bytes read
	 */
	[[nodiscard]]
	virtual int GetToken(FILE *file, char *buffer, int maxSize) const = 0;

	template<size_t BufferSize>
	[[nodiscard]]
	std::optional<judge::JudgeDifference> Compare(FILE *outputStream, FILE *expectedStream) const
	{
		auto result = std::make_optional<judge::JudgeDifference>();
		char outputBuffer[BufferSize], expectedBuffer[BufferSize];
		int pos = 0;
		while (true)
		{
			int outputCnt   = GetToken(outputStream, outputBuffer, sizeof(outputBuffer));
			int expectedCnt = GetToken(expectedStream, expectedBuffer, sizeof(expectedBuffer));
			if (outputCnt == -1 || expectedCnt == -1)
				return std::nullopt;
			if (outputCnt == 0 && expectedCnt == 0)
				break;

			++pos;

			if (_mode & IgnoreCaseInsensitive)
			{
				utils::ToLower(outputBuffer, 1);
				utils::ToLower(expectedBuffer, 1);
			}

			if (_mode & Trim)
			{
				utils::Trim(outputBuffer, outputCnt);
				utils::Trim(expectedBuffer, expectedCnt);
			}

			if (outputCnt != expectedCnt || strcmp(outputBuffer, expectedBuffer) != 0)
			{
				result->set_pos(pos);
				result->set_actual(outputBuffer);
				result->set_expected(expectedBuffer);
				return result;
			}
		}
		return result;
	}
};

class ComparerBuilder
{
public:

	[[nodiscard]]
	static std::unique_ptr<BaseComparer> Get(int mode);
};

}// namespace himu

#endif// !HIMU_JUDGE_CORE_COMPARERBASE_H
