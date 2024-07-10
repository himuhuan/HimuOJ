using Himu.EntityFramework.Core;
using Himu.EntityFramework.Core.Entity;
using Himu.Home.HttpApi.Hubs;
using Himu.HttpApi.Utility.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Himu.Home.HttpApi.Controllers
{
    [Route("api/chatsessions")]
    [ApiController]
    public class ChatSessionController : ControllerBase
    {
        private readonly HimuMySqlContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatSessionController(HimuMySqlContext context, IHubContext<ChatHub> hub)
        {
            _context = context;
            _hubContext = hub;
        }

        [HttpGet("{sessionId}/messages")]
        public async Task<ActionResult> GetSessionMessage(Guid sessionId)
        {
            List<ChatMessage> messages = await _context.ChatMessages
                .Where(c => c.SessionId == sessionId)
                .ToListAsync();
            return Ok(messages);
        }

        [HttpPost("{sessionId}/messages")]
        [Authorize]
        public async Task<ActionResult> PostSessionMessage(Guid sessionId, [FromBody] PostSessionMessageRequest request)
        {
            ChatMessage message = new()
            {
                SessionId = sessionId,
                SenderId = request.SenderId,
                Value = request.Message,
                SendTime = DateTime.Now
            };

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();
            // Send a message to all clients to refresh the chat message
            await _hubContext.Clients.All.SendAsync("RefreshMessage");
            return Ok();
        }
    }
}
