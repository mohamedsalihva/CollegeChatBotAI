using CollegeChatbotAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CollegeChatbotAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/chatlogs")]
    public class AdminChatLogController : ControllerBase
    {
        private readonly ChatLogService _chatLogService;

        public AdminChatLogController(ChatLogService chatLogService)
        {
            _chatLogService = chatLogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllChatLogs()
        {
            var logs = await _chatLogService.GetAllAsync();
            return Ok(logs);
        }
    }
}
