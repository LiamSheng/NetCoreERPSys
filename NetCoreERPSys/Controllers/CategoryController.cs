using Microsoft.AspNetCore.Mvc;
using NetCoreERPSys.Data;
using NetCoreERPSys.Models;

namespace NetCoreERPSys.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        //  框架会在以下两个位置查询 View, 若都没有则会抛出异常.
        // /Views/Category/Index.cshtml
        // /Views/Shared/Index.cshtml
        public IActionResult Index()
        {
            List<Category> objList = _db.Categories.ToList();
            return View(objList);
        }

        // 当用户在创建页面上填完表单并点击“提交”按钮时，会发送一个 POST 请求
        // [HttpPost] 确保只有 POST 请求才会进入这个方法
        /*
         * 1. 用户表单提交的时候, 浏览器将这些数据打包成 Name=Comedy&DisplayOrder=4 的格式, 并通过 POST 请求发送给服务器.
         * 2. 请求到达了您那个带有 [HttpPost] 标记的 Create 方法.
         * 3. 模型绑定系统看到方法签名需要一个 Category 类型的参数 obj, 于是会自动创建一个空的 Category 对象实例.
         * 4.   找到 Name=Comedy -> 执行 obj.Name = "Comedy";
         *      找到 DisplayOrder=4 -> 执行 obj.DisplayOrder = 4;
         * 5. 最终, 这个 obj 对象就被填充好了, 然后传递给 Create 方法.
         */
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            _db.Categories.Add(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
            // return View(); // 显示一个全新的、空的表单页面. 最常用于响应 GET 请求的 Create() 方法.
        }

        // 为什么有两个 Create 方法?
        // 此动作方法负责响应 GET 请求, 向用户显示一个空的“创建分类”表单页面.
        public IActionResult Create()
        {
            return View();
        }
    }
}
