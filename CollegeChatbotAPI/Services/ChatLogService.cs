using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CollegeChatbotAPI.Models;

namespace CollegeChatbotAPI.Services
{
    public class ChatLogService
    {
        private readonly string _connectionString;

        public ChatLogService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new Exception("Connection string not found");
        }

        // ✅ SAVE CHAT LOG
        public async Task SaveAsync(string userMessage, string botResponse, string source)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = @"
                INSERT INTO ChatLogs (UserMessage, BotResponse, Source, CreatedAt)
                VALUES (@userMessage, @botResponse, @source, @createdAt);
            ";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@userMessage", userMessage);
            command.Parameters.AddWithValue("@botResponse", botResponse);
            command.Parameters.AddWithValue("@source", source);
            command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);

            await command.ExecuteNonQueryAsync();
        }

        // ✅ READ CHAT LOGS (ADMIN)
        public async Task<List<ChatLog>> GetAllAsync()
        {
            var logs = new List<ChatLog>();

            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = @"
                SELECT Id, UserMessage, BotResponse, Source, CreatedAt
                FROM ChatLogs
                ORDER BY CreatedAt DESC;
            ";

            using SqlCommand command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                logs.Add(new ChatLog
                {
                    id = reader.GetInt32(0),
                    UserMessage = reader.GetString(1),
                    BotResponse = reader.GetString(2),
                    Source = reader.GetString(3),
                    CreatedAt = reader.GetDateTime(4)
                });
            }

            return logs;
        }
    }
}
