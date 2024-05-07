using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Himu.EntityFramework.Core.Entity
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ExecutionStatus
    {
        PENDING,                    // PENDING
        RUNNING,                    // RUNNING
        ACCEPTED,                   // AC
        WRONG_ANSWER,               // WA
        COMPILE_ERROR,             // CE
        TIME_LIMIT_EXCEEDED,         // TLE
        RUNTIME_ERROR,              // RE
        MEMORY_LIMIT_EXCEEDED,        // MLE
        INTERNAL_ERROR              // ITE
    }
}