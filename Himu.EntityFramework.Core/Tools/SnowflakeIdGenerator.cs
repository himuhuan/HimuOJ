using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Yitter.IdGenerator;

namespace Himu.EntityFramework.Core.Tools
{
    public class SnowflakeIdGenerator : ValueGenerator<long>
    {
        public override long Next(EntityEntry entry)
        {
            return YitIdHelper.NextId();
        }

        public override bool GeneratesTemporaryValues => false;
    }
}