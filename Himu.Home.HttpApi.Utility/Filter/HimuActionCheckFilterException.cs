namespace Himu.HttpApi.Utility
{
    /// <summary>
    /// Exception thrown when the HTTP API Action 
    /// does not have the corresponding parameter specified by <see cref="HimuActionCheckAttribute"/>.
    /// </summary>
    public class HimuActionCheckFilterException : Exception
    {
        public HimuActionCheckFilterException()
        {

        }

        public HimuActionCheckFilterException(string message) : base(message)
        {

        }

        public HimuActionCheckFilterException(string message, Exception innerException) 
            : base(message, innerException)
        {

        }
    }
}
