using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MYCV.Application.Configuration;
using MYCV.Application.Interfaces;
using MYCV.Application.Services;
using MYCV.Infrastructure.Data;
using MYCV.Infrastructure.Repositories;
using MYCV.Infrastructure.Security;
using MYCV.Infrastructure.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ============================
// DB CONTEXT
// ============================
builder.Services.AddDbContext<MyCvDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ============================
// EMAIL
// ============================
builder.Services.AddSingleton<EmailTemplateService>(sp =>
    new EmailTemplateService(builder.Environment.ContentRootPath));

builder.Services.AddScoped<IEmailService, EmailService>();

// ============================
// REPOSITORIES
// ============================
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserPersonalDetailRepository, UserPersonalDetailRepository>();
builder.Services.AddScoped<IUserEducationRepository, UserEducationRepository>();
builder.Services.AddScoped<IUserExperienceRepository, UserExperienceRepository>();
builder.Services.AddScoped<IUserSkillRepository, UserSkillRepository>();
builder.Services.AddScoped<IUserProjectRepository, UserProjectRepository>();
builder.Services.AddScoped<IUserLanguageRepository, UserLanguageRepository>();
builder.Services.AddScoped<IUserSummaryObjectiveRepository, UserSummaryObjectiveRepository>();
builder.Services.AddScoped<IUserReferenceRepository, UserReferenceRepository>();
builder.Services.AddScoped<IUserSubscriptionRepository, UserSubscriptionRepository>();
builder.Services.AddScoped<ICvTemplateRepository, CvTemplateRepository>();
builder.Services.AddScoped<IUserSelectedTemplateRepository, UserSelectedTemplateRepository>();

// ============================
// SERVICES
// ============================
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserPersonalDetailService, UserPersonalDetailService>();
builder.Services.AddScoped<IUserEducationService, UserEducationService>();
builder.Services.AddScoped<IUserExperienceService, UserExperienceService>();
builder.Services.AddScoped<IUserSkillService, UserSkillService>();
builder.Services.AddScoped<IUserProjectService, UserProjectService>();
builder.Services.AddScoped<IUserLanguageService, UserLanguageService>();
builder.Services.AddScoped<IUserSummaryObjectiveService, UserSummaryObjectiveService>();
builder.Services.AddScoped<IUserReferenceService, UserReferenceService>();
builder.Services.AddScoped<IUserSubscriptionService, UserSubscriptionService>();
builder.Services.AddScoped<ICvTemplateService, CvTemplateService>();
builder.Services.AddScoped<IUserSelectedTemplateService, UserSelectedTemplateService>();
builder.Services.AddScoped<ICvPreviewService, CvPreviewService>();

// ============================
// FILE STORAGE
// ============================
builder.Services.Configure<FileStorageSettings>(
    builder.Configuration.GetSection("FileStorage"));

builder.Services.AddScoped<IFileService>(sp =>
{
    var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<FileStorageSettings>>();
    var rootFolder = options.Value.ProfileImagesPath;

    if (string.IsNullOrWhiteSpace(rootFolder))
        throw new Exception("ProfileImagesPath is not configured in appsettings.json");

    if (!Directory.Exists(rootFolder))
        Directory.CreateDirectory(rootFolder);

    return new FileService(rootFolder);
});

// ============================
// TOKEN SERVICE
// ============================
builder.Services.AddScoped<ITokenService, TokenService>();

// ============================
// CONTROLLERS
// ============================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ============================
// CORS (PROFESSIONAL FIX)
// ============================
var allowedOrigins = builder.Configuration
    .GetSection("AllowedOrigins")
    .Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp", policy =>
    {
        policy
            .WithOrigins(allowedOrigins ?? new string[0])
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ============================
// JWT AUTH
// ============================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization();

// ============================
// BUILD APP
// ============================
var app = builder.Build();

// ============================
// AUTO DATABASE MIGRATION
// ============================
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<MyCvDbContext>();

        // Set timeout BEFORE any database operations
        dbContext.Database.SetCommandTimeout(300);

        // Test connection first (optional but helpful)
        if (!await dbContext.Database.CanConnectAsync())
        {
            logger.LogError("Cannot connect to the database");
            return;
        }

        logger.LogInformation("Applying database migrations with 300 second timeout...");

        // Apply migrations
        await dbContext.Database.MigrateAsync(); 

        logger.LogInformation("Database migration completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database migration failed.");
        throw; 
    }
}

// ============================
// MIDDLEWARE
// ============================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowWebApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();