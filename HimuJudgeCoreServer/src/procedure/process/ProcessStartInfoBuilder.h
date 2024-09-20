#pragma once

#ifndef HIMU_JUDGE_CORE_PROCESSSTARTINFOBUILDER_H
#define HIMU_JUDGE_CORE_PROCESSSTARTINFOBUILDER_H

#include "ProcessStartInfo.h"

namespace himu
{

class ProcessStartInfoBuilder
{
	ProcessStartInfo &_startInfo;

public:
	ProcessStartInfoBuilder &TimeLimit(int64_t timeLimit)
	{
		_startInfo.TimeLimit = timeLimit;
		return *this;
	}

	ProcessStartInfoBuilder &MemoryLimit(int64_t memoryLimit)
	{
		_startInfo.MemoryLimit = memoryLimit;
		return *this;
	}

	ProcessStartInfoBuilder &InputRedirection(FILE *inputRedirection)
	{
		_startInfo.InputRedirection = inputRedirection;
		return *this;
	}

	ProcessStartInfoBuilder &Command(const std::string &command)
	{
		_startInfo.Command = command;
		return *this;
	}

	ProcessStartInfoBuilder &OutputRedirection(FILE *outputRedirection)
	{
		_startInfo.RedirectStandardOutput = true;
		_startInfo.OutputRedirection      = outputRedirection;
		return *this;
	}

	ProcessStartInfoBuilder &OutputLimit(int64_t outputLimit)
	{
		_startInfo.OutputLimit = outputLimit;
		return *this;
	}

	ProcessStartInfoBuilder &RedirectStandardOutput(bool redirectStandardOutput)
	{
		_startInfo.RedirectStandardOutput = redirectStandardOutput;
		return *this;
	}

	ProcessStartInfoBuilder &RedirectStandardError(bool redirectStandardError)
	{
		_startInfo.RedirectStandardError = redirectStandardError;
		return *this;
	}

	ProcessStartInfoBuilder(ProcessStartInfo &startInfo) : _startInfo(startInfo)
	{
	}
};

}// namespace himu::stage

#endif// !HIMU_JUDGE_CORE_PROCESSSTARTINFOBUILDER_H
