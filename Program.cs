using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SecureAuthDemo.Configuration;
using SecureAuthDemo.Data;
using SecureAuthDemo.Extensions;
using SecureAuthDemo.Middleware;
using SecureAuthDemo.Repositories;
using SecureAuthDemo.Services;
using SecureAuthDemo.Services.Implimentations;
using SecureAuthDemo.Services.Interfaces;
using Serilog;
using System;
using System.Text;
using System.Text.Json.Serialization;

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
//builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer(); 
//builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {your JWT token}'"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var jwtSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSection);
var jwtSettings = jwtSection.Get<JwtSettings>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Key)),
        ValidateIssuer = true,
        ValidateAudience = true,

        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true
    };
});

builder.Services.Configure<GoogleAuthSettings>(
    builder.Configuration.GetSection("GoogleAuthSettings"));

builder.Services.Configure<AwsCognitoSettings>(
    builder.Configuration.GetSection("AwsCognitoSettings"));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("Role", "Admin"));
});

builder.Services.AddCacheServices(builder.Configuration);

//builder.Services.AddRedis(builder.Configuration);

builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// DI: repositories & services (simple)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, LocalAuthService>();
builder.Services.AddScoped<ExternalAuthFlow>();
builder.Services.AddScoped<IStateStore, StateStore>();

builder.Services.AddTransient<GoogleAuthService>();
builder.Services.AddTransient<CognitoAuthService>();
builder.Services.AddSingleton<AuthServiceFactory>();
//builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();
//builder.Services.AddScoped<ICognitoAuthService, CognitoAuthService>();  

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.UseRouting();
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

        //Inject Google Login button dynamically
        c.HeadContent = @"
        <script>
        window.addEventListener('load', () => {
            const tryAddButton = () => {
                const topbar = document.querySelector('.swagger-ui .topbar');
                if (!topbar) {
                    setTimeout(tryAddButton, 500);
                    return;
                }

                if (!document.getElementById('google-login-btn')) {
                    const btn = document.createElement('button');
                    btn.id = 'google-login-btn';
                    btn.innerText = 'Login with Google';
                    btn.style.marginLeft = '10px';
                    btn.style.background = '#4285F4';
                    btn.style.color = 'white';
                    btn.style.border = 'none';
                    btn.style.padding = '8px 16px';
                    btn.style.borderRadius = '5px';
                    btn.style.cursor = 'pointer';
                    btn.onclick = async () => {
                        try {
                            const response = await fetch('/api/auth/google/login');
                            const data = await response.json();
                            if (data.loginUrl) {
                                window.open(data.loginUrl, '_blank');
                            } else {
                                alert('Login URL not found.');
                            }
                        } catch (err) {
                            alert('Failed to fetch Google login URL.');
                            console.error(err);
                        }
                    };
                    topbar.appendChild(btn);
                }
            };

            tryAddButton();
        });
        </script>";
        //btn.onclick = () => window.location.href = '/api/auth/external/login';

    });     // Serve interactive Swagger UI
}

//app.UseRouting();

// Logs HTTP requests
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", time = DateTime.UtcNow }));
//app.MapGet("test", () => "Hello there, This is working");
app.MapControllers();

app.Run();

