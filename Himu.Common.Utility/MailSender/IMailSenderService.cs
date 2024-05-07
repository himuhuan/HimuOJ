namespace Himu.Common.Service
{
    public interface IMailSenderService
    {
        public bool Send(string mailTo, string subject, string body, bool useHtml);
    }
}