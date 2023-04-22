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

## 项目结构

代码结构

``` shell
PerryQBot
    ├─Commands
    │  └─Handlers
    ├─EntityFrameworkCore
    │  ├─Configurations
    │  └─Entities
    ├─Migrations
    ├─OpenAI
    │  ├─BackgroundJobs
    │  └─HttpRequests
    ├─Options
    ├─Properties
    └─QQBot
```


## 部署说明

### 使用 Docker Compose

1. 需要安装 Docker 和 Docker Compose。
2. 在 `docker-compose.yml` 文件所在目录下，运行 `docker-compose up -d`。
3. 如果需要修改配置，请先将 `appsettings.json` 文件存放在正确的位置，并修改 `.yml` 文件中对应的挂载路径。如果不挂载配置文件，可以把docker-compose.yml内的挂载部分删掉。
4. 注意appsettings.json内的连接字符串配置的主机地址应该是数据库容器的名称，而不是localhost.
5. 首次运行的时候数据库是不存在的，你需要在项目所在的地方修改配置为真实的连接字符串，并执行EntityFrameworkCore.Tools的Update-Database命令，将数据库结构生成出来。
6. 运行Bot的机器需要可以访问外网。

### `docker-compose.yml`

``` yaml
version: "3.4"

# 定义网络
networks:
  pqcnet:
    driver: bridge

# 定义服务
services: 
  # 数据库
  perrybot_postgresql:
    image: postgres:latest
    # 容器名
    container_name: perrybot_postgresql
    restart: always
    # 映射宿主机端口12345到数据库容器的5432，用于外部使用工具访问数据库查看数据
    ports:
      - "65432:5432"
    # 挂载路径
    volumes:
      - /perry/PerryQBotDB:/var/lib/postgresql/data 
    environment:
      # 这里2个容器同在名为pqcnet的docker网络，直接使用容器名字和5432端口即可访问数据库
      # Bot里使用的连接字符串是appsettings.json里指定的 "Host=perrybot_postgresql;Database=PerryQQBot;User ID=postgres;Password=123456;"
      POSTGRES_USER: "postgres"
      POSTGRES_PASSWORD: "Aa123456."
      POSTGRES_DB: "PerryQQBot"
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      timeout: 5s
      retries: 5
    # 加入到网络 pqcnet
    networks:
      - pqcnet

  # QBot
  perrybot_bot:
    # 容器名
    container_name: perrybot_bot
    build:
      # 指定执行位置
      context: .
      # 指定Dockerfile
      dockerfile: ./PerryQBot/Dockerfile
    # 加入到网络 pqcnet
    networks:
      - pqcnet
    restart: always
    volumes:
      # 这里其实是由于开源项目，我的真实配置放在了 /perry/PerryQBotConfig/appsettings.json
      # 挂载路径下的appsettings.json文件到容器内的/app/appsettings.json使用
      # 这里容器里的/app，是Dockerfile内指定的工作目录，如果需要修改，则需要一起改。
      - /perry/PerryQBotConfig/appsettings.json:/app/appsettings.json

```

### 程序配置介绍

``` json
{
  "MiraiBotOptions": {
    "Host": "your-website.com", // Mirai所在的服务器地址
    "Port": 9010, // Mirai放出来的端口
    "QQ": "12345678", // Mirai登录的QQ
    "AdminQQ": "1111111", // 管理员QQ，接收登录或者超时消息
    "MaxHistory": 4, // 连续对话最大历史条数
    "VerifyKey": "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", // Mirai 设置的http-client的 verifyKey
    "CommandStartChar": "#" // 命令前缀 如 #帮助 $$帮助
  },
  "OpenAiOptions": {
    "CompletionUrl": "https://api.openai.com/v1/chat/completions",
    "Key": "sk-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
  },
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=MyBotDB;User ID=postgres;Password=postgres;"
  }
}
```

1. MiraiBotOptions
   - Host: Mirai所在的服务器地址
   - Port: Mirai放出来的端口
   - QQ: Mirai登录的QQ
   - AdminQQ: 管理员QQ，接收登录或者超时消息
   - MaxHistory: 连续对话最大历史条数
   - VerifyKey: Mirai 设置的http-client的 verifyKey
   - CommandStartChar: 命令前缀 如 #帮助 $$帮助

2. OpenAiOptions
   - CompletionUrl: OpenAI的API地址，不用修改，如果要改，从官方文档查看。
   - Key: OpenAI的Key

3. ConnectionStrings
   - Default: 数据库连接字符串，这里使用的是PostgreSQL，如果要使用其他数据库，需要修改对应的Nuget包，然后修改这里的连接字符串。并且修改程序里所依赖的 `AbpEntityFrameworkCoreXXXModule` 为对应的module