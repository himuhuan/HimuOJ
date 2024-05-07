 /** 
 * Copyright (c) 2024 Himu, all rights reserved.
 * 
 * Version 1.2.0 (build on 2024/05/07):
 *	- Linux: Add limit of the running time and memory usage of the program
 *  - Linux: Add detection of terminated program by Posix signal
 * 
 * Redistribution and use in source and binary forms, with or without modification,
 * are permitted provided that the following conditions are met:
 *
 *   Redistributions of source code must retain the above copyright notice, this
 *   list of conditions and the following disclaimer.
 *
 *   Redistributions in binary form must reproduce the above copyright notice, this
 *   list of conditions and the following disclaimer in the documentation and/or
 *   other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 * ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

#include "dbcontext/dbcontext.h"
#include "launcher/launcher.h"
#include "shared/config/app_config.h"
#include "shared/logger.h"
#include "server/judge_server.h"

///////////////////////////// GLOBAL VARIABLES /////////////////////////////////

std::unique_ptr<himu::config::AppConfig> gAppConfig;
std::unique_ptr<himu::dbcontext::DbContextFactory> gDbContextFactory;
std::unique_ptr<himu::launcher::Launcher> gLauncher;

void initializeServer();

int main()
{
	initializeServer();
	himu::server::JudgeServer server(5000);
	return server.run();
}

void initializeServer()
{
	ServerLogger::initialize("himuoj", nullptr);
	gAppConfig = std::make_unique<himu::config::AppConfig>("appconfig.json");
	gDbContextFactory =
		std::make_unique<himu::dbcontext::DbContextFactory>("mysql", *gAppConfig->database);
	gLauncher = std::make_unique<himu::launcher::Launcher>(20);
}
