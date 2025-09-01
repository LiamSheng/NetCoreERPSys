using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NetCoreERPSys.DataAccess.Repository.IRepository;
using NetCoreERPSys.Models;
using NetCoreERPSys.Models.ViewModels;

namespace NetCoreERPSysWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
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
        public IActionResult Upsert(ProductVM productVM, IFormFile file)
        {
            // 用户点击提交表单后, 浏览器只会提交用户输入或选择的值, 例如 Product.Name
            // 它不会将整个下拉列表的选项（也就是 CategoryList）提交回来.
            // obj.CategoryList 属性将会是 null, 将会导致 ModelState.IsValid 返回 false.
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath; // wwwroot 文件夹的绝对路径.

                if (file == null)
                {

                }
                else
                {
                    string fileName = Guid.NewGuid().ToString(); // 使用 GUID 作为随机的文件名, 避免文件名冲突.
                    string extension = Path.GetExtension(file.FileName); // 获取上传文件的扩展名, 包括点号, 例如 ".jpg"
                    string fullFileName = fileName + extension; // 生成完整的文件名, 例如 "a1b2c3d4-e5f6-7g8h-9i0j-k1l2m3n4o5p6.jpg"
                    string productsPath = Path.Combine(wwwRootPath, @"images\products"); // 图片将上传到 wwwroot/images/products 文件夹下.

                    // 这个正在被编辑的产品，之前是否已经有一张图片了？
                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath); // 删除服务器磁盘上的图像.
                        }
                    }

                    // 任何实现了 IDisposable 接口的对象, 都应该（也只能）在 using 语句中使用.
                    using (var fileStream = new FileStream(Path.Combine(productsPath, fullFileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream); // 将上传的文件内容复制到服务器上的文件流中, 实现文件保存.
                    }
                    productVM.Product.ImageUrl = @"\images\products\" + fullFileName; // 赋值给 Product 对象的 ImageUrl 属性, 以便存储到数据库.
                }

                if (productVM.Product.Id == 0)
                {
                    // Add.
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    // Update
                    _unitOfWork.Product.Update(productVM.Product);
                }

                _unitOfWork.Save();
                TempData["created"] = "Product created successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category
                    .GetAll()
                    .Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    });

                productVM.CategoryList = CategoryList;

                return View(productVM);
            }
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

        //http://localhost:5188/Admin/Product/getallapi
        #region API CALLS
        [HttpGet]
        public IActionResult GetAllAPI()
        {
            List<Product> objList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objList });
        }
        #endregion

    }
}
