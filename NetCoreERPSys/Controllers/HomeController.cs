using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NetCoreERPSys.Models;

namespace NetCoreERPSys.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        //return View();
        return View("Index");
    }

    /*
     * 此样板项目页面 https://localhost:7053/Home/Privacy
     * 其中:
     *  1. Home 对应的是 HomeController.cs 这个控制器文件.
     *  2. Privacy 对应的是 HomeController 类里面的这动作方法.
     */
    public IActionResult Privacy()
    {
        return View(); // <- 框架会去寻找 Privacy.cshtml
        /*
         * 框架会按顺序在几个地方查找，直到找到为止：
         *  1. Views/[控制器名]/[动作名].cshtml  (例如: Views/Home/Privacy.cshtml)
         *  2. Views/Shared/[动作名].cshtml   (例如: Views/Shared/Privacy.cshtml)
         */
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
