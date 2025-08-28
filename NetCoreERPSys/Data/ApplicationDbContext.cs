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
    }
}
