namespace Himu.HttpApi.Utility
{
    public class HimuApiResponse
    {
        public HimuApiResponseCode Code { get; set; } = HimuApiResponseCode.Succeed;

        public string Message { get; set; } = "Success";

        public bool IsSuccess() => Code == HimuApiResponseCode.Succeed;

        public HimuApiResponse Success(string message)
        {
            Code = HimuApiResponseCode.Succeed;
            Message = message;
            return this;
        }

        public HimuApiResponse Failed(string message, HimuApiResponseCode code = HimuApiResponseCode.BadRequest)
        {
            Code = code;
            Message = message;
            return this;
        }

        public HimuApiResponse Failed(Exception exception)
        {
            Code = HimuApiResponseCode.BadRequest;
            Message = exception.ToString();
            return this;
        }

        public HimuApiResponse Assert(bool condition, string message)
        {
            if (condition) Success("okay");
            else Failed(message);
            return this;
        }

        public static HimuApiResponse SuccessResponse { get; } = new HimuApiResponse();
    }

    public class HimuApiResponse<T> : HimuApiResponse
        where T : notnull
    {
        public T Value { get; set; } = default!;
    }
}