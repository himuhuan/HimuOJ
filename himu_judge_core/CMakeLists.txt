cmake_minimum_required(VERSION 3.22)
project(himu_judge_core_server)

set(CMAKE_CXX_STANDARD 17)

set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_SOURCE_DIR}/bin)

if (CMAKE_BUILD_TYPE STREQUAL Debug)
    add_definitions(-D_DEBUG)
endif ()

find_package(spdlog CONFIG REQUIRED)
find_package(fmt CONFIG REQUIRED)
find_package(RapidJSON CONFIG REQUIRED)

find_package(Sqlpp11
    COMPONENTS MySQL
    CONFIG 
    REQUIRED)

add_subdirectory(src)

