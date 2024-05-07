#ifndef JUDGECORESERVER_COMPILER_COMPILER_PROVIDER_H_
#define JUDGECORESERVER_COMPILER_COMPILER_PROVIDER_H_

#include "base_compiler_provider.h"

namespace himu
{

/**
 * @brief Create a compile provider according to the type of source code.
 * @param compilerName The name of the compiler. Now only support "g++".
 * @return A unique pointer to the compile provider.
*/
std::unique_ptr<BaseCompilerProvider> createCompilerProvider(const char *compilerName);

}// namespace himu
#endif// JUDGECORESERVER_COMPILER_COMPILER_PROVIDER_H_
