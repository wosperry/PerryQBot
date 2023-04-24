using Volo.Abp.Domain.Entities;

namespace PerryQBot.EntityFrameworkCore.Entities;

public class UserHistory : Entity<long>
{
    public string Message { get; set; }
    public DateTime DateTime { get; set; }
    public User User { get; set; }
    public long UserId { get; set; }
    public string Role { get; set; }
}