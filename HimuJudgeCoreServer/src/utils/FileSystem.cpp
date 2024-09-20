#include "Utils.h"
#include <filesystem>
#include <type_traits>

errno_t himu::utils::OpenStdFile(FILE **file, const std::filesystem::path &filePath, const char *mode)
{
	using TChar = typename std::filesystem::path::value_type;
	if constexpr (std::is_same_v<TChar, char>)
	{
		return fopen_s(file, filePath.string().c_str(), mode);
	}
	else if constexpr (std::is_same_v<TChar, wchar_t>)
	{
		size_t modeLen = strlen(mode);
		if (modeLen >= 8)
			return EINVAL; 
		wchar_t wMode[8];
		memset(wMode, 0, sizeof(wMode));
		mbstowcs(wMode, mode, modeLen);
#if _WIN32
		// For Windows, we can use _wfopen_s
		return _wfopen_s(file, filePath.wstring().c_str(), wMode);
#else
		// For other platforms, we use fopen_s directly
		return fopen_s(file, filePath.string().c_str(), mode);
#endif
	}
	else
	{
		return EINVAL;
	}
}

errno_t himu::utils::OpenStdFile(FILE **file, const std::string &filePath, const char *mode)
{
	return fopen_s(file, filePath.c_str(), mode);
}
