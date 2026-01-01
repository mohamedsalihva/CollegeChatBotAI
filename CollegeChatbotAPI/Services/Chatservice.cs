using CollegeChatbotAPI.DTOs;
using CollegeChatbotAPI.Models;

namespace CollegeChatbotAPI.Services
{
    public class ChatService
    {
        private readonly IAIService _aiService;
        private readonly DatabaseService _dbService;

        public ChatService(IAIService aiService, DatabaseService dbService)
        {
            _aiService = aiService;
            _dbService = dbService;
        }

        public async Task<ChatResponse> GetResponse(string userMessage)
        {
            if (string.IsNullOrWhiteSpace(userMessage))
            {
                return new ChatResponse
                {
                    Answer = "Please enter a valid question.",
                    Source = "SYSTEM"
                };
            }

            string msg = userMessage.ToLower().Replace(".", "").Replace(" ", "");

            //all courses

            if (IsAllCoursesQuery(msg))
            {
                var courses = await _dbService.GetAllCourseNames();

                return new ChatResponse
                {
                    Answer = "Available courses are: " + string.Join(", ", courses),
                    Source = "DATABASE"
                };
            }

            // single course

            var course = await _dbService.GetCourseFromMessage(msg);
            if (course != null)
            {
                return BuildCourseResponse(course, msg);
            }


            //faqs

            var matchedFaqs = await _dbService.GetMatchedFaqs(msg);

            if (matchedFaqs.Count == 1)
            {
                var answer = await _dbService.GetFaqAnswerByFaqId(matchedFaqs[0].FaqId);

                return new ChatResponse
                {
                    Answer = answer ?? "Answer not available.",
                    Source = "DATABASE"
                };
            }

            if (matchedFaqs.Count > 1)
            {
                var categories = matchedFaqs.Select(x => x.Category).Distinct();

                return new ChatResponse
                {
                    Answer = "I can help you with " + string.Join(", ", categories) +
                             ". Please ask one topic at a time.",
                    Source = "GUIDANCE"
                };
            }


            //ai fallback

            string prompt = $@"
You are a college enquiry assistant.
Answer only college related questions.

User question:
{userMessage}
";

            var aiAnswer = await _aiService.GetAIResponse(prompt);

            return new ChatResponse
            {
                Answer = aiAnswer,
                Source = "AI"
            };
        }

     

        private bool IsAllCoursesQuery(string msg)
        {
            return msg.Contains("all courses")
                || msg.Contains("available courses")
                || msg.Contains("what courses")
                || msg.Contains("courses offered")
                || msg.Contains("list courses")
                || msg.Equals("courses")
                || msg.Equals("course list");
        }

        private ChatResponse BuildCourseResponse(Course course, string msg)
        {
            var parts = new List<string>();

            bool wantsFees = msg.Contains("fees");
            bool wantsDuration = msg.Contains("duration");
            bool wantsEligibility = msg.Contains("eligibility");

            if (!wantsFees && !wantsDuration && !wantsEligibility)
            {
                parts.Add($"Course: {course.CourseName}");
                parts.Add($"Duration: {course.Duration}");
                parts.Add($"Eligibility: {course.Eligibility}");
                parts.Add($"Fees: {course.Fees}");
            }
            else
            {
                if (wantsFees)
                    parts.Add($"Fees: {course.Fees}");

                if (wantsDuration)
                    parts.Add($"Duration: {course.Duration}");

                if (wantsEligibility)
                    parts.Add($"Eligibility: {course.Eligibility}");
            }

            return new ChatResponse
            {
                Answer = string.Join(". ", parts) + ".",
                Source = "DATABASE"
            };
        }
    }
}
