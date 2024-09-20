#pragma once

#include <iostream>
#include <memory>
#include <string>
#include <filesystem>

#include <spdlog/async.h>
#include <spdlog/sinks/daily_file_sink.h>
#include <spdlog/sinks/stdout_color_sinks.h>
#include <spdlog/spdlog.h>
#include <fmt/core.h>
#include <fmt/std.h>

#if _WIN32

// For Windows, we need to define NOMINMAX to avoid conflicts with std::min and std::max
#define NOMINMAX

#include <Windows.h>

#endif

#include "shared/Shared.h"