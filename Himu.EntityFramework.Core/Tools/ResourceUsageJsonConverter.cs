using System.Text.Json;
using System.Text.Json.Serialization;
using Himu.EntityFramework.Core.Entity.Components;

namespace Himu.EntityFramework.Core.Tools
{
    public class ResourceUsageJsonConverter : JsonConverter<ResourceUsage>
    {
        public override ResourceUsage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            long memoryByteUsed = 0;
            TimeSpan span = TimeSpan.Zero;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return new ResourceUsage(memoryByteUsed, span);
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = reader.GetString() ?? string.Empty;
                    reader.Read();
                    switch (propertyName.ToLowerInvariant())
                    {
                        case "memorybyteused":
                            memoryByteUsed = reader.GetInt64();
                            break;
                        case "timeused":
                            long timeMsUsed = reader.GetInt64();
                            span = TimeSpan.FromMilliseconds(timeMsUsed);
                            break;
                        default:
                            throw new JsonException();
                    }
                }
            }
            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer,
            ResourceUsage value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("memoryByteUsed", value.MemoryByteUsed);
            writer.WriteNumber("timeUsed", value.TimeUsed.TotalMilliseconds);
            writer.WriteEndObject();
        }
    }
}
