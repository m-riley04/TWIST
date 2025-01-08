using System.Text.Json.Serialization;
using System.Text.Json;
using Newtonsoft.Json;
using TWISTServer.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        /// TODO: Make this more secure/not only localhost
        builder.WithOrigins
            (
                "http://localhost:7026", 
                "https://localhost:7026", 
                "http://localhost:5173", 
                "https://localhost:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// JSON Serializer
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.SnakeCaseLower);

// Add SignalR
builder.Services.AddSignalR();

// BUILD APP
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Uses
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

// Mappings
app.MapControllers();
app.MapFallbackToFile("/index.html");
app.MapHub<ChatHub>("/hub");

// Run the app
app.Run();
