using LanceCerto.WebApp.Data;
using LanceCerto.WebApp.Models;
using LanceCerto.WebApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;            // <-- Necessário para UseSqlite
using Microsoft.AspNetCore.Http;
using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

#region 🔧 Serviços e Configurações

// 📦 Banco de Dados SQLite
builder.Services.AddDbContext<LanceCertoDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔐 ASP.NET Identity (usuários com chave primária int)
builder.Services.AddIdentity<Usuario, IdentityRole<int>>(options =>
{
    // Regras de senha
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    // Proteção contra brute-force
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // E-mails únicos obrigatórios
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<LanceCertoDbContext>()
.AddDefaultTokenProviders();

// 🍪 Cookies de autenticação segura
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
});

// 🌍 MVC com Razor Views
builder.Services.AddControllersWithViews();

// 🤖 Serviço de verificação reCAPTCHA
builder.Services.AddHttpClient();
builder.Services.AddScoped<RecaptchaService>();

// 🚫 Rate Limiting por IP (AspNetCoreRateLimit)
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

#endregion

var app = builder.Build();

#region 🌐 Pipeline HTTP

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseIpRateLimiting(); // 🔒 Proteção contra requisições abusivas

app.UseAuthentication();
app.UseAuthorization();

// 🧭 Rota padrão
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

#endregion

app.Run();