using Microsoft.EntityFrameworkCore.ValueGeneration;
using Yitter.IdGenerator;

namespace Himu.EntityFramework.Core.Tools;

public static class SnowflakeIdProvider
{
    private static readonly SnowflakeIdGenerator ProviderInstance = new();

    public static void ConfigureGeneratorOptions(IdGeneratorOptions options)
    {
        YitIdHelper.SetIdGenerator(options);
    }

    public static ValueGenerator<long> GetIdGenerator()
    {
        return ProviderInstance;
    }
}