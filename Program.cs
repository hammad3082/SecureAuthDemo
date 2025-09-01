using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SecureAuthDemo.Data;
using SecureAuthDemo.Repositories;
using SecureAuthDemo.Services;
using Serilog;
using System;

var builder = WebApplication.CreateBuilder(args);

// Configure Logging (Serilog)
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();

// Authorization (placeholder)
//builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// DI: repositories & services (simple)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, LocalAuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();       // Serve OpenAPI JSON
    //app.UseSwaggerUI();     // Serve interactive Swagger UI
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = ""; // root
    });     // Serve interactive Swagger UI
}

app.UseRouting();

// Logs HTTP requests
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", time = DateTime.UtcNow }));
app.MapGet("test", () => "Hello there, This is working");
app.MapControllers();

app.Run();

