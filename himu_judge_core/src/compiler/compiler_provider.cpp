#include "compiler_provider.h"
#include "compiler/gcc_compiler_provider.h"

namespace himu
{
	std::unique_ptr<BaseCompilerProvider> createCompilerProvider(const char *compilerName)
{
	if (!strcmp(compilerName, "g++"))
	{
		auto compilerPtr =
			new compiler::GccCompilerProvider("g++", {"-O0", "-std=c++17", "-ftrapv"});
		return std::unique_ptr<BaseCompilerProvider>(compilerPtr);
	}
	return nullptr;
}
}// namespace himu
