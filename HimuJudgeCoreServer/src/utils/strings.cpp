#include "pch.h"
#include "Utils.h"

const std::string &himu::utils::ReplaceAllSubString(std::string &src,
													const std::string &subStr,
													const std::string &replaceStr)
{
	std::string::size_type pos = 0;
	while ((pos = src.find(subStr, pos)) != std::string::npos)
	{
		src.replace(pos, subStr.length(), replaceStr);
	}
	return src;
}