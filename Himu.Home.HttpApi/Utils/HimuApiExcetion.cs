using Himu.Home.HttpApi.Response;

namespace Himu.Home.HttpApi.Utils
{
    public class HimuApiException : Exception
    {
        public HimuApiResponseCode Code { get; }

        public HimuApiException(HimuApiResponseCode code, string message) : base(message)
        {
            Code = code;
        }

        public HimuApiException(HimuApiResponseCode code, string message, Exception innerException) 
            : base(message, innerException)
        {
            Code = code;
        }
    }
}
