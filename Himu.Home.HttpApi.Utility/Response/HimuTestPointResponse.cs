namespace Himu.HttpApi.Utility
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
