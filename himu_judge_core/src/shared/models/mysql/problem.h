#ifndef HIMU_PROBLEM_H
#define HIMU_PROBLEM_H

#include <sqlpp11/char_sequence.h>
#include <sqlpp11/data_types.h>
#include <sqlpp11/table.h>

namespace himu::sql_models
{
namespace Problemset_
{
struct Id
{
	struct _alias_t
	{
		static constexpr const char _literal[] = "Id";
		using _name_t = sqlpp::make_char_sequence<sizeof(_literal), _literal>;
		template<typename T>
		struct _member_t
		{
			T Id;
			T &operator()()
			{
				return Id;
			}
			const T &operator()() const
			{
				return Id;
			}
		};
	};
	using _traits = sqlpp::make_traits<
		sqlpp::integer, sqlpp::tag::must_not_insert, sqlpp::tag::must_not_update>;
};
struct DetailCode
{
	struct _alias_t
	{
		static constexpr const char _literal[] = "Detail_Code";
		using _name_t = sqlpp::make_char_sequence<sizeof(_literal), _literal>;
		template<typename T>
		struct _member_t
		{
			T DetailCode;
			T &operator()()
			{
				return DetailCode;
			}
			const T &operator()() const
			{
				return DetailCode;
			}
		};
	};
	using _traits = sqlpp::make_traits<sqlpp::text, sqlpp::tag::can_be_null>;
};
struct DetailTitle
{
	struct _alias_t
	{
		static constexpr const char _literal[] = "Detail_Title";
		using _name_t = sqlpp::make_char_sequence<sizeof(_literal), _literal>;
		template<typename T>
		struct _member_t
		{
			T DetailTitle;
			T &operator()()
			{
				return DetailTitle;
			}
			const T &operator()() const
			{
				return DetailTitle;
			}
		};
	};
	using _traits = sqlpp::make_traits<sqlpp::text, sqlpp::tag::can_be_null>;
};
struct DetailContent
{
	struct _alias_t
	{
		static constexpr const char _literal[] = "Detail_Content";
		using _name_t = sqlpp::make_char_sequence<sizeof(_literal), _literal>;
		template<typename T>
		struct _member_t
		{
			T DetailContent;
			T &operator()()
			{
				return DetailContent;
			}
			const T &operator()() const
			{
				return DetailContent;
			}
		};
	};
	using _traits = sqlpp::make_traits<sqlpp::text, sqlpp::tag::can_be_null>;
};
struct DetailMaxMemoryLimitByte
{
	struct _alias_t
	{
		static constexpr const char _literal[] = "Detail_MaxMemoryLimitByte";
		using _name_t = sqlpp::make_char_sequence<sizeof(_literal), _literal>;
		template<typename T>
		struct _member_t
		{
			T DetailMaxMemoryLimitByte;
			T &operator()()
			{
				return DetailMaxMemoryLimitByte;
			}
			const T &operator()() const
			{
				return DetailMaxMemoryLimitByte;
			}
		};
	};
	using _traits = sqlpp::make_traits<sqlpp::integer, sqlpp::tag::require_insert>;
};
struct DetailMaxExecuteTimeLimit
{
	struct _alias_t
	{
		static constexpr const char _literal[] = "Detail_MaxExecuteTimeLimit";
		using _name_t = sqlpp::make_char_sequence<sizeof(_literal), _literal>;
		template<typename T>
		struct _member_t
		{
			T DetailMaxExecuteTimeLimit;
			T &operator()()
			{
				return DetailMaxExecuteTimeLimit;
			}
			const T &operator()() const
			{
				return DetailMaxExecuteTimeLimit;
			}
		};
	};
	using _traits = sqlpp::make_traits<sqlpp::integer, sqlpp::tag::require_insert>;
};
struct ContestId
{
	struct _alias_t
	{
		static constexpr const char _literal[] = "ContestId";
		using _name_t = sqlpp::make_char_sequence<sizeof(_literal), _literal>;
		template<typename T>
		struct _member_t
		{
			T ContestId;
			T &operator()()
			{
				return ContestId;
			}
			const T &operator()() const
			{
				return ContestId;
			}
		};
	};
	using _traits = sqlpp::make_traits<sqlpp::integer, sqlpp::tag::require_insert>;
};
struct DetailAllowDownloadAnswer
{
	struct _alias_t
	{
		static constexpr const char _literal[] = "Detail_AllowDownloadAnswer";
		using _name_t = sqlpp::make_char_sequence<sizeof(_literal), _literal>;
		template<typename T>
		struct _member_t
		{
			T DetailAllowDownloadAnswer;
			T &operator()()
			{
				return DetailAllowDownloadAnswer;
			}
			const T &operator()() const
			{
				return DetailAllowDownloadAnswer;
			}
		};
	};
	using _traits = sqlpp::make_traits<sqlpp::integer>;
};
struct DetailAllowDownloadInput
{
	struct _alias_t
	{
		static constexpr const char _literal[] = "Detail_AllowDownloadInput";
		using _name_t = sqlpp::make_char_sequence<sizeof(_literal), _literal>;
		template<typename T>
		struct _member_t
		{
			T DetailAllowDownloadInput;
			T &operator()()
			{
				return DetailAllowDownloadInput;
			}
			const T &operator()() const
			{
				return DetailAllowDownloadInput;
			}
		};
	};
	using _traits = sqlpp::make_traits<sqlpp::integer>;
};
struct DistributorId
{
	struct _alias_t
	{
		static constexpr const char _literal[] = "DistributorId";
		using _name_t = sqlpp::make_char_sequence<sizeof(_literal), _literal>;
		template<typename T>
		struct _member_t
		{
			T DistributorId;
			T &operator()()
			{
				return DistributorId;
			}
			const T &operator()() const
			{
				return DistributorId;
			}
		};
	};
	using _traits = sqlpp::make_traits<sqlpp::integer>;
};
}// namespace Problemset_

struct Problemset
	: sqlpp::table_t<
		  Problemset, Problemset_::Id, Problemset_::DetailCode, Problemset_::DetailTitle,
		  Problemset_::DetailContent, Problemset_::DetailMaxMemoryLimitByte,
		  Problemset_::DetailMaxExecuteTimeLimit, Problemset_::ContestId,
		  Problemset_::DetailAllowDownloadAnswer, Problemset_::DetailAllowDownloadInput,
		  Problemset_::DistributorId>
{
	struct _alias_t
	{
		static constexpr const char _literal[] = "problemset";
		using _name_t = sqlpp::make_char_sequence<sizeof(_literal), _literal>;
		template<typename T>
		struct _member_t
		{
			T problemset;
			T &operator()()
			{
				return problemset;
			}
			const T &operator()() const
			{
				return problemset;
			}
		};
	};
};
}// namespace himu
#endif
