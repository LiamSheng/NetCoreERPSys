using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetCoreERPSys.DataAccess.Repository.IRepository;
using NetCoreERPSys.Models;
using NetCoreERPSysWeb.Models;
using System.Diagnostics;
using System.Security.Claims;

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
        ShoppingCart shoppingCart = new()
        {
            Product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "Category"),
            Count = 1,
            ProductId = id
        };

        if (shoppingCart == null)
        {
            return NotFound();
        }

        return View(shoppingCart);
    }

    [HttpPost]
    [Authorize] //只有经过身份验证（即已登录）的用户才能访问此方法, 游客尝试访问的时候回传 HTTP 401, Cookies 中间件会将其重定向到登录页面.
    public IActionResult Details(ShoppingCart shoppingCart)
    {
        //User 是基类控制器内置的一个属性，代表当前发出请求的用户.
        var claimsIdentity = (ClaimsIdentity)User.Identity;

        // 从当前登录用户的身份声明中，提取出他的唯一ID，并将其赋值给 userId 这个字符串变量.
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        shoppingCart.ApplicationUserId = userId;

        // !!! 这里不仅仅是一个一查询, 框架在内存中创建了这条数据的一个快照 (Snapshot)，并开始“监视”或“追踪”这个 cartFromDb 对象.
        // 从此刻起，cartFromDb 就是一个被追踪的实体 (Tracked Entity).
        ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId && u.ProductId == shoppingCart.ProductId);

        if (cartFromDb != null)
        {
            // 任何对 cartFromDb 对象属性的更改，都会被 EF Core 监视到.
            cartFromDb.Count += shoppingCart.Count;

            // Update 方法主要用于未被追踪的实体, 不是通过当前 DbContext 查询出来的实体, 才需要调用 Update 方法.
            // 框架追踪的更新和手动调用 Update 方法的更新, 效果是一样的, 不会重复添加记录.
            _unitOfWork.ShoppingCart.Update(cartFromDb);
        }
        else
        {
            //add cart record
            _unitOfWork.ShoppingCart.Add(shoppingCart);
        }
        TempData["success"] = "Cart updated successfully!";

        _unitOfWork.Save();

        return RedirectToAction(nameof(Index));
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
