#pragma once

#ifndef HIMU_JUDGE_CORE_JUDGE_SERVICE_H
#define HIMU_JUDGE_CORE_JUDGE_SERVICE_H

#include "protos/Judge.grpc.pb.h"

namespace himu
{

class JudgeService final : public judge::JudgeService::Service
{
	virtual grpc::Status ProcessJudgeTask(
		grpc::ServerContext *context, grpc::ServerReaderWriter<judge::JudgeResult, judge::JudgeTask> *stream) override;

	virtual grpc::Status Judge(grpc::ServerContext *context,
							   const judge::JudgeTask *request,
							   judge::JudgeResult *response) override;
};

}// namespace himu

#endif