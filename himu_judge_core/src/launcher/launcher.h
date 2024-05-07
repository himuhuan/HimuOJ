#ifndef JUDGECORESERVER_LAUNCHER_H
#define JUDGECORESERVER_LAUNCHER_H

#include "shared/utils/task_pool.h"
#include <memory>

namespace himu::launcher
{

/**
 * @brief Launcher launch a task for each commit, which is responsible for compiling and running the code.
 * @details Launcher is responsible for:
 * - Manage thread pool.
 * - If the thread pool is full, Launcher will wait until a thread is available. 
 * - Else creating a new process for the commit, and restrict the process's resource usage.
 * - After the process exits, Launcher will collect the result and update the database.
 */
class Launcher
{
public:
	explicit Launcher(size_t maxThread);

	~Launcher();

	/**
	 * @brief submit a commit task
	*/
	void submitTask(long commitId);

private:
	util::TaskPool _pool;
	static void launcherTask(long commitId);
};

std::unique_ptr<Launcher> createLauncher(size_t maxThread);

}// namespace himu::launcher

extern std::unique_ptr<himu::launcher::Launcher> gLauncher;

#endif// JUDGECORESERVER_LAUNCHER_H
