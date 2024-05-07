//
// Created by LiuHuan on 2023/11/16.
//

#ifndef JUDGECORESERVER_GCCCOMPILERPROVIDER_H_
#define JUDGECORESERVER_GCCCOMPILERPROVIDER_H_

#include "base_compiler_provider.h"

namespace himu::compiler
{

class GccCompilerProvider : public BaseCompilerProvider
{
public:
	GccCompilerProvider(const fs::path &compilerPath,
						const std::initializer_list<std::string> &compilerArgs);

	CompileResult compile(fs::path source) override;
};

}// namespace himu::compiler

#endif// JUDGECORESERVER_GCCCOMPILERPROVIDER_H_
