using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Sockets;
using System.Text;

namespace Himu.Common.Service.JudgeCoreService
{
    /// <summary>
    /// a client to communicate with judge core server
    /// </summary>
    /// <remarks>
    /// message body: total_length + commit_id_length + commit_id + message_body (optional)
    /// </remarks>
    internal class JudgeSocketService : IJudgeCoreService
    {
        private readonly JudgeCoreConfiguration _configuration;
        private readonly ILogger<JudgeSocketService> _logger;

        public JudgeSocketService(
            IOptions<JudgeCoreConfiguration> configuration,
            ILogger<JudgeSocketService> logger
        )
        {
            _configuration = configuration.Value;
            _logger = logger;
        }

        /// <summary>
        /// Send a request to judge core server to judge a commit
        /// </summary>
        /// <returns> response code from server </returns>
        public async Task<int> RequestJudgeCommitAsync(long commitId)
        {
            var judgeMessage = JudgeCoreProtocol.BuildRequestMessage(commitId);

            CancellationTokenSource cts = new(_configuration.ConnectionTimeout);

            using Socket socket = new(SocketType.Stream, ProtocolType.Tcp);
            await socket.ConnectAsync(_configuration.ServerAddress, _configuration.Port, cts.Token);
            _logger.LogDebug(
                "(commit: {commitId}) connected to core server, sending request",
                commitId);

            int responseCode;
            try
            {
                int messageSend = 0;
                while (messageSend < judgeMessage.Length)
                {
                    messageSend +=
                        await socket.SendAsync(judgeMessage, SocketFlags.None, cts.Token);
                }

                // response from server is a status code
                byte[] response = new byte[sizeof(int)];
                int responseLength =
                    await socket.ReceiveAsync(response, SocketFlags.None, cts.Token);
                
                if (responseLength is 0 or > sizeof(int))
                {
                    _logger.LogError("invaild response from core server");
                    return -1;
                }

                responseCode = int.Parse(Encoding.UTF8.GetString(response));
                _logger.LogDebug(
                    "response from core server: {responseCode}", responseCode);
            }
            catch (SocketException ex)
            {
                _logger.LogError("socket error: {what}", ex.Message);
                return -1;
            }
            catch (TaskCanceledException e)
            {
                _logger.LogError("{src}: operation timeout", e.Source);
                return -2;
            }

            return responseCode;
        }
    }
}