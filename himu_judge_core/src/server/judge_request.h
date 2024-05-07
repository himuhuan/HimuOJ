#pragma once

#ifndef JUDGECORESERVER_SERVER_REQUEST_H
#define JUDGECORESERVER_SERVER_REQUEST_H

#include <array>
#include <cerrno>
#include <cstddef>
#include <cstdint>
#include <cstdlib>
#include <cstring>
#include <memory>
#include <string_view>

namespace
{

// MSVC do not has strdup
char *stringDuplicate(const char *src)
{
	size_t len = strlen(src) + 1;
	char *res  = new char[len];
	return static_cast<char *>(std::memcpy(res, src, len));
}

}// namespace

namespace himu::server
{

struct ServerRequestDecodeResult
{
	std::uint32_t length;
	std::uint32_t type;
	std::uint64_t commitId;
	char *message;

	ServerRequestDecodeResult() : length {0}, type {0}, commitId {0}, message {nullptr}
	{
	}
	~ServerRequestDecodeResult()
	{
		delete[] message;
	}
};

/**
 * The request message from the client to the server.
 * @details
 * The data of the message itself is a string, the format is as follows
 *	- total length (4 unit length)(including the length itself)
 *	- the length of commit id (4 unit length)
 *	- commit id
 *	- message body
 * @example 0023_0015_498845723160645
 * @see ServerRequestDecodedHeader
 */
class ServerRequest
{
public:
	static constexpr std::size_t MAX_BODY_LENGTH     = 128;
	static constexpr std::size_t HEADER_LENGTH       = 8;
	static constexpr std::size_t MAX_DECODE_BUF_SIZE = 20;

private:
	std::array<char, HEADER_LENGTH + MAX_BODY_LENGTH> _data;
	std::size_t _bodyLength;

public:
	char *rawData() noexcept
	{
		return _data.data();
	}

	const char *rawData() const noexcept
	{
		return _data.data();
	}

	char *body() noexcept
	{
		return _data.data() + HEADER_LENGTH;
	}

	const char *body() const noexcept
	{
		return _data.data() + HEADER_LENGTH;
	}

	std::size_t bodyLength() const noexcept
	{
		return _bodyLength;
	}

	std::size_t length() const noexcept
	{
		return _bodyLength + HEADER_LENGTH;
	}

	[[maybe_unused]]
	std::string_view asString() const noexcept
	{
		return std::string_view {_data.data(), length()};
	}

	/**
	 * Decode the request message from the client.
	 * 
	 * @return nullptr if failed to decode
	 * @return decode result if success
	 * @todo check message more strictly
	 * @see ServerRequestDecodeResult
	 */
	std::unique_ptr<ServerRequestDecodeResult> decode()
	{
		if (_data.size() < HEADER_LENGTH)
			return nullptr;

		auto result = std::make_unique<ServerRequestDecodeResult>();
		std::array<char, MAX_DECODE_BUF_SIZE> buf {};
		std::memcpy(buf.data(), _data.data(), 4);

		result->length = static_cast<uint32_t>(std::strtoul(buf.data(), nullptr, 10));
		if (errno == ERANGE || result == 0)
			return nullptr;

		std::memcpy(buf.data(), _data.data() + 4, 4);
		auto commitLength = std::strtoul(buf.data(), nullptr, 10);
		if (errno == ERANGE || commitLength > MAX_DECODE_BUF_SIZE || commitLength == 0)
			return nullptr;
		std::memcpy(buf.data(), _data.data() + HEADER_LENGTH, commitLength);
		result->commitId = std::strtoul(buf.data(), nullptr, 10);
		if (errno == ERANGE)
			return nullptr;

		// has message body
		if (result->length > HEADER_LENGTH + commitLength)
		{
			result->message = stringDuplicate(_data.data() + HEADER_LENGTH + commitLength);
		}
		return result;
	}

	void clear() noexcept
	{
		std::memset(_data.data(), 0, _data.size());
		_bodyLength = 0;
	}
};

}// namespace himu::server
#endif// !JUDGECORESERVER_SERVER_REQUEST_H
