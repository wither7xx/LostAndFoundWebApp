using LostAndFoundWebApp.Models;
using LostAndFoundWebApp.Services.Email;
using LostAndFoundWebApp.Services.Mysql;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Logout";
    });
builder.Services.AddSingleton<IEmailSender, EmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
if (DatabaseOperate.TestConnection())
{
    Console.WriteLine("数据库连接成功！");

}
else
{
    Console.WriteLine("数据库连接失败！");

}
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.UseSession();
app.UseAuthentication();

app.UseStaticFiles();
var uploadsImagesPath = Path.Combine(app.Environment.WebRootPath, "uploads", "images");
var uploadsClaimsPath = Path.Combine(app.Environment.WebRootPath, "uploads", "claims");

if (!Directory.Exists(uploadsImagesPath))
{
    Directory.CreateDirectory(uploadsImagesPath);
}

if (!Directory.Exists(uploadsClaimsPath))
{
    Directory.CreateDirectory(uploadsClaimsPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsImagesPath),
    RequestPath = "/uploads/images"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsClaimsPath),
    RequestPath = "/uploads/claims"
});

app.Run();
