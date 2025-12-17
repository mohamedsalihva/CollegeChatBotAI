// Add this where you configure services
builder.Services.AddHttpClient<ChatService>(client => client.BaseAddress = new Uri("https://api.openai.com/"));