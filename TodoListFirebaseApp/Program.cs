using Microsoft.AspNetCore.Builder; // Correct namespace for WebApplication
using Microsoft.Extensions.DependencyInjection; // Correct namespace for services
using Microsoft.Extensions.Hosting; // Correct namespace for environment checks
using Microsoft.OpenApi.Models;
using TodoListFirebaseApp.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TodoList Firebase API", Version = "v1" });
});
builder.Services.AddScoped<FirebaseService>();

var app = builder.Build();

// Configure Swagger middleware
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TodoList Firebase API V1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at root URL
    });
//}

app.UseAuthorization();
// Program.cs (after building app)
app.MapGet("/", () => Results.Ok(new { service = "todo-api", ok = true, ts = DateTime.UtcNow }));

// keep your existing endpoints too, e.g.:
app.MapGet("/api/health", () => Results.Ok(new { ok = true }));
app.MapControllers(); 
app.Run();
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
