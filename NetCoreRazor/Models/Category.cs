using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NetCoreRazor.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Category Name")]
        [MaxLength(25)]
        public string Name { get; set; } = string.Empty;

        [Range(1, 100, ErrorMessage = "The field DisplayOrder must be between 1 ~ 100.")]
        public int DisplayOrder { get; set; }
    }
}
