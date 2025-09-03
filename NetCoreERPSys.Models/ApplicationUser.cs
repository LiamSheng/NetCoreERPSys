using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetCoreERPSys.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string? Name { get; set; }

        public string? StreetAddress { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? PostalCode { get; set; }

        public int? CompanyId { get; set; }

        [ForeignKey("CompanyId")] // 指的是这个类的 public int? CompanyId { get; set; }
        public Company Company { get; set; } // Company 中 名为 "Id" 或 "CompanyId" 的属性时，
                                             // 会自动把它识别为主键, 与之对应.
    }
}
