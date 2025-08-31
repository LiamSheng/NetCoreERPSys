using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NetCoreERPSys.DataAccess.Repository.IRepository;
using NetCoreERPSys.Models;

namespace NetCoreERPSysWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Product> objList = _unitOfWork.Product.GetAll().ToList();
            return View(objList);
        }

        [HttpPost]
        public IActionResult Create(Product obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(obj);
                _unitOfWork.Save();
                TempData["created"] = "Product created successfully!";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        public IActionResult Create()
        {
            /**
             * SelectListItem 是 ASP.NET Core MVC 和 Razor Pages 内置的一个辅助类，
             * 它被专门设计用来填充 HTML 的 <select> 下拉框元素, 它只有两个核心属性
             *      Text: 将在下拉框中显示给用户看的文本.
             *      Value: 当用户选中这一项时，提交给服务器的值.
             */
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category
                .GetAll()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });

            /**
             * ViewBag:
             *  1. 从 Controller 到 View 的单向传递, 非常适合传递那些不属于核心业务模型（Model）的临时性、少量数据.
             *  2. ViewBag 是一个 dynamic 类型的对象, 可以在运行时动态地向它添加任何属性, 而无需在编译前预先定义它们.
             *  3. 生命周期非常短暂，仅存在于当前这一次 HTTP 请求中. 当服务器处理完请求并将视图响应给浏览器后数据就会销毁.
             *  4. ViewBag 实际上只是 ViewData 的一个语法糖:
             *      ViewBag.Message = "Hello World";
             *      ViewData["Message"] = "Hello World;
             */
            // ViewBag.CategoryList = CategoryList;
            ViewData["CategoryList"] = CategoryList;

            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);

            if (productFromDb == null)
            {
                return NotFound();
            }

            return View(productFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Product obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(obj);
                _unitOfWork.Save();
                TempData["updated"] = "Product updated successfully!";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);

            if (productFromDb == null)
            {
                return NotFound();
            }

            return View(productFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Product? ProductFromDb = _unitOfWork.Product.Get(u => u.Id == id);

            if (ProductFromDb == null)
            {
                return NotFound();
            }

            _unitOfWork.Product.Remove(ProductFromDb);
            TempData["deleted"] = "Product deleted successfully!";
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }
    }
}
