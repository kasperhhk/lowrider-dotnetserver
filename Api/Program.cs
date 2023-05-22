using System.Text.Json;
using System.Text.Json.Serialization;
using Api.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddChatFeature();
builder.Services.AddSingleton(new JsonSerializerOptions
{
  Converters = { new JsonStringEnumConverter() },
  PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
  PropertyNameCaseInsensitive = true
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapControllers();

app.UseWebSockets();

app.Run();
