#ifndef JUDGECORESERVER_WORD_TOKEN_READER_H_
#define JUDGECORESERVER_WORD_TOKEN_READER_H_

#include <cstdio>
#include <string>

class WordTokenReader {
public:
	explicit WordTokenReader(FILE *stream);
	[[nodiscard]] bool hasNext() const noexcept;
	std::string next();
private:
	FILE *m_stream;
};

#endif // !JUDGECORESERVER_WORD_TOKEN_READER_H_
