// 需要像容器添加服务/中间件的时候, 需要在 Program.cs 里添加
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using NetCoreERPSys.DataAccess;
using NetCoreERPSys.DataAccess.Repository;
using NetCoreERPSys.DataAccess.Repository.IRepository;
using NetCoreERPSys.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

/*
 * 1. 向项目中添加默认的用户身份认证和管理功能.
 * 2. 设定了一个安全规则，即用户必须通过某种方式（如邮件）确认其账户，然后才能登录.
 * 3. Identity 系统使用 Entity Framework Core，并通过 ApplicationDbContext 这个类将所有用户和角色的数据保存到数据库中.
 */
//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

// 从一个仅包含用户账户管理的基础身份验证系统，升级到了一个同时支持用户和角色（Roles）管理的更完整的身份验证和授权系统.
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 这里不是 Cookie 本身的特性,而是配置框架认证中间件如何处理认证行为的服务器端的指令.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

// Identity 的 Razor 页面需要注册服务才能使用.
builder.Services.AddRazorPages();

// builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IEmailSender, EmailSender>();

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
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages(); // 映射了 Razor 页面, 使其可以通过 URL 访问.

app.MapControllerRoute(
    name: "default",
    // 文件名是 HomeController.cs，但 URL 里只需要写 Home.
    // 一个命名约定：框架在匹配控制器时，会自动忽略类名末尾的 Controller 这个词，这让 URL 更简洁.
    pattern: "{area=Customer}/{controller=Home}/{action=Privacy}/{id?}");

app.Run();
