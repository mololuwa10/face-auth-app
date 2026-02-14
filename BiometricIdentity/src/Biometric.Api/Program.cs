using Biometric.Application.Interfaces;
using Biometric.Application.Services;
using Biometric.Infrastructure.Persistence;
using Biometric.Infrastructure.Repositories;
using Biometric.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register Neon/Postgres with Vector Support
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions.UseVector()
    )
);

// Register the HTTP client to talk to your Python AI
builder.Services.AddHttpClient<IFaceAiService, FaceAiClient>(client =>
{
    // Pulls from User Secrets: "AiService:BaseUrl": "http://localhost:8000/"
    client.BaseAddress = new Uri(
        builder.Configuration["AiService:BaseUrl"] ?? "http://localhost:8000/"
    );
});

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAllOrigins",
        // "AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
    );
});

// Link Interfaces to Implementations
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<EnrollmentService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// 5. MIDDLEWARE PIPELINE
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
