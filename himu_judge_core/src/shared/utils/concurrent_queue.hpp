#ifndef JUDGECORESERVER_SHARED_UTILS_CONCURRENT_QUEUE_HPP
#define JUDGECORESERVER_SHARED_UTILS_CONCURRENT_QUEUE_HPP

#include <mutex>
#include <optional>
#include <queue>

namespace himu::util
{

/**
 * ConcurrentQueue is a thread-safe queue.
 */
template<typename T>
class ConcurrentQueue
{
private:
	mutable std::mutex _mutex;
	std::queue<T> _queue;

public:
	ConcurrentQueue()  = default;
	~ConcurrentQueue() = default;

	[[nodiscard]]
	bool empty() const noexcept
	{
		std::lock_guard<std::mutex> lock(_mutex);
		return _queue.empty();
	}

	[[nodiscard]]
	auto size() const noexcept
	{
		std::lock_guard<std::mutex> lock(_mutex);
		return _queue.size();
	}

	void push(const T &value)
	{
		std::lock_guard<std::mutex> lock(_mutex);
		_queue.emplace(value);
	}

	void push(T &&value)
	{
		std::lock_guard<std::mutex> lock(_mutex);
		_queue.push(std::move(value));
	}

	bool tryPop(T &out) noexcept
	{
		std::lock_guard<std::mutex> lock(_mutex);
		if (_queue.empty())
		{
			return false;
		}
		out = std::move(_queue.front());
		_queue.pop();
		return true;
	}

	[[nodiscard]]
	std::optional<T> pop() noexcept
	{
		std::lock_guard<std::mutex> lock(_mutex);
		if (_queue.empty())
		{
			return std::nullopt;
		}
		auto out = std::move(_queue.front());
		_queue.pop();
		return out;
	}
};

}// namespace himu::util
#endif//JUDGECORESERVER_SHARED_UTILS_CONCURRENT_QUEUE_HPP