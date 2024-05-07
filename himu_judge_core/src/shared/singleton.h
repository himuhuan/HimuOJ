
#ifndef JUDGECORESERVER_SHARED_ISINGLETON_H
#define JUDGECORESERVER_SHARED_ISINGLETON_H

class [[maybe_unused]] ISingleton {
public:
	ISingleton(const ISingleton &) = delete;
	ISingleton &operator=(const ISingleton &) = delete;
	ISingleton(ISingleton &&) = delete;

protected:
	ISingleton() = default;
	virtual ~ISingleton() = default;
};

#endif //JUDGECORESERVER_SHARED_ISINGLETON_H
