using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddAllFeatures();
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
