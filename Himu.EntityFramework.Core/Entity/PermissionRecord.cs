using Himu.EntityFramework.Core.Entity.Components;
using System.Text.Json.Serialization;

namespace Himu.EntityFramework.Core.Entity
{
    public class PermissionRecord
    {
        public long Id { get; set; }

        public DateTime Date { get; set; }

        public long UserId { get; set; }
        public HimuHomeUser User { get; set; } = null!;

        public long RoleId { get; set; }
        public HimuHomeRole Role { get; set; } = null!;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PermissionOperation Operation { get; set; }
    }
}
