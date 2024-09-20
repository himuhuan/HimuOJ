using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Himu.EntityFramework.Core.Tools
{
    public class TimespanMillisecondsConverter : ValueConverter<TimeSpan, long>
    {
        public TimespanMillisecondsConverter()
            : base(v => (long) v.TotalMilliseconds, v => TimeSpan.FromMilliseconds(v))
        {
        }
    }
}