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

        public IActionResult Create()
        {
            return View(); // View 方法不传值的话, 会自动传入一个带有默认值的 Category 对象.
        }
    }
}
