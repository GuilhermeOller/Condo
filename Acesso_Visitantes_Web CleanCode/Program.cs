using Acesso_Moradores_Visitantes.Data;
using Acesso_Moradores_Visitantes.Extensions;
using Acesso_Moradores_Visitantes.Repositorys.Implementations;
using Acesso_Moradores_Visitantes.Repositorys.Interfaces;
using Acesso_Moradores_Visitantes.Services.Implementations;
using Acesso_Moradores_Visitantes.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.FileProviders;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "AuthCookie";
        options.LoginPath = "/Login/Index";
        options.AccessDeniedPath = "/Login/Index";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = false;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

builder.Services.AddRepositories();
builder.Services.AddServices();

// Configuração de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Configuração de sessão
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = false;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("dbAcesso")));

builder.Services.AddDbContext<AppDbContextMorador>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("dbAcesso")));

// Serviços MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Pipeline de configuração
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Visitante/Index");
    app.UseHsts();
}
app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(@"D:\IMGS"),
    RequestPath = "/imagens"
});
//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors("AllowAll");
app.UseSession(); // Importante vir antes de Authentication
app.UseAuthentication();
app.UseAuthorization();

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Visitante}/{action=Index}/{id?}");
app.MapFallbackToController("Index", "Visitante");
app.MapControllers();

app.Run();