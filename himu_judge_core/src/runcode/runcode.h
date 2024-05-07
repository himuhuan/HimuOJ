#ifndef JUDGECORESERVER_RUNCODE_H
#define JUDGECORESERVER_RUNCODE_H

#include "basic_code_runner.h"

namespace himu
{

/**
 * @brief Get a code runner for current platform
 */
std::unique_ptr<BaseCodeRunner> createCodeRunner(const himu::CodeRunnerLimit& limit);

}// namespace himu

#endif// JUDGECORESERVER_RUNCODE_H
