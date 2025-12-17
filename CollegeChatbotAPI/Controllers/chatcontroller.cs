using Microsoft.AspNetCore.Mvc;
using CollegeChatbotAPI.Services;
using CollegeChatbotAPI.DTOs;

namespace CollegeChatbotAPI.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly ChatService _chatService;

        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
        {
            var reply = await _chatService.GetResponse(request.Message);
            return Ok(new { response = reply });
        }
    }
}
