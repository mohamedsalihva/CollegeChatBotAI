using CollegeChatbotAPI.DTOs;

namespace CollegeChatbotAPI.Services
{
    public class ChatService
    {
        private readonly MessageService _messageService;
        private readonly CourseService _courseService;
        private readonly FaqService _faqService;
        private readonly AiFallbackService _aiFallbackService;
        private readonly ChatLogService _chatLogService;

        public ChatService(
            MessageService messageService,
            CourseService courseService,
            FaqService faqService,
            AiFallbackService aiFallbackService,
            ChatLogService chatLogService)
        {
            _messageService = messageService;
            _courseService = courseService;
            _faqService = faqService;
            _aiFallbackService = aiFallbackService;
            _chatLogService = chatLogService;
        }

        public async Task<ChatResponse> GetResponse(string userMessage)
        {
            ChatResponse response;

            if (string.IsNullOrWhiteSpace(userMessage))
            {
                response = new ChatResponse
                {
                    Answer = "Please enter a valid question.",
                    Source = "SYSTEM"
                };
            }
            else
            {
                string msg = _messageService.Normalize(userMessage);

                response =
                    await _courseService.HandleCourseQuery(msg)
                    ?? await _faqService.HandleFaqQuery(msg)
                    ?? await _aiFallbackService.GetFallbackResponse(userMessage);
            }

            
            await _chatLogService.SaveAsync(
                userMessage,
                response.Answer,
                response.Source
            );

            return response;
        }
    }
}
