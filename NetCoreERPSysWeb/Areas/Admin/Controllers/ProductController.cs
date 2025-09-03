using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NetCoreERPSys.DataAccess.Repository.IRepository;
using NetCoreERPSys.Models;
using NetCoreERPSys.Models.ViewModels;
using NetCoreERPSys.Utility;

namespace NetCoreERPSysWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    // /Account/Login?ReturnUrl=%2FAdmin%2FCategory%2FIndex.
    // /identity/Account/Login?ReturnUrl=%2FAdmin%2FCategory%2FIndex 返回登录页面. -> 以 customer 登录 -> Access Denied.
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IWebHostEnvironment _hostEnvironment; // 框架自带, 不需要依赖注入

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> objList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return View(objList);
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                // 如果模型验证失败，重新填充下拉列表并返回视图
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }

            string wwwRootPath = _hostEnvironment.WebRootPath;

            // --- 数据库保存逻辑 ---
            if (productVM.Product.Id == 0) // 这是 Create (创建)
            {
                // 只有在创建新产品时才处理图片上传
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productsPath = Path.Combine(wwwRootPath, @"images\products");
                    using (var fileStream = new FileStream(Path.Combine(productsPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\products\" + fileName;
                }

                _unitOfWork.Product.Add(productVM.Product);
                TempData["success"] = "Product created successfully!";
            }
            else // 这是 Update (更新)
            {
                // 1. 先从数据库加载原始对象 (EF Core 会开始跟踪它)
                var productFromDb = _unitOfWork.Product.Get(u => u.Id == productVM.Product.Id);
                if (productFromDb == null)
                {
                    return NotFound(); // 如果找不到，返回 404
                }

                // 2. 检查是否有新文件上传
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productsPath = Path.Combine(wwwRootPath, @"images\products");

                    // 删除旧图片
                    if (!string.IsNullOrEmpty(productFromDb.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, productFromDb.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // 保存新图片
                    using (var fileStream = new FileStream(Path.Combine(productsPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productFromDb.ImageUrl = @"\images\products\" + fileName; // 更新已跟踪对象的 ImageUrl
                }

                // 3. 将 ViewModel 中的其他属性值更新到已跟踪的数据库对象上
                productFromDb.Title = productVM.Product.Title;
                productFromDb.Description = productVM.Product.Description;
                productFromDb.ISBN = productVM.Product.ISBN;
                productFromDb.Author = productVM.Product.Author;
                productFromDb.ListPrice = productVM.Product.ListPrice;
                productFromDb.Price = productVM.Product.Price;
                productFromDb.Price50 = productVM.Product.Price50;
                productFromDb.Price100 = productVM.Product.Price100;
                productFromDb.CategoryId = productVM.Product.CategoryId;

                // 4. 更新这个已跟踪的、并且属性已更新的对象
                _unitOfWork.Product.Update(productFromDb);
                TempData["success"] = "Product updated successfully!";
            }

            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        public IActionResult Upsert(int? id)
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
            // ViewData["CategoryList"] = CategoryList;

            ProductVM productVM = new ProductVM();
            productVM.CategoryList = CategoryList;
            productVM.Product = new Product();

            if (id is null || id == 0)
            {
                // create
                return View(productVM);
            }
            else
            {
                // update
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);

                return View(productVM);
            }
        }

        #region API CALLS
        //http://localhost:5188/Admin/Product/getallapi
        [HttpGet]
        public IActionResult GetAllAPI()
        {
            List<Product> objList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objList });
        }


        // [HttpDelete] 方法会返回一个 IActionResult 或 ActionResult<T> 类型
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Deleted Successful!" });
        }
        #endregion
    }
}
