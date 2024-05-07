#ifndef JUDGECORE_SERVER_COMPILER_RUNNER_H_
#define JUDGECORE_SERVER_COMPILER_RUNNER_H_

#include <utility>

#include "shared/bits.h"
#include "compile_result.h"

class BaseCompilerProvider {
public:
    BaseCompilerProvider(fs::path compilerPath, std::initializer_list<std::string> compilerArgs)
        : _compilerPath(std::move(compilerPath)), _compilerArgs(compilerArgs) {
    }

    virtual ~BaseCompilerProvider() = default;

    /**
     * @brief Compile source code
     * @param source Source code or file
     */
    virtual CompileResult compile(fs::path source) = 0;

public:
    [[nodiscard]] const fs::path &compilerPath() const {
        return _compilerPath;
    }

    [[nodiscard]] const std::vector<std::string> &compilerArgs() const {
        return _compilerArgs;
    }

protected:
    fs::path _compilerPath;
    std::vector<std::string> _compilerArgs;
};

#endif  // JUDGECORE_SERVER_COMPILER_RUNNER_H_