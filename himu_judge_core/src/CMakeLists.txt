cmake_minimum_required(VERSION 3.10)

include_directories(.)

add_executable(judge_core_server
        judge_core_server.cpp

        shared/bits.h
        shared/singleton.h
        shared/logger.cpp
        shared/logger.h
        
        "shared/global_variables.h"

        "shared/models/mysql/point_result.h"
        "shared/models/mysql/test_point.h"
        "shared/models/mysql/problem.h"

        "shared/dto/diff_result.h"
        "shared/dto/exit_code_with_message.h"
        "shared/dto/program_execution_status.h"        
        "shared/dto/resource_usage.h"        
        "shared/dto/testpoint.h"
        "shared/dto/user_commit.cpp"
        "shared/dto/problem_limilt.h"
        

        commandline/command_line_options.h
        commandline/command_line_options.cpp
        commandline/command_line_options.cpp
        
        compiler/base_compiler_provider.h
        compiler/gcc_compiler_provider.cpp
        compiler/gcc_compiler_provider.h
        compiler/compiler_provider.h
        compiler/compiler_provider.cpp

        runcode/basic_code_runner.h
        runcode/linux_code_runner.cpp
        runcode/linux_code_runner.h
        "shared/dto/run_code_case_result.h"
        
        runcode/runcode.h
        runcode/runcode.cpp

        result_compare/stream_result_compare.h
        result_compare/word_token_reader.h
        result_compare/word_token_reader.cpp

        "dbcontext/dbcontext.h"
        "dbcontext/mysql_dbcontext.cpp"
        dbcontext/mysql_dbcontext.h
        "dbcontext/dbcontext_factory.cpp"
        
        "shared/utils/to_program_execution_status.cpp" 
        
        "shared/config/app_config.h"
        "shared/config/app_config.cpp"
        "shared/error_code.h" 
        "shared/config/dbcontext_config.h" 
        "shared/dto/testpoint_result.cpp" "shared/dto/user_commit.h"
		launcher/launcher.h
		shared/utils/concurrent_queue.hpp
		shared/utils/task_pool.h

        "launcher/launcher.cpp"
		compiler/compile_result.h
        
        "server/judge_request.h"
        "server/judge_server.h"  
        "server/judge_server.cpp" 
        "server/judge_session.h"
 "runcode/basic_code_runner.cpp")

target_link_libraries(judge_core_server PUBLIC fmt::fmt)
target_link_libraries(judge_core_server PRIVATE spdlog::spdlog)
target_link_libraries(judge_core_server PRIVATE MySQL::MySQL sqlpp11::mysql sqlpp11::sqlpp11)

# required by sqlpp11
if (CMAKE_HOST_UNIX)
	target_link_libraries(judge_core_server PRIVATE pthread resolv ssl crypto zstd z)
endif()