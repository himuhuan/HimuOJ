#ifndef JUDGECORESERVER_SHARED_UTILS_TASK_POOL_H
#define JUDGECORESERVER_SHARED_UTILS_TASK_POOL_H

#include <condition_variable>
#include <functional>
#include <future>
#include <memory>
#include <mutex>
#include <thread>

#include "concurrent_queue.hpp"

namespace himu::util
{

class TaskPool
{
private:
	using Task = std::function<void()>;

	ConcurrentQueue<Task> _taskQueue;
	std::vector<std::thread> _threads;
	std::mutex _poolMutex;
	std::condition_variable _conditionalLock;
	bool _shutdown;

	class TaskWorker
	{
	private:
		int _id;
		TaskPool *_pool;

	public:
		TaskWorker(TaskPool *pool, int id) : _id(id), _pool(pool)
		{
		}

		void operator()()
		{
			std::function<void()> func;
			bool dequeued;
			while (!_pool->_shutdown)
			{
				{
					std::unique_lock<std::mutex> lock(_pool->_poolMutex);
					if (_pool->_taskQueue.empty())
					{
						_pool->_conditionalLock.wait(lock);
					}
					dequeued = _pool->_taskQueue.tryPop(func);
				}
				if (dequeued)
				{
					func();
				}
			}
		}
	};

public:
	explicit TaskPool(size_t maxThreads) : _threads(maxThreads), _shutdown(false)
	{
	}

	~TaskPool()
	{
		if (!_shutdown)
		{
			stop();
		}
	}

	TaskPool(const TaskPool &)            = delete;
	TaskPool(TaskPool &&)                 = delete;
	TaskPool &operator=(const TaskPool &) = delete;
	TaskPool &operator=(TaskPool &&)      = delete;

	void start()
	{
		for (int i = 0; i < _threads.size(); ++i)
		{
			_threads[i] = std::thread(TaskWorker(this, i));
		}
	}

	/**
	 * @brief Force stop TaskPool.
	 * All tasks in the queue will be discarded.
	 * But the tasks that are being executed will not be interrupted.
	*/
	void stop()
	{
		_shutdown = true;
		_conditionalLock.notify_all();

		for (auto & _thread : _threads)
		{
			if (_thread.joinable())
			{
				_thread.join();
			}
		}
	}

	/**
	 * @brief Submit a task to the pool.
	 * @tparam Func the type of the task.
	 * @tparam ...Args arguments of the task.
	 * @param f function of the task.
	 * @param ...args arguments of the task.
	 * @return a future of the task.
	*/
	template<typename Func, typename... Args>
	auto submit(Func &&f, Args &&...args)
	{
		std::function<decltype(f(args...))()> func =
			std::bind(std::forward<Func>(f), std::forward<Args>(args)...);
		auto taskPtr = std::make_shared<std::packaged_task<decltype(f(args...))()>>(func);
		std::function<void()> warppedFunc = [taskPtr]() { (*taskPtr)(); };
		_taskQueue.push(warppedFunc);
		_conditionalLock.notify_one();
		return taskPtr->get_future();
	}

	bool hasTask() const noexcept
	{
		return !_taskQueue.empty();
	}

	auto getTaskCount() const noexcept
	{
		return _taskQueue.size();
	}
};

}// namespace himu::util
#endif//JUDGECORESERVER_SHARED_UTILS_TASK_POOL_H