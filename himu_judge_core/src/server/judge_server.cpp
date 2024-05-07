#include "judge_server.h"
#include "judge_session.h"
#include <asio.hpp>

using namespace asio::ip;

namespace himu::server
{

struct JudgeServer::JudgeServerImpl
{
	asio::io_context context;
	tcp::acceptor acceptor;

	explicit JudgeServerImpl(int port) : acceptor(context, tcp::endpoint(tcp::v4(), port))
	{
		startAccept();
	}

	~JudgeServerImpl()
	{
		context.stop();
	}

	void run()
	{
		MyLogger.info(
			"JudgeServer is listening on localhost:{0}", acceptor.local_endpoint().port());
		context.run();
	}

private:
	void startAccept()
	{
		acceptor.async_accept([this](std::error_code ec, tcp::socket socket) {
			if (!ec)
			{
				std::make_shared<JudgeSession>(std::move(socket))->start();
			}
			startAccept();
		});
	}
};

JudgeServer::JudgeServer(int port) : _impl(std::make_unique<JudgeServerImpl>(port))
{
}

// g++: invalid application of ‘sizeof’ to incomplete type
JudgeServer::~JudgeServer() = default;

int JudgeServer::run()
{
	try
	{
		_impl->run();
	}
	catch (std::exception &e)
	{
		MyLogger.error("error: {0}", e.what());
		return EXIT_FAILURE;
	}
	return 0;
}

}// namespace himu::server