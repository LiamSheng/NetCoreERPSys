// 需要像容器添加服务/中间件的时候, 需要在 Program.cs 里添加
using Microsoft.EntityFrameworkCore;
using NetCoreERPSys.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    // 文件名是 HomeController.cs，但 URL 里只需要写 Home.
    // 一个命名约定：框架在匹配控制器时，会自动忽略类名末尾的 Controller 这个词，这让 URL 更简洁.
    pattern: "{controller=Home}/{action=Privacy}/{id?}");

app.Run();
