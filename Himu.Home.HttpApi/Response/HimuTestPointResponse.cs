namespace Himu.Home.HttpApi.Response
{
    public class HimuTestPointResponseValue
    {
        public string InputFileContent { get; set; } = string.Empty;

        public string AnswerFileContent { get; set; } = string.Empty;
    }

    public class HimuTestPointResponse : HimuApiResponse<HimuTestPointResponseValue>
    {
    }
}