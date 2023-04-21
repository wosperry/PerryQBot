using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NUglify.JavaScript.Syntax;
using PerryQBot.EntityFrameworkCore.Entities;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace PerryQBot.OpenAI;

public class OpenAIMessageManager : IOpenAIMessageManager, ITransientDependency
{
    public IRepository<User> UserRepository { get; set; }
    public ILogger<OpenAIMessageManager> Logger { get; set; }

    [UnitOfWork]
    public virtual async Task<List<string>> BuildUserRequestMessagesAsync(string senderId, string message)
    {
        var result = new List<string>();
        var user = await (await UserRepository.WithDetailsAsync(x => x.History))
            .FirstOrDefaultAsync(t => t.QQ == senderId);

        if (user is not null)
        {
            // 添加预设
            if (!string.IsNullOrWhiteSpace(user.Preset))
            {
                result.Add(user.Preset);
            }
            if (user.History.Count > 0)
            {
                foreach (var his in user.History.OrderBy(x => x.DateTime))
                {
                    result.Add(his.Message);
                }
            }
            if (user.History.Count > 2)
            {
                user.History.Remove(user.History.OrderBy(x => x.DateTime).First());
            }
            user.History.Add(new UserHistory { Message = message, DateTime = DateTime.Now });
            await UserRepository.UpdateAsync(user);
        }
        // 添加当前请求
        result.Add(message);

        Logger.LogInformation("请求参数：");
        Logger.LogInformation(JsonConvert.SerializeObject(result));
        return result;
    }
}