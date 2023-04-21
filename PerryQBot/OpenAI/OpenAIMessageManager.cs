using NUglify.JavaScript.Syntax;
using PerryQBot.EntityFrameworkCore.Entities;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace PerryQBot.OpenAI;

public class OpenAIMessageManager : IOpenAIMessageManager, ITransientDependency
{
    public IRepository<User> UserRepository { get; set; }

    [UnitOfWork]
    public virtual async Task<List<string>> BuildUserRequestMessagesAsync(string senderId, string message)
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