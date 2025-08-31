using Microsoft.EntityFrameworkCore;
using NetCoreERPSys.Models;

namespace NetCoreERPSys.DataAccess
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
        public DbSet<Product> Products { get; set; } // 代表数据库中的 Products 表.

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

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Title = "Fortune of Time",
                    Description = "A thrilling adventure across centuries, where a mysterious artifact holds the key to changing history itself.",
                    Author = "Billy Spark",
                    ISBN = "SWD9999001",
                    ListPrice = 99.00,
                    Price = 90.00,
                    Price50 = 85.00,
                    Price100 = 80.00,
                    CategoryId = 1,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 2,
                    Title = "Dark Skies",
                    Description = "In the depths of space, a lone crew discovers a signal that could be humanity's greatest discovery or its final undoing.",
                    Author = "Nancy Hoover",
                    ISBN = "CAW777777701",
                    ListPrice = 40.00,
                    Price = 30.00,
                    Price50 = 25.00,
                    Price100 = 20.00,
                    CategoryId = 2,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 3,
                    Title = "Vanish in the Sunset",
                    Description = "A detective on the verge of retirement takes on one last case that blurs the line between justice and revenge.",
                    Author = "Julian Button",
                    ISBN = "RITO5555501",
                    ListPrice = 55.00,
                    Price = 50.00,
                    Price50 = 40.00,
                    Price100 = 35.00,
                    CategoryId = 1,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 4,
                    Title = "Cotton Candy",
                    Description = "A heartwarming story of friendship and finding magic in the small moments of life at a summer carnival.",
                    Author = "Abby Muscles",
                    ISBN = "WS3333333301",
                    ListPrice = 70.00,
                    Price = 65.00,
                    Price50 = 60.00,
                    Price100 = 55.00,
                    CategoryId = 3,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 5,
                    Title = "Rock in the Ocean",
                    Description = "An epic tale of survival and hope, as one person's resilience is tested against the vast, unforgiving sea.",
                    Author = "Ron Parker",
                    ISBN = "SOTJ1111111101",
                    ListPrice = 30.00,
                    Price = 27.00,
                    Price50 = 25.00,
                    Price100 = 20.00,
                    CategoryId = 2,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 6,
                    Title = "Leaves and Wonders",
                    Description = "Explore the hidden world of botany and the surprising secrets of the plants that surround us every day.",
                    Author = "Laura Phantom",
                    ISBN = "FOT000000001",
                    ListPrice = 25.00,
                    Price = 23.00,
                    Price50 = 22.00,
                    Price100 = 20.00,
                    CategoryId = 3,
                    ImageUrl = ""
                }
            );
        }
    }
}
