using CollegeChatbotAPI.DTOs;
using CollegeChatbotAPI.Models;

namespace CollegeChatbotAPI.Services
{
    public class CourseService
    {
        private readonly DatabaseService _dbService;

        public CourseService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public async Task<ChatResponse?> HandleCourseQuery(string msg)
        {
            bool asksFees = msg.Contains("fee") || msg.Contains("fees");
            bool asksDuration = msg.Contains("duration");
            bool asksEligibility = msg.Contains("eligibility");

         
            var course = await _dbService.GetCourseFromMessage(msg);

            if ((asksFees || asksDuration || asksEligibility) && course == null)
            {
                return new ChatResponse
                {
                    Answer = "Please specify the course name (e.g., BCA, MCA, BSc) to get details like fees, duration, or eligibility.",
                    Source = "GUIDANCE"
                };
            }

            
            if (IsAllCoursesQuery(msg))
            {
                var courses = await _dbService.GetAllCourseNames();
                return new ChatResponse
                {
                    Answer = "Available courses are: " + string.Join(", ", courses),
                    Source = "DATABASE"
                };
            }

            
            if (course != null)
            {
                return BuildCourseResponse(course, msg);
            }

           
            return null;
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

            bool wantsFees = msg.Contains("fee") || msg.Contains("fees");
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
                if (wantsFees) parts.Add($"Fees: {course.Fees}");
                if (wantsDuration) parts.Add($"Duration: {course.Duration}");
                if (wantsEligibility) parts.Add($"Eligibility: {course.Eligibility}");
            }

            return new ChatResponse
            {
                Answer = string.Join(". ", parts) + ".",
                Source = "DATABASE"
            };
        }
    }
}
