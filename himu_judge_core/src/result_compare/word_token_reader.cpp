#include "word_token_reader.h"

WordTokenReader::WordTokenReader(FILE *stream) : m_stream(stream) {
}

bool WordTokenReader::hasNext() const noexcept {
    return !feof(m_stream);
}

std::string WordTokenReader::next() {
    int ch;
    while (std::isspace(ch = fgetc(m_stream))) continue;
    if (ch == EOF)
        return "";
    std::string token;
    do {
        token.push_back(static_cast<char>(ch));
    } while (!std::isspace(ch = fgetc(m_stream)) && ch != EOF);
    return token;
}