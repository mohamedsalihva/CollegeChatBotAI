using CollegeChatbotAPI.DTOs;
using Microsoft.Data.SqlClient;

namespace CollegeChatbotAPI.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new Exception("Connection string not found");
        }

        //All courses
        public async Task<List<string>> GetAllCourseNames()
        {
            var courses = new List<string>();

            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = "SELECT CourseName FROM Courses";

            using SqlCommand command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                courses.Add(reader.GetString(0));
            }

            return courses;
        }

        //single course details
        public async Task<Course?> GetCourseFromMessage(string msg)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = @"
        SELECT TOP 1 CourseName, Eligibility, Duration, Fees
        FROM Courses
        WHERE
            LOWER(@msg) LIKE '%' + LOWER(CourseName) + '%'
            OR
            (
                Aliases IS NOT NULL
                AND EXISTS (
                    SELECT 1
                    FROM STRING_SPLIT(Aliases, ',')
                    WHERE LOWER(@msg) LIKE '%' + LOWER(value) + '%'
                )
            );
    ";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@msg", msg);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Course
                {
                    CourseName = reader.GetString(0),
                    Eligibility = reader.GetString(1),
                    Duration = reader.GetString(2),
                    Fees = reader.GetString(3)
                };
            }

            return null;
        }

        
        //Matched FAQs
        public async Task<List<(int FaqId, string Category)>> GetMatchedFaqs(string msg)
        {
            var result = new List<(int, string)>();

            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = @"
                SELECT DISTINCT f.FaqId, k.Category
                FROM FAQ f
                JOIN FAQKeywords k ON f.FaqId = k.FaqId
                WHERE LOWER(@msg) LIKE '%' + LOWER(k.Keyword) + '%';
            ";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@msg", msg);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add((reader.GetInt32(0), reader.GetString(1)));
            }

            return result;
        }

        public async Task<string?> GetFaqAnswerByFaqId(int faqId)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = "SELECT Answer FROM FAQ WHERE FaqId = @faqId";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@faqId", faqId);

            return (await command.ExecuteScalarAsync())?.ToString();
        }
    }
}
