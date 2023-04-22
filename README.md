# PerryQBot

## 介绍

使用了Mirai.Net类库对接Mirai，能够自动处理QQ消息并回复。该项目的主要功能是管理QQ用户或者群聊中@机器人的人的消息，每个用户都有个独一份的预设和历史，可区分对话。
通过拼接参数并访问openai api，实现智能的自动回复。

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
   - [x] 支持修改历史消息条数
   - [ ] 支持修改温度

2. 命令
   - [x] 增加约定，用于控制，需要增加一个命令就增加一个Handler实现类即可
   - [x] 获取命令的消息不需要前缀： 命令、help、帮助、幫助
   - [x] 支持修改命令前缀
   - [x] 支持添加唤醒词
   - [ ] 唤醒词现在是在Handler里添加的，导致handle了一次之后，不会再触发其他命令，如果发送 ai #help 之类的，会被拿去请求openAI。。。累了不想了
   
3. 其他功能
   - [x] 在群里引用某个信息，并收藏
   - [x] 搜索收藏的消息

## 项目结构

代码结构

``` mathematica
PerryQBot                # 项目根目录
├──Commands              #   命令相关文件夹
│  └──Handlers           #       命令处理程序 可在此处参考原有的Handler创建新的命令，重新运行之后即可生效
├──EntityFrameworkCore   #   EFCore
│  ├──Configurations     #       实体配置文件夹
│  └──Entities           #   实体文件夹
├──OpenAI                #   OpenAI
│  ├──BackgroundJobs     #       后台任务执行OpenAI请求、发送QQ消息回复（这个考虑迁到QQBot）
│  └──HttpRequests       #       管理OpenAI请求参数等
├──Options               #   管理选项配置
└──QQBot                 #   QQ信息监听等，如果是添加命令建议直接添加命令Handler实现。
```


## 部署说明

### 使用 Docker Compose

1. 需要安装 Docker 和 Docker Compose。
2. 在 `docker-compose.yml` 文件所在目录下，运行 `docker-compose up -d`。
3. 如果需要修改配置，请先将 `appsettings.json` 文件存放在正确的位置，并修改 `.yml` 文件中对应的挂载路径。如果不挂载配置文件，可以把docker-compose.yml内的挂载部分删掉。
4. 首次运行的时候数据库是不存在的，你需要在项目所在的地方修改配置为真实的连接字符串，并执行EntityFrameworkCore.Tools的Update-Database命令，将数据库结构生成出来。
5. 运行Bot的机器需要可以访问外网。

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
    "Host": "your-website.com",
    "Port": 9010,
    "QQ": "12345678",
    "AdminQQ": "1111111",
    "MaxHistory": 4,
    "VerifyKey": "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
    "CommandPrefix": "#"
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


## 命令

当收到私聊@机器人时，如果聊天开头是以命令开头的，则进入命令处理程序，不走OpenAI请求。

### 命令说明

命令前缀默认为"$$"，如果需要修改，在配置文件的`MiraiBotOptions`下修改`CommandPrefix`的值。
以下是以"#"为前缀的命令说明

1. #帮助
   - 说明：显示帮助信息
2. #预设
   - 参数：预设文本
   - 说明：修改发送者的预设文本，同时清理发送者的历史消息
   - 示例：
      - #预设 你好，我的名字叫Perry。
      - #预设 我想让你充当英英词典，对于给出的英文单词，你要给出其中文意思以及英文解释，此外不要有其他反馈，第一个单词是“Hello"。
3. #清空历史
   - 说明：清空历史记录（不包含预设）
4. #收藏 [额外文本]
   - 参数：额外文本
   - 说明：收藏发送者引用的文本及当前发送的文本
   - 示例：
      - #收藏 你好，我的名字叫Perry。
      - #收藏 我想让你充当英英词典，对于给出的英文单词，你要给出其中文意思以及英文解释，此外不要有其他反馈，第一个单词是“Hello"。
      - [长按引用的前面的消息] #收藏 加上了我自己补充的内容
5. #查询收藏 [条件]
   - 说明：根据参数搜索查询收藏，最多返回展示3条结果
   - 示例：
      - #查询收藏 Perry
      - #查询收藏 MongoDB

### 命令处理程序

1. 位置
   ``` mathematica
   PerryQBot               # 项目根目录
   ├─Commands              #   命令相关文件夹
   │  └─Handlers           #       命令处理程序 可在此处参考原有的Handler创建新的命令，重新运行之后即可生效
   ```
2. 添加自定义命令处理程序
   在1中位置，创建新的类，比如 `GreetingCommandHandler.cs`，要成为一个可被框架使用的命令，还需要满足以下条件：
   - 继承 `CommandHandlerBase` 并重写 `HandleAndResponseAsync` 方法，添加你的逻辑，并返回需要回复用户的消息。
   - 标记 `[Command("[不带前缀的命令字符]")]` 特性
   - 标记 `[ExposeService(typeof(ICommandHandler))]` 特性
      `[ExposeService(typeof(ICommandHandler))]` 可选，如果你认为Abp框架可以正常注入这个类型，则可以不写这个特性。
   
3. 代码示例：
   ``` csharp
   using Volo.Abp.DependencyInjection;
   
   namespace PerryQBot.Commands.Handlers;
   
   [Command("你好")]
   [ExposeServices(typeof(ICommandHandler))]
   public class GreetingCommandHandler : CommandHandlerBase
   {
       public override async Task HandleAndResponseAsync(CommandContext context)
       {
           // 这里为了代码直观，直接等待完成了
           await Task.CompletedTask; 
   
           ResponseMessage = $"你好，{context.SenderName}";
       }
   }
   ```

4. 效果
   ```
   wosperry 17:58:54
   #你好
   
   Mochi 17:58:54
   你好，wosperry
   
   ```

