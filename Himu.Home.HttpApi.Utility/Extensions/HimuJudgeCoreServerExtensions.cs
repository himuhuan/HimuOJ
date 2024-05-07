using System.Text.Json;
using System.Text.Json.Nodes;

namespace Himu.HttpApi.Utility.Extensions
{
    public static class HimuJudgeCoreServerExtensions
    {
        public static async Task<T?> DeserializeAs<T>(this JsonObject obj)
        {
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            obj.WriteTo(writer);
            await writer.FlushAsync();
            T? result = JsonSerializer.Deserialize<T>(stream.ToArray());
            return result;
        }

        public static async Task<IEnumerable<T>> DeserializeAs<T>(this JsonArray array)
        {
            List<T> result = new();
            foreach (var item in array)
            {
                T? obj = await item!.AsObject().DeserializeAs<T>() ?? throw new JsonException();
                result.Add(obj);
            }
            return result;
        }
    }
}
