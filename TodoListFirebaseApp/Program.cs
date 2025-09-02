using Microsoft.AspNetCore.Builder; // Correct namespace for WebApplication
using Microsoft.Extensions.DependencyInjection; // Correct namespace for services
using Microsoft.Extensions.Hosting; // Correct namespace for environment checks
using TodoListFirebaseApp.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<FirebaseService>();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();
app.Run();
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
