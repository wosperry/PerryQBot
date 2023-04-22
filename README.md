# PerryQBot

#### 介绍

使用 `Mirai.Net` 与 `Mirai` 机器人通信，收到消息后调用OpenAI接口，然后回复消息到QQ。

#### 使用的工具

- Serilog.AspNetCore：强大的日志框架，可帮助记录和分析应用程序中的异常。
- Flurl.Http：一个轻量级的 HTTP 客户端库，可帮助快速创建和发送 HTTP 请求。
- Mirai.Net：用于实现对接Mirai项目QQ机器人的工具。项目因此必须为AGPL3.0协议，请勿用于不符合规定的用途
- Volo.Abp.EventBus：一个事件驱动的微服务架构组件，可帮助解耦应用程序中的各个部分。
- Volo.Abp.Autofac：基于 Autofac 的依赖注入库，提供了非常方便的构造函数注入。
- Volo.Abp.AspNetCore：基于 ASP.NET 的模块化框架，可帮助快速开发 Web 应用程序。
- Volo.Abp.BackgroundWorkers：用于从后台运行长时间任务的组件。
- Volo.Abp.BackgroundJobs：可帮助在后台运行计划任务和重复性任务的组件。

#### 使用

1. 请根据说明更改 `appsettings.json` 中的配置。
2. 运行程序时，请注意如果使用 Docker Compose，需要先将 `appsettings.json` 文件存放在正确的位置，并修改 `.yml` 文件中对应的挂载路径。如果不挂载配置文件，可以把docker-compose.yml内的挂载部分删掉。

#### 准备做的东西

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
