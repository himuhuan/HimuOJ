namespace Himu.HttpApi.Utility.Extensions
{
    public static class HimuApiResponseExtensions
    {
        public static HimuApiResponse Success(this HimuApiResponse response, string message)
        {
            response.Code = HimuApiResponseCode.Succeed;
            response.Message = message;
            return response;
        }

        public static HimuApiResponse Failed(this HimuApiResponse response, string message)
        {
            response.Code = HimuApiResponseCode.BadRequest;
            response.Message = message;
            return response;
        }

        public static HimuApiResponse Failed(this HimuApiResponse response, Exception exception)
        {
            response.Code = HimuApiResponseCode.BadRequest;
            response.Message = exception.ToString();
            return response;
        }

        public static HimuApiResponse
            Assert(this HimuApiResponse response, bool condition, string message)
        {
            if (condition) response.Success("okay ");
            else response.Failed(message);
            return response;
        }
    }
}