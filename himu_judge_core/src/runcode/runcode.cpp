#include "runcode.h"
#include "linux_code_runner.h"

namespace himu
{
	[[nodiscard]]
	std::unique_ptr<BaseCodeRunner>
	createCodeRunner(const himu::CodeRunnerLimit &limit)
	{
#ifdef __linux__
		auto runner = std::make_unique<code_runner::LinuxCodeRunner>(limit);
		if (!runner->successCreated()) return nullptr;
		return runner;
#else
		// TODO: Support other platforms
		return nullptr;
#endif
	}

}// namespace himu