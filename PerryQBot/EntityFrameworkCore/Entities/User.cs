using Volo.Abp.Domain.Entities;

namespace PerryQBot.EntityFrameworkCore.Entities
{
    public class User : Entity<long>
    {
        public string QQ { get; set; }
        public string Preset { get; set; }
        public string QQNickName { get; internal set; }
    }
}