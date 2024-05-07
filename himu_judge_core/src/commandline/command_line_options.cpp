#include "command_line_options.h"


void CommandLineOptions::parserSetup() {
    _parser.add<std::string>("server", 'S', "server host", true);
    _parser.add<int>("port", 'p', "server port", true);
    _parser.add("debug", '\0', "debug mode (detail output)");
}

CommandLineOptions::CommandLineOptions(int argc, char **argv) {
    parserSetup();
    _parser.parse_check(argc, argv);
    _serverHost = _parser.get<std::string>("server");
    _serverPort = _parser.get<int>("port");
    _debugMode = _parser.exist("debug");
}

std::string_view CommandLineOptions::serverHost() const {
    return _serverHost;
}

int CommandLineOptions::serverPort() const {
    return _serverPort;
}
bool CommandLineOptions::isDebugMode() const noexcept {
    return _debugMode;
}
