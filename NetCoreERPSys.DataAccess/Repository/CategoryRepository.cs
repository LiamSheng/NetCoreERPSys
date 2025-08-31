using NetCoreERPSys.DataAccess.Repository.IRepository;
using NetCoreERPSys.Models;

namespace NetCoreERPSys.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        // Save() 方法在 UnitOfWork 里单独实现.

        public void Update(Category category)
        {
            _db.Categories.Update(category);
        }
    }
}
