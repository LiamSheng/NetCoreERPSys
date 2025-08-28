using Microsoft.EntityFrameworkCore;
using NetCoreERPSys.Models;

namespace NetCoreERPSys.Data
{
    /*
     * DbContext 这个(抽象)基类需要 options 对象来了解它应该用什么连接字符串、什么数据库类型和数据库交互.
     */
    public class ApplicationDbContext : DbContext
    {
        /*
         * 构造函数定义:
         * - 创建一个 ApplicationDbContext 的新实例时, 必须提供一个 options 对象.
         * - 这个 options 对象包含了配置 DbContext 所需的信息, 比如数据库连接字符串, 数据库提供程序等.
         */
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // 在 Nuget console 运行 add-migration AddCategoryTable -> update-database.
        public DbSet<Category> Categories { get; set; } // 代表数据库中的 Categories 表.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 告诉 EF Core，在创建 Category 表之后，
            // 请立即向表中插入这三条数据。
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                new Category { Id = 2, Name = "SciFi", DisplayOrder = 2 },
                new Category { Id = 3, Name = "History", DisplayOrder = 3 }
            );

            // 假设您不想让表名叫 Categories，而是叫 MyCategories
            // modelBuilder.Entity<Category>().ToTable("MyCategories");

            // 假设您想让 Name 字段在数据库中不能重复
            // modelBuilder.Entity<Category>().HasIndex(c => c.Name).IsUnique();
        }
    }
}
