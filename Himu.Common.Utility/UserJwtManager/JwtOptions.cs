namespace Himu.Common.Service
{
    public class JwtOptions
    {
        public string SecretToken { get; set; } = null!;
        public int AccessExpireSeconds { get; set; }
    }
}