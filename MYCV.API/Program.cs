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
// 1️⃣ Configure DbContext
// ============================
builder.Services.AddDbContext<MyCvDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ============================
// 2️⃣ Configure Email Services
// ============================
builder.Services.AddSingleton<EmailTemplateService>(sp =>
    new EmailTemplateService(builder.Environment.ContentRootPath));
builder.Services.AddScoped<IEmailService, EmailService>();

// ============================
// 3️⃣ Configure Repositories
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
// 4️⃣ Configure Application Services
// ============================
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserPersonalDetailService, UserPersonalDetailService>();
builder.Services.AddScoped<IUserEducationService, UserEducationService>();
builder.Services.AddScoped<IUserExperienceService, UserExperienceService>();
builder.Services.AddScoped < IUserSkillService, UserSkillService>();
builder.Services.AddScoped<IUserProjectService, UserProjectService>();
builder.Services.AddScoped<IUserLanguageService, UserLanguageService>();
builder.Services.AddScoped<IUserSummaryObjectiveService, UserSummaryObjectiveService>();
builder.Services.AddScoped<IUserReferenceService, UserReferenceService>();
builder.Services.AddScoped<IUserSubscriptionService, UserSubscriptionService>();
builder.Services.AddScoped<ICvTemplateService, CvTemplateService>();
builder.Services.AddScoped<IUserSelectedTemplateService, UserSelectedTemplateService>();
builder.Services.AddScoped<ICvPreviewService, CvPreviewService>();

// ============================
// 5️⃣ Configure File Service
// ============================

builder.Services.Configure<FileStorageSettings>(
    builder.Configuration.GetSection("FileStorage"));

builder.Services.AddScoped<IFileService>(sp =>
{
    var options = sp.GetRequiredService<
        Microsoft.Extensions.Options.IOptions<FileStorageSettings>>();

    var rootFolder = options.Value.ProfileImagesPath;

    if (string.IsNullOrWhiteSpace(rootFolder))
        throw new Exception("ProfileImagesPath is not configured in appsettings.json");

    // Ensure folder exists
    if (!Directory.Exists(rootFolder))
        Directory.CreateDirectory(rootFolder);

    return new FileService(rootFolder);
});


// ============================
// 6️⃣ Configure Security Services
// ============================
builder.Services.AddScoped<ITokenService, TokenService>();

// ============================
// 7️⃣ Add Controllers, Swagger, CORS
// ============================
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp", policy =>
    {
        policy
            .WithOrigins("https://localhost:7167") // front-end
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ============================
// 8️⃣ Configure JWT Authentication
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
// 9️⃣ Build App
// ============================
var app = builder.Build();

// ============================
// 🔟 Configure Middleware
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