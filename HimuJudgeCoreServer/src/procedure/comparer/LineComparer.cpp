#include "LineComparer.h"

// for line mode, we remove the last '\n' character
int himu::LineComparer::GetToken(FILE *file, char *buffer, int maxSize) const
{
	memset(buffer, 0, maxSize);
	if (fgets(buffer, maxSize, file) == nullptr)
		return feof(file) ? 0 : -1;
	size_t len = strlen(buffer);
#if _WIN32
	// Ugly windows line ending
	if (len > 0 && buffer[len - 2] == '\r')
	{
		buffer[len - 2] = '\0';
		return len - 2;
	}
#endif
	if (len > 0 && buffer[len - 1] == '\n')
		buffer[len - 1] = '\0';
	return len - 1;
}

std::optional<judge::JudgeDifference> himu::LineComparer::Compare(FILE *outputStream, FILE *expectedStream) const
{
	return BaseComparer::Compare<kMaxComparerLineLength>(outputStream, expectedStream);
}
