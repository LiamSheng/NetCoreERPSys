using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NetCoreERPSys.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Category Name")]
        public string Name { get; set; } = string.Empty; // Property initializer, 属性初始化为空字符串, 避免 null 引发的问题

        public int DisplayOrder { get; set; }
    }
}
