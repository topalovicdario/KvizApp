using ServerKVIZ.Models;
using ServerKVIZ.Services;

var builder = WebApplication.CreateBuilder(args);

// Register IHttpClientFactory
builder.Services.AddHttpClient();

// Register IGetQuestions service with TriviaQuestions (make it Transient)
builder.Services.AddTransient<IGetQuestions, TriviaQuestions>();

// Register DataBaseQuestion as transient (questions are cached)
builder.Services.AddTransient<DataBaseQuestion>();
builder.Services.AddTransient<GameSession>();
// Register GameSession as transient (new instance per request)


// Register IMemoryCache
builder.Services.AddMemoryCache();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
