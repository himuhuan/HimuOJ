#include "pch.h"
#include "shared/Shared.h"

#include <grpcpp/grpcpp.h>
#include <grpcpp/health_check_service_interface.h>
#include <grpcpp/ext/proto_server_reflection_plugin.h>

#include "HimuServices.h"

// Services to register
#include "JudgeService.h"

namespace himu
{

void himu::BuildServerAndStart(himu::ServerConfiguration config)
{
	const std::string kServerAddress = fmt::format("{}:{}", config.Host, config.Port);
	JudgeService judgeService;
	grpc::EnableDefaultHealthCheckService(true);
	grpc::reflection::InitProtoReflectionServerBuilderPlugin();
	grpc::ServerBuilder builder;
	builder.AddListeningPort(kServerAddress, grpc::InsecureServerCredentials());
	builder.RegisterService(&judgeService);
	std::unique_ptr<grpc::Server> server = builder.BuildAndStart();
	LOGGER->info("Server listening on {}", kServerAddress);
	server->Wait();
}

}// namespace himu