namespace Himu.Common.Service
{
    /// <summary>
    /// Configuration for judge core client to connect judge server
    /// </summary>
    public class JudgeCoreConfiguration
    {
        public string ServerAddress { get; set; } = null!;
        
        public int Port {get; set;}

        public int ConnectionTimeout { get; set; }
    }
}
