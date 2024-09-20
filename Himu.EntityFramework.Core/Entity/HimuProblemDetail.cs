using Himu.EntityFramework.Core.Tools;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Himu.EntityFramework.Core.Entity
{
    [Owned]
    public class HimuProblemDetail
    {
        public string Code { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public long MaxMemoryLimitByte { get; set; } = 0;

        [JsonConverter(typeof(TimeSpanMsJsonConverter))]
        public TimeSpan MaxExecuteTimeLimit { get; set; }

        public bool AllowDownloadInput { get; set; }

        public bool AllowDownloadAnswer { get; set; }
    }
}