#include "pch.h"
#include "shared/Shared.h"
#include "utils/Utils.h"
#include "procedure/JudgeProcedure.h"
#include <grpcpp/grpcpp.h>

#include "JudgeService.h"

using namespace himu;

namespace himu
{

grpc::Status JudgeService::ProcessJudgeTask(grpc::ServerContext *context,
											grpc::ServerReaderWriter<judge::JudgeResult, judge::JudgeTask> *stream)
{
	judge::JudgeTask task;
	while (stream->Read(&task))
	{
		int64_t commitId      = task.commitid();
		std::string sourceUri = task.sourceuri();
		auto &compilerInfo    = task.compiler();

		LOGGER->info("Received task for commitId: {0}.", commitId);

#if _DEBUG
		LOGGER->debug("============== Now start judge task: commit={0} ==============", commitId);
		LOGGER->debug("Source URI: {0}, using compiler: {1}", sourceUri, compilerInfo.name());
#endif

		bool success = true;
		auto procedureResult = JudgeProcedure::Create(task).Execute();
		if (procedureResult.has_value())
		{
			stream->Write(*procedureResult);
		}
		else
		{
			judge::JudgeResult result;
			result.set_commitid(commitId);
			result.set_status(judge::JudgeStatus::PENDING);
			result.set_status(judge::JudgeStatus::INTERNAL_ERROR);
			stream->Write(result);
		}
	}
	return grpc::Status::OK;
}

grpc::Status JudgeService::Judge(grpc::ServerContext *context,
								 const judge::JudgeTask *request,
								 judge::JudgeResult *response)
{
	int64_t commitId      = request->commitid();
	std::string sourceUri = request->sourceuri();
	LOGGER->debug("Received task for commitId: {0}, sourceUri: {1}", commitId, sourceUri);
	response->set_commitid(commitId);
	response->set_status(judge::JudgeStatus::PENDING);
	return grpc::Status::OK;
}

}// namespace himu
