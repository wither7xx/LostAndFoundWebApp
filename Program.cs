using LostAndFoundWebApp.Models;
using LostAndFoundWebApp.Services.Email;
using LostAndFoundWebApp.Services.Mysql;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // 未认证时跳转的页面
        options.LogoutPath = "/Logout"; // 登出时跳转的页面
        options.AccessDeniedPath = "/AccessDenied"; // 无权限访问时跳转的页面
    });

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

app.UseAuthentication(); // 启用身份认证

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();

