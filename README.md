# PerryQBot

## 介绍

使用 `Mirai.Net` 与 `Mirai` 机器人通信，收到消息后调用OpenAI接口，然后回复消息到QQ。


## 引用

使用了以下开源组件:

- [Serilog.AspNetCore](https://github.com/serilog/serilog-aspnetcore) - 用于记录应用程序日志。
- [Flurl.Http](https://github.com/tmenier/Flurl) - 用于 HTTP 请求。
- [Mirai.Net](https://github.com/project-mirai/mirai.net) - 用于编写 QQ 机器人。
- [Volo.Abp.AspNetCore.Mvc](https://abp.io/) - ASP.NET Core MVC 集成。
- [Volo.Abp.EventBus](https://abp.io/) - 事件总线。
- [Volo.Abp.Autofac](https://abp.io/) - 依赖注入与控制反转。
- [Volo.Abp.BackgroundWorkers](https://abp.io/) - 后台工作者。
- [Volo.Abp.BackgroundJobs](https://abp.io/) - 后台任务。
- [Volo.Abp.EntityFrameworkCore.PostgreSql](https://abp.io/) - PostgreSQL 数据库集成。
- [Volo.Abp.Uow](https://abp.io/) - 工作单元 (Unit of Work) 设计模式的实现。

## 使用

1. 请根据说明更改 `appsettings.json` 中的配置。
2. 运行程序时，请注意如果使用 Docker Compose，需要先将 `appsettings.json` 文件存放在正确的位置，并修改 `.yml` 文件中对应的挂载路径。如果不挂载配置文件，可以把docker-compose.yml内的挂载部分删掉。

## 准备做的东西

1. 对接GPT
   - [x] 一句一句对话
   - [x] 计划加上可以配置条数的历史消息上文
   - [x] 增加预设和历史信息（按QQ号区分对话）
   - [x] 计划加上群里按发送者QQ号区分历史消息
   - [ ] 支持修改温度
   - [ ] 支持修改历史消息条数

2. 命令
   - [x] 增加约定，用于控制，需要增加一个命令就增加一个Handler实现类即可
   - [x] 获取命令的消息不需要前缀： 命令、help、帮助、幫助
   - [x] 支持修改命令前缀
   - [ ] 支持修改唤醒词
   
3. 其他功能
   - [ ] 在群里引用某个信息，并收藏
   - [ ] 搜索收藏的消息
