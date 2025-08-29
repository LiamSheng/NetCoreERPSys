using Microsoft.AspNetCore.Mvc.RazorPages;
using NetCoreRazor.Data;
using NetCoreRazor.Models;

namespace NetCoreRazor.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public List<Category> CategoryList { get; set; } = new List<Category>();

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
            CategoryList = _db.Categories.ToList();
            // 数据自动绑定到 @model NetCoreRazor.Pages.Categories.IndexModel
            // 不需要再返回视图了
        }
    }
}
