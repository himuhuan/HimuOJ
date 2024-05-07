using System.Text;

namespace Himu.Common.Service.JudgeCoreService
{
    /// <summary>
    /// This utility class is used to maintain socket protocols and handle details
    /// </summary>
    internal static class JudgeCoreProtocol
    {
        public const int HEADER_LENGTH = 8;
        public const int MAX_REQUEST_MESSAGE_LENGTH = 512;

        public static byte[] BuildRequestMessage(long commitId)
        {
            string commitIdStr = commitId.ToString();
            int messageLength = HEADER_LENGTH + commitIdStr.Length;
            if (messageLength > MAX_REQUEST_MESSAGE_LENGTH)
            {
                throw new ArgumentOutOfRangeException(nameof(commitId),
                    "message too long");
            }
            string message = $"{messageLength:D4}{commitIdStr.Length:D4}{commitIdStr}";
            return Encoding.UTF8.GetBytes(message);
        }
    }
}
