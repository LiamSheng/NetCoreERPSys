using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NetCoreERPSys.Models.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }

        // CategoryList 作为一个 不可为空的引用类型，在模型验证时被系统隐式地当作是必需的 [Required].
        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}
