using Microsoft.EntityFrameworkCore;
using System;
using TouristSpotWeb.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// ���U IHttpClientFactory
builder.Services.AddHttpClient();
// ���U IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// �P���I��Ʈw�s�u
builder.Services.AddDbContext<Sightseeing_spotsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WebDatabase"))
);

// �s�W Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// �t�m��������
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)  // �q�{�ϥ� Cookie ����
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";  // �n�J����
        options.AccessDeniedPath = "/Account/AccessDenied";  // �v����������
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

// �t�m HTTP �ШD�޹D
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// �ҥΨ������ҩM���v
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();  // �ҥ� Session

// �t�m����
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");
app.MapControllerRoute(
    name: "TouristSpotDetails",
    pattern: "TouristSpotDetails/{id?}",
    defaults: new { controller = "Home", action = "TouristSpotDetails" });
app.MapControllerRoute(
    name: "TouristSpotChange",
    pattern: "TouristSpot/Manage/{id?}",
    defaults: new { controller = "Home", action = "Manage" });

app.Run();
