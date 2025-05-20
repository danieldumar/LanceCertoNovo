using LanceCerto.WebApp.Data;
using LanceCerto.WebApp.Models;
using LanceCerto.WebApp.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

    // Lockout (proteção contra brute-force)
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // E-mails únicos
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<LanceCertoDbContext>()
.AddDefaultTokenProviders();

// 🍪 Cookies de autenticação
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
});

// 🌍 MVC com Razor
builder.Services.AddControllersWithViews();

// 🤖 reCAPTCHA
builder.Services.Configure<RecaptchaSettings>(
    builder.Configuration.GetSection("GoogleReCaptcha"));
builder.Services.AddHttpClient();
builder.Services.AddScoped<RecaptchaService>();

// 🚫 Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(
    builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

#endregion

var app = builder.Build();

#region 🔄 Migrations SQLite
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LanceCertoDbContext>();
    db.Database.Migrate();
}
#endregion

#region 🌐 Pipeline HTTP

// Processa cabeçalhos de proxy antes de redirecionar para HTTPS
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

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

// Proteção contra requisições abusivas
app.UseIpRateLimiting();

app.UseAuthentication();
app.UseAuthorization();

// Rota padrão
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

#endregion

app.Run();