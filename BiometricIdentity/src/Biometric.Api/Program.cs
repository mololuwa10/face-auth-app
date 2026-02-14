using Biometric.Application.Interfaces;
using Biometric.Infrastructure.Persistence;
using Biometric.Infrastructure.Repositories;
using Biometric.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. DATABASE: Register Neon/Postgres with Vector Support
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions.UseVector()
    )
);

// 2. AI CLIENT: Register the HTTP client to talk to your Python AI
builder.Services.AddHttpClient<IFaceAiService, FaceAiClient>(client =>
{
    // Pulls from User Secrets: "AiService:BaseUrl": "http://localhost:8000/"
    client.BaseAddress = new Uri(
        builder.Configuration["AiService:BaseUrl"] ?? "http://localhost:8000/"
    );
});

// 3. DEPENDENCY INJECTION: Link Interfaces to Implementations
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<EnrollmentService>();

// 4. API INFRASTRUCTURE
builder.Services.AddControllers(); // Required for AuthController
builder.Services.AddOpenApi(); // Keeps your Swagger/OpenApi working

var app = builder.Build();

// 5. MIDDLEWARE PIPELINE
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers(); // This maps your [Route("api/[controller]")] attributes

app.Run();
