using LostAndFoundWebApp.Models;
using LostAndFoundWebApp.Services.Email;
using LostAndFoundWebApp.Services.Mysql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

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

app.Run();
