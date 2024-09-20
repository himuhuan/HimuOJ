cmake_minimum_required (VERSION 3.16)

include_directories(.)

add_executable (HimuJudgeCoreServer)

set(SRC_DIR ${CMAKE_CURRENT_SOURCE_DIR})

file(GLOB_RECURSE HimuJudgeCoreServerSrc
	"${SRC_DIR}/*.cc"
    "${SRC_DIR}/*.cpp"
    "${SRC_DIR}/*.h"
)

target_sources(HimuJudgeCoreServer PUBLIC ${HimuJudgeCoreServerSrc})

target_precompile_headers(HimuJudgeCoreServer PUBLIC "pch.h")

find_package(spdlog CONFIG REQUIRED)
find_package(fmt CONFIG REQUIRED)
find_package(gRPC CONFIG REQUIRED)
find_package(RapidJSON CONFIG REQUIRED)

target_link_libraries ( 
	HimuJudgeCoreServer PRIVATE 
	gRPC::grpc++ gRPC::grpc++_reflection gRPC::gpr gRPC::grpc gRPC::grpc++ 
	protobuf::libprotoc protobuf::libprotobuf protobuf::libprotobuf-lite 
	fmt::fmt
	spdlog::spdlog
	rapidjson)