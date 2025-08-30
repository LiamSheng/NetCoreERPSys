using Microsoft.AspNetCore.Mvc;
using NetCoreERPSysWeb.Data;
using NetCoreERPSysWeb.Models;

namespace NetCoreERPSysWeb.Controllers
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
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("DisplayOrder", "Name != DisplayOrder is a must!");
            }

            if (obj.Name != null && obj.Name.ToLower() == "asp-validation-summary")
            {
                ModelState.AddModelError("", "The name 'asp-validation-summary' is not allowed.");
            }

            // ModelState 是处理模型验证结果的对象, 验证提交的表单数据是否符合 Model 上面用方括号 [...] 定义的那些数据注解特性定义的规则.
            if (ModelState.IsValid)
            {
                _db.Categories.Add(obj);
                _db.SaveChanges();
                TempData["created"] = "Category created successfully!"; // 设置一个临时数据, 用于在重定向后的页面显示成功消息, TempData 只能保存到下一个请求, 之后就会被清除.
                return RedirectToAction("Index");
            }
            return View(obj); // 将用户提交的数据 obj 再次传递给视图, 让用户看到自己刚刚输入的内容, 以便进行修改.
            // return View(); // 显示一个全新的、空的表单页面. 最常用于响应 GET 请求的 Create() 方法.
        }

        // 为什么有两个 Create 方法?
        // 此动作方法负责响应 GET 请求, 向用户显示一个空的“创建分类”表单页面.
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Edit(int? id) // 参数名 'id' 与路由中的 '{id?}' 匹配, 与 asp-route-id 的 'id' 对应.
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _db.Categories.Find(id);
            // Category? categoryFromDb = _db.Categories.FirstOrDefault(c => c.Id == id);
            // Category? categoryFromDb = _db.Categories.Where(c => c.Id == id).FirstOrDefault();

            if (categoryFromDb == null)
            {
                return NotFound();
            }

            // 点击 Edit 按钮的时候, 就会根据 Id 的值查询到这个对象,
            // 框架中的 asp-for 标签助手读取了这个模型的值，并自动将其设置为了输入框的 value.
            return View(categoryFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("DisplayOrder", "Name != DisplayOrder is a must!");
            }

            if (obj.Name != null && obj.Name.ToLower() == "asp-validation-summary")
            {
                ModelState.AddModelError("", "The name 'asp-validation-summary' is not allowed.");
            }

            if (ModelState.IsValid)
            {
                _db.Categories.Update(obj);
                _db.SaveChanges();
                TempData["updated"] = "Category updated successfully!";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(int? id) // 参数名 'id' 与路由中的 '{id?}' 匹配, 与 asp-route-id 的 'id' 对应.
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? categoryFromDb = _db.Categories.Find(id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }

        /**
         * 表单发送 HTTP POST 请求, 框架会寻找:
         *  1. 被 [HttpPost] 特性标记,
         *  2. 其“动作名称”是 Delete 的方法
         */
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id) // 一个类中不能有签名完全一样的两个方法, 故改为 DeletePOST.
        {
            Category? categoryFromDb = _db.Categories.Find(id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }

            _db.Categories.Remove(categoryFromDb);
            TempData["deleted"] = "Category deleted successfully!";
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
