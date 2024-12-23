## HimuOJ: An Online Judge implementation

HimuOJ is an Online Judge implementation base on .NET Core WebAPI + Vue + C++ Axios.

> [!CAUTION]
> This project is still under development and some features are not complete.

此仓库下的 HimuOJ 是基于 .NET 6 + MYSQL + Vue 单体WEB程序。此单体应用已不再更新、另请参阅 此应用的微服务版本 HimuOJOnContainers。相较而言：
1. HimuOJOnContainers 是基于 .NET 8 + PGSQL 的，并且数据库模型设计不同
2. HimuOJOnContainers 引入 事务总线与Worker 在 .NET 服务内 调用本地代码 （参见 仓库Sandbox) 而不是实现 C++ 服务器完成评测。
3. HimuOJOnContainers 是基于 第三方 IDP 的，而HimuOJ 是人工实现的 JWT 鉴权。

The HimuOJ under this repository is a monolithic web application based on .NET 6 + MySQL + Vue. This monolithic application is no longer being updated; please refer to the microservices version of this application, HimuOJOnContainers. In comparison:

1. HimuOJOnContainers is based on .NET 8 + PGSQL, and its database model design is different.
2. HimuOJOnContainers introduces a transaction bus and worker that invoke local code within the .NET service (refer to the Sandbox repository), instead of implementing a C++ server to perform the evaluation.
3. HimuOJOnContainers is based on a third-party IDP, whereas HimuOJ implements JWT authentication manually.
