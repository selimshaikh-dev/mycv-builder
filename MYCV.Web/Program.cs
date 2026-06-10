using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using MYCV.Web.Helpers;
using MYCV.Web.Services.Api;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------
// 1️⃣ Add MVC Services
// ---------------------------------
builder.Services
    .AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive =
            AppJsonOptions.Options.PropertyNameCaseInsensitive;

        options.JsonSerializerOptions.DefaultIgnoreCondition =
            AppJsonOptions.Options.DefaultIgnoreCondition;

        foreach (var converter in AppJsonOptions.Options.Converters)
        {
            options.JsonSerializerOptions.Converters.Add(converter);
        }
    });

// ---------------------------------
// 2️⃣ Bind ApiSettings (IMPORTANT)
// ---------------------------------
builder.Services.Configure<ApiSettings>(
    builder.Configuration.GetSection("ApiSettings"));

// ---------------------------------
// 3️⃣ Register HttpContextAccessor
// ---------------------------------
builder.Services.AddHttpContextAccessor();

// ---------------------------------
// 4️⃣ Register JWT Token Handler
// ---------------------------------
builder.Services.AddTransient<JwtTokenHandler>();

// ---------------------------------
// 5️⃣ Register API HttpClients
// ---------------------------------

builder.Services.AddHttpClient<IAuthApiService, AuthApiService>((sp, client) =>
{
    var config = sp.GetRequiredService<IOptions<ApiSettings>>();

    client.BaseAddress = new Uri(config.Value.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);

    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient<ICvApiService, CvApiService>((sp, client) =>
{
    var config = sp.GetRequiredService<IOptions<ApiSettings>>();

    client.BaseAddress = new Uri(config.Value.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);

    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
})
.AddHttpMessageHandler<JwtTokenHandler>();

// ---------------------------------
// 6️⃣ Configure Cookie Authentication
// ---------------------------------
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";

        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;

        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

// ---------------------------------
// 7️⃣ Authorization
// ---------------------------------
builder.Services.AddAuthorization();

// ---------------------------------
// 8️⃣ Build Application
// ---------------------------------
var app = builder.Build();

// ---------------------------------
// 9️⃣ Configure Middleware Pipeline
// ---------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ---------------------------------
// 🔟 Routes
// ---------------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ---------------------------------
// 1️⃣1️⃣ Run App
// ---------------------------------
app.Run();