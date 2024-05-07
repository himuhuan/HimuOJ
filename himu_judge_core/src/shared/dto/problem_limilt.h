#ifndef JUDGECORESERVER_DTO_PROBLEM_LIMIT_H
#define JUDGECORESERVER_DTO_PROBLEM_LIMIT_H

namespace himu::dto
{

/**
 * @brief Max memory limit and max execute time limit of a problem.
 */
struct ProblemLimit
{
	long maxMemoryLimitByte;
	// The unit of maxExecuteTimeLimitTick is tick. 1 ms = 10000 tick.
	long maxExecuteTimeLimitTick;

	ProblemLimit(long _maxMemoryLimitByte, long _maxExecuteTimeLimit)
		: maxMemoryLimitByte(_maxMemoryLimitByte), maxExecuteTimeLimitTick(_maxExecuteTimeLimit)
	{
	}
};

}// namespace himu::dto
#endif// !JUDGECORESERVER_DTO_PROBLEM_LIMIT_H
