# PerryQBot

## 介绍

使用了Mirai.Net类库对接Mirai，能够自动处理QQ消息并回复。该项目的主要功能是管理QQ用户或者群聊中@机器人的人的消息，每个用户都有一个预设功能，并且都保留若干条历史记录。通过拼接参数并访问openai api，实现智能的自动回复。

此项目采用AGPL3.0开源协议，任何人都可以自由使用、复制、修改和传播该项目的代码。当您使用该项目及代码的时候，需要遵循AGPL3.0协议的规定，即在您发布基于该项目的代码时，必须发布您的代码，并且将其中的修改也一并开源。此外，您还需要将该项目的许可证和版权信息放在您发布的文档和代码文件中。

## 引用

使用了以下开源组件:

- [Mirai.Net](https://github.com/project-mirai/mirai.net) - 用于编写 QQ 机器人。
- [Serilog.AspNetCore](https://github.com/serilog/serilog-aspnetcore) - 用于记录应用程序日志。
- [Flurl.Http](https://github.com/tmenier/Flurl) - 用于 HTTP 请求。
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
