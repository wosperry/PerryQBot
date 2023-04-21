using NUglify.JavaScript.Syntax;
using PerryQBot.EntityFrameworkCore.Entities;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace PerryQBot.OpenAI;

public class OpenAIMessageManager : IOpenAIMessageManager, ITransientDependency
{
    public IRepository<User> UserRepository { get; set; }

    public async Task<List<string>> BuildUserRequestMessagesAsync(string senderId, string message)
    {
        var result = new List<string>();
        var user = await UserRepository.FirstOrDefaultAsync(t => t.QQ == senderId);
        // 添加预设
        if (!string.IsNullOrWhiteSpace(user?.Preset))
        {
            result.Add(user.Preset);
        }
        // 添加当前请求
        result.Add(message);
        return result;
    }
}