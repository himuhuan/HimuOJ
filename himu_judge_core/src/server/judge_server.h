/**
 * @file judge_server.h
 * @author himu
 * @version 0.1
 * @brief The server that listens to the incoming judge requests.
 * 
 * JudgeServer is a server that listens to the incoming judge requests (maybe from other server).
 * Normally, it listens to the incoming requests from the web server at port 5500.
 * The server will then process the requests and send the result back to the web server.
 * The request packet just contains the commit id, and the server starts to judge the commit and 
 * simply sends a status code back to client.
*/

#ifndef JUDGECORESERVER_SERVER_H
#define JUDGECORESERVER_SERVER_H

#include <memory>

namespace himu::server
{

class JudgeServer
{
public:
	explicit JudgeServer(int port);
	~JudgeServer();
	int run();
private:
	struct JudgeServerImpl;
	std::unique_ptr<JudgeServerImpl> _impl;
};

}// namespace himu::server

#endif//! JUDGECORESERVER_SERVER_H