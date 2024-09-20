cmake_minimum_required (VERSION 3.16)

project ("HimuJudgeCoreServer")

if (CMAKE_CXX_COMPILER_ID STREQUAL "MSVC")
	# for some Windows API
	add_definitions("-DUNICODE" "-D_UNICODE")
	# set_property(TARGET HimuJudgeCoreServer PROPERTY VS_DEBUGGER_WORKING_DIRECTORY "${ProjectDir}..")
endif()

add_subdirectory(src)

if (CMAKE_VERSION VERSION_GREATER 3.12)
  set_property(TARGET HimuJudgeCoreServer PROPERTY CXX_STANDARD 20)
endif()
