﻿using PerryQBot.Commands;
using Volo.Abp.DependencyInjection;

namespace PerryQBot.CommandHandlers;

[Command("代码")]
[ExposeServices(typeof(ICommandHandler))]
public class CodeCommandHandler : CommandHandlerBase
{
    public override async Task ExecuteAsync(CommandContext context)
    {
        ResponseMessage = """
                我很乐意为您介绍项目PerryQBot。

                PerryQBot是一个使用Mirai.Net类库对接Mirai的QQ机器人，可以自动处理QQ消息并回复。该项目旨在管理QQ用户或群聊中@机器人的人的消息，每个用户都有一个预设，默认为空。
                聊天中可以保持数条历史记录以实现连续对话，智能对话是通过 GPT-3.5-turbo 的接口实现的。
                该项目已在GitHub上公开，同时也在Gitee有开放的仓库，开源协议为AGPL3.0。如果您希望查看或贡献该项目，请点击以下链接访问：
                https://github.com/wosperry/PerryQBot
                感谢您的兴趣和支持！
                """;
        await Task.CompletedTask;
    }
}