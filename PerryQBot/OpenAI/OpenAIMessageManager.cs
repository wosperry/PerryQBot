using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PerryQBot.EntityFrameworkCore.Entities;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace PerryQBot.OpenAI;

public class OpenAIMessageManager : IOpenAIMessageManager, ITransientDependency
{
    public IRepository<User> UserRepository { get; set; }
    public ILogger<OpenAIMessageManager> Logger { get; set; }
    public IOptions<MiraiBotOptions> BotOptions { get; set; }

    [UnitOfWork]
    public virtual async Task<List<string>> BuildUserRequestMessagesAsync(string senderId, string senderName, string message)
    {
        var result = new List<string>();
        var user = await (await UserRepository.WithDetailsAsync(x => x.History))
            .FirstOrDefaultAsync(t => t.QQ == senderId);

        if (user is not null)
        {
            if (!string.IsNullOrWhiteSpace(senderName))
            {
                result.Add($"我QQ昵称是{senderName}，请以{senderName}称呼我。");
            }
            // 添加预设
            if (!string.IsNullOrWhiteSpace(user.Preset))
            {
                result.Add(user.Preset);
            }
            // 添加历史记录
            if (user.History.Count > 0)
            {
                foreach (var his in user.History.OrderBy(x => x.DateTime))
                {
                    result.Add(his.Message);
                }
            }

            user.History.Add(new UserHistory { Message = message, DateTime = DateTime.Now });
            if (user.History.Count > BotOptions.Value.MaxHistory)
            {
                user.History.Remove(user.History.OrderBy(x => x.DateTime).First());
            }
            await UserRepository.UpdateAsync(user);
        }
        else
        {
            await UserRepository.InsertAsync(new User
            {
                QQ = senderId,
                QQNickName = senderName,
                History = new List<UserHistory>() { new UserHistory { Message = message, DateTime = DateTime.Now } },
                Preset = ""
            }, true);
        }
        // 添加当前请求
        result.Add(message);

        Logger.LogInformation("请求参数：");
        Logger.LogInformation(JsonConvert.SerializeObject(result));
        return result;
    }
}