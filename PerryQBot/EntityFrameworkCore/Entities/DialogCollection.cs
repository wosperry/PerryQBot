using Volo.Abp.Domain.Entities;

namespace PerryQBot.EntityFrameworkCore.Entities;

public class DialogCollection : Entity<long>
{
    public string UserName { get; set; }
    public string UserQQ { get; set; }
    public string Message { get; set; }
    public string QuoteMessage { get; set; }
    public DateTime DateTime { get; set; }
}