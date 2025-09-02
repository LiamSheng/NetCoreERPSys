using Microsoft.AspNetCore.Mvc;
using NetCoreERPSys.DataAccess.Repository.IRepository;
using NetCoreERPSys.Models;
using NetCoreERPSysWeb.Models;
using System.Diagnostics;

namespace NetCoreERPSysWeb.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly IUnitOfWork _unitOfWork;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        IEnumerable<Product> productsList = _unitOfWork.Product.GetAll(includeProperties: "Category");
        return View(productsList);
    }

    public IActionResult Details(int id)
    {
        Product product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "Category");

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
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
