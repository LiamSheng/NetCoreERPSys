using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NetCoreERPSysWeb.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Category Name")]
        [MaxLength(25)]
        public string Name { get; set; } = string.Empty; // Property initializer, 属性初始化为空字符串, 避免 null 引发的问题

        [Range(1, 100, ErrorMessage = "The field DisplayOrder must be between 1 ~ 100.")]
        public int DisplayOrder { get; set; }
    }
}
