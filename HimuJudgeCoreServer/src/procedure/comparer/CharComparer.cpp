#include "CharComparer.h"
#include "utils/Utils.h"

int himu::CharComparer::GetToken(FILE *file, char *buffer, int maxSize) const
{
	int cnt = fread(buffer, sizeof(char), 1, file);
	if (cnt == 0)
		return (feof(file) ? 0 : -1);
	return cnt;
}

std::optional<judge::JudgeDifference> himu::CharComparer::Compare(FILE *outputStream, FILE *expectedStream) const
{
	return BaseComparer::Compare<1>(outputStream, expectedStream);
}
