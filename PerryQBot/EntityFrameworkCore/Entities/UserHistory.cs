using Volo.Abp.Domain.Entities;

namespace PerryQBot.EntityFrameworkCore.Entities
{
    public class UserHistory : Entity<long>
    {
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
    }
}