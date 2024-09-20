using Himu.Home.HttpApi.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Himu.Home.HttpApi.Response
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
            if (exception is HimuApiException apiException)
            {
                Failed(apiException.Message, apiException.Code);
            }
            else
            {
                Failed(exception.Message, HimuApiResponseCode.UnexpectedError);
            }
            return this;
        }

        public HimuApiResponse Assert(bool condition, string message)
        {
            if (condition) Success("okay");
            else Failed(message);
            return this;
        }

        public static HimuApiResponse SuccessResponse { get; } = new HimuApiResponse();

        public ActionResult ActionResult()
        {
            return Code switch
            {
                HimuApiResponseCode.Succeed => new OkObjectResult(this),
                HimuApiResponseCode.ResourceNotExist => new NotFoundObjectResult(this),
                HimuApiResponseCode.BadAuthentication => new UnauthorizedObjectResult(this),
                HimuApiResponseCode.UnexpectedError => new ObjectResult(this) { StatusCode = 500 },
                _ => new BadRequestObjectResult(this)
            };
        }
    }

    public class HimuApiResponse<T> : HimuApiResponse
        where T : notnull
    {
        public T Value { get; set; } = default!;
    }
}