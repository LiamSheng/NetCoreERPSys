using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NetCoreRazor.Data;
using NetCoreRazor.Models;

namespace NetCoreRazor.Pages.Categories
{
    [BindProperties]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        // Model.Category 直接与其 Razor page 绑定.
        // 框架默认不会将 POST 请求的表单数据绑定到 PageModel 的属性上.
        // 必须明确地使用 [BindProperty] 来“授权”这个属性, 告诉框架：“请把表单数据绑定到这个属性上”.
        // [BindProperty]
        public Category? Category { get; set; }

        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
        }

        // 不使用 [BindProperty] + 带参数的 OnPost:
        //public IActionResult OnPost(Category category)
        //{
        //    _db.Categories.Add(category);
        //    _db.SaveChanges();

        //    return RedirectToPage("Index");
        //}

        public IActionResult OnPost()
        {
            // 直接使用被框架自动填充的 Category 属性
            _db.Categories.Add(Category);
            _db.SaveChanges();
            TempData["created"] = "Category created successfully";
            return RedirectToPage("Index");
        }
    }
}
