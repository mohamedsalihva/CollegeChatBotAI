using CollegeChatbotAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHttpClient<OllamaAIService>();

builder.Services.AddScoped<IAIService, OllamaAIService>();

builder.Services.AddScoped<ChatService>();

builder.Services.AddScoped<DatabaseService>();

var app = builder.Build();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
