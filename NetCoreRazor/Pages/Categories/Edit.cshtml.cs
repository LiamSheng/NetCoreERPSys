using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NetCoreRazor.Data;
using NetCoreRazor.Models;

namespace NetCoreRazor.Pages.Categories
{
    [BindProperties]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        // Model.Category 直接与其 Razor page 绑定.
        // 框架默认不会将 POST 请求的表单数据绑定到 PageModel 的属性上.
        // 必须明确地使用 [BindProperty] 来“授权”这个属性, 告诉框架：“请把表单数据绑定到这个属性上”.
        // [BindProperty]
        public Category? Category { get; set; }

        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public void OnGet(int? id)
        {
            if (id == null || id == 0)
            {
                NotFound();
            }
            Category = _db.Categories.Find(id);
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid && Category != null)
            {
                _db.Categories.Update(Category);
                _db.SaveChanges();
                TempData["created"] = "Category updated successfully";
                return RedirectToPage("Index");
            }
            else
            {
                return Page(); // 保持在当前页面而不是跳转
            }

        }
    }
}
