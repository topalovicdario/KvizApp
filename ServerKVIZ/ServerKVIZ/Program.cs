using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ServerKVIZ.Models;
using ServerKVIZ.Repositoryes;
using ServerKVIZ.Services;
using System.Text;

//nije implementiran do kraja DI za PlayerServices i QuestionServices, i jos neke sitnice :(

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"];
var jwtIssuer = jwtSettings["Issuer"];

if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("nema kljuca!");
}


builder.Services.AddSignalR();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });


builder.Services.AddHttpClient();
builder.Services.AddTransient<IQuestionRepository, TriviaQuestionsRepository>();
builder.Services.AddTransient<QuestionServices>();
builder.Services.AddTransient<GameSession>();
builder.Services.AddMemoryCache();
builder.Services.AddTransient<PlayerRepository>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseWebSockets(); 

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<GameHub>("/gamehub");
});

app.MapControllers();
app.Run();
