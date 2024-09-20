#include "Comparer.h"
#include "CharComparer.h"
#include "LineComparer.h"

namespace himu
{

std::unique_ptr<BaseComparer> ComparerBuilder::Get(int mode)
{
	switch (mode & ComparerMode::TokenMarks)
	{
		case ComparerMode::Char:
			return std::make_unique<CharComparer>(mode);
		case ComparerMode::Line:
			return std::make_unique<LineComparer>(mode);
		default:
			return nullptr;
	}

	// impossible to reach
	return nullptr;
}

}// namespace himu