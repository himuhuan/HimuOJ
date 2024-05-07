#pragma once

#ifndef JUDGECORESERVER_SERVER_SESSION_HPP
#define JUDGECORESERVER_SERVER_SESSION_HPP

#include "judge_request.h"
#include "launcher/launcher.h"
#include "shared/logger.h"
#include <asio.hpp>
#include <memory>

namespace himu::server
{

class JudgeSession : public std::enable_shared_from_this<JudgeSession>
{
private:
	using tcp = asio::ip::tcp;
	tcp::socket _socket;
	ServerRequest _request;

public:
	explicit JudgeSession(asio::io_context &context) : _socket(context)
	{
	}

	explicit JudgeSession(tcp::socket socket) : _socket(std::move(socket))
	{
	}

	void start()
	{
		_start();
	}

private:
	void _start()
	{ 
		auto self {shared_from_this()};
		_socket.async_read_some(
			asio::buffer(_request.rawData(), ServerRequest::MAX_BODY_LENGTH),
			[this, self](std::error_code ec, std::size_t length) {
				if (!ec)
				{
					auto result = _request.decode();
					if (result != nullptr)
					{
						gLauncher->submitTask(result->commitId);
						_reply(std::to_string(0));
					}
					else
					{
						_reply(std::to_string(1));
					}
				}
			});
	}

	void _reply(std::string message)
	{
		auto self {shared_from_this()};
		asio::async_write(
			_socket, asio::buffer(message, message.size()),
			[this, self](std::error_code ec, std::size_t len) {
				if (!ec)
				{
					_request.clear();
					_start();
				}
			});
	}
};

}// namespace himu::server

#endif// !JUDGECORESERVER_SERVER_SESSION_HPP
