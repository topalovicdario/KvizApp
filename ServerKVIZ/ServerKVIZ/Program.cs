using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using ServerKVIZ.Models;
using ServerKVIZ.Repositoryes;
using ServerKVIZ.Services;
using System.Text;



var builder = WebApplication.CreateBuilder(args);

builder.Logging.SetMinimumLevel(LogLevel.Debug);

var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"];
var jwtIssuer = jwtSettings["Issuer"];

if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("nema kljuca!");
}





builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddSingleton<IMemoryCache>(sp => new MemoryCache(new MemoryCacheOptions()));


builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IQuestionRepository, TriviaQuestionsRepository>();
builder.Services.AddSingleton<IQuestionService,QuestionServices>();
builder.Services.AddSingleton<IPlayerRepository,PlayerRepository>();
builder.Services.AddTransient<IPlayerServices,PlayerServices>();
builder.Services.AddSingleton<IGameSessionService,GameSessionServices>();


builder.Services.AddTransient<IAuthentificatable,PlayerServices>();


//zaboravio sam dodat dublju specifikaciju comita

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR()
    .AddJsonProtocol(options => {
        options.PayloadSerializerOptions.PropertyNamingPolicy = null;
        // Ovo će zadržati PascalCase imena svojstava koja imate u C# klasi
    });
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseWebSockets(); 

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();


app.MapHub<GameHub>("gamehub");

app.MapControllers();
app.Run();
