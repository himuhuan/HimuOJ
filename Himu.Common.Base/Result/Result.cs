namespace Himu.Common.Base
{
    public enum Result
    {
        Success = 0,
        Error = 1,
        Warning = 2,
        Info = 3
    }

    public readonly struct Result<T>
    {
        public readonly T Value;
        public readonly Result Status;

        public Result(T value, Result status = Result.Success)
        {
            Value = value;
            Status = status;
        }
    }
}