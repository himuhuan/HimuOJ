#ifndef HIMU_JUDGE_CORE_UTILS_H
#define HIMU_JUDGE_CORE_UTILS_H

#include <cctype> // for std::tolower
#include <cwctype>// for std::towlower
#include <type_traits>

namespace himu::utils
{

const std::string &ReplaceAllSubString(std::string &src, const std::string &subStr, const std::string &replaceStr);

template<typename CharT>
void ToLower(CharT *str, size_t len)
{
	if constexpr (std::is_same_v<CharT, char>)
	{
		for (size_t i = 0; i < len; ++i)
			str[i] = std::tolower(str[i]);
	}
	else if constexpr (std::is_same_v<CharT, wchar_t>)
	{
		for (size_t i = 0; i < len; ++i)
			str[i] = std::towlower(str[i]);
	}
	else
	{
		static_assert(false, "Unsupported char type");
	}
}

template<typename CharT>
size_t Trim(CharT *str, size_t len)
{
	size_t start = 0, end = len, newLen = len;
	if constexpr (std::is_same_v<CharT, char>)
	{
		while (start < len && std::isspace(str[start]))
			++start;
		while (end > start && std::isspace(str[end - 1]))
			--end;
	}
	else if constexpr (std::is_same_v<CharT, wchar_t>)
	{
		while (start < len && std::iswspace(str[start]))
			++start;
		while (end > start && std::iswspace(str[end - 1]))
			--end;
	}
	else
	{
		static_assert(false, "Unsupported char type");
	}

	if (start > 0)
	{
		for (size_t i = start; i < end; ++i)
			str[i - start] = str[i];
	}

	newLen           = end - start;
	str[end - start] = 0;
	return newLen;
}

template<typename CharT, typename Pred>
void RemoveChar(CharT *str, size_t len, Pred p)
{
	size_t j = 0;
	for (size_t i = 0; i < len; ++i)
	{
		if (!Pred(str[i]))
			str[j++] = str[i];
	}
	str[j] = 0;
}

errno_t OpenStdFile(FILE **file, const std::filesystem::path &filePath, const char *mode);

errno_t OpenStdFile(FILE **file, const std::string &filePath, const char *mode);

}// namespace himu::utils

#endif// !HIMU_JUDGE_CORE_UTILS_H
