namespace Himu.Common.Service
{
    public class MailSenderOptions
    {
        public string StmpServer { get; set; } = null!;
        public string SenderMail { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int Port { get; set; }
    }
}