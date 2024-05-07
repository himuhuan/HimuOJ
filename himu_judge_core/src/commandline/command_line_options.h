
#ifndef COMMAND_LINE_OPTIONS_H
#define COMMAND_LINE_OPTIONS_H

#include "cmdline.h"
#include "shared/bits.h"

class CommandLineOptions {
public:
    CommandLineOptions(int argc, char **argv);
    ~CommandLineOptions() = default;

    [[nodiscard]] std::string_view serverHost() const;
    [[nodiscard]] int serverPort() const;
    [[nodiscard]] bool isDebugMode() const noexcept;
private:
    cmdline::parser _parser{};
    std::string _serverHost;
    int _serverPort;
    bool _debugMode;
    void parserSetup();
};

#endif  // COMMAND_LINE_OPTIONS_H
