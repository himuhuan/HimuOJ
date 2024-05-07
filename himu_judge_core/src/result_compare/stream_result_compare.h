#ifndef JUDGECORESERVER_STREAM_RESULT_COMPARE_H_
#define JUDGECORESERVER_STREAM_RESULT_COMPARE_H_

#include <cassert>
#include <cstdio>
#include <memory>
#include <string>
#include <tuple>

#include "shared/dto/diff_result.h"
#include "word_token_reader.h"

namespace himu::result_compare
{

/**
 * @brief Compares the difference between user output and expected output
 * @tparam _TokenReader A class for reading tokens from an output stream, must implement hasNext()
 * and next() methods
 * @param userOutputStream A file pointer to the user output, cannot be null
 * @param expectedStream A file pointer to the expected output, cannot be null
 * @return A DiffResult with token count if the user output is same as the expected
 * @return A DiffResult that indicates the difference if the user output is different from the expected
 */
template<typename TokenReader>
dto::DiffResult streamResultCompare(FILE *userOutputStream, FILE *expectedStream)
{
	assert(userOutputStream != nullptr);
	assert(expectedStream != nullptr);

	/*MyLogger.debug("Opening files: {} , {}", static_cast<void *>(expectedStream), static_cast<void *>
                     (userOutputStream)); */

	std::unique_ptr<TokenReader> userOutputReader = std::make_unique<TokenReader>(userOutputStream);
	std::unique_ptr<TokenReader> expectedReader   = std::make_unique<TokenReader>(expectedStream);
	std::string output, expected;

	int tokenCount = 0;
	while (expectedReader->hasNext())
	{
		++tokenCount;
		expected = expectedReader->next();
		if (!userOutputReader->hasNext())
			return {expected, "", tokenCount};
		output = userOutputReader->next();
		if (expected != output)
			return {expected, output, tokenCount};
	}
	if (userOutputReader->hasNext())
		return {"", output, tokenCount};
	return {"", "", tokenCount};
}

}// namespace himu::result_compare
#endif// !JUDGECORESERVER_STREAM_RESULT_COMPARE_H_
