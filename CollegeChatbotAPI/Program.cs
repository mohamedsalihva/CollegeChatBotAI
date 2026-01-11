using CollegeChatbotAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// HTTP client for Ollama
builder.Services.AddHttpClient<OllamaAIService>();

// AI abstraction
builder.Services.AddScoped<IAIService, OllamaAIService>();

// Refactored services
builder.Services.AddScoped<MessageService>();
builder.Services.AddScoped<CourseService>();
builder.Services.AddScoped<FaqService>();
builder.Services.AddScoped<AiFallbackService>();
builder.Services.AddScoped<ChatLogService>();

// Orchestrator
builder.Services.AddScoped<ChatService>();

// Database
builder.Services.AddScoped<DatabaseService>();

var app = builder.Build();

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
