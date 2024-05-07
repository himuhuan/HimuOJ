namespace Himu.EntityFramework.Core.Tools
{
    public enum StorageUnitEnum
    {
        Byte, Kilobyte, Megabyte, Gigabyte
    }

    public record StorageUnitSize(long ByteSize)
    {
        public StorageUnitSize(long size, StorageUnitEnum unitEnum)
            : this(size)
        {
            ByteSize = unitEnum switch
            {
                StorageUnitEnum.Byte => size,
                StorageUnitEnum.Kilobyte => size * 1024,
                StorageUnitEnum.Megabyte => size * 1024 * 1024,
                StorageUnitEnum.Gigabyte => size * 1024 * 1024,
                _ => throw new ArgumentOutOfRangeException(nameof(unitEnum), unitEnum, "Not support unitEnum")
            };
        }
    }
}