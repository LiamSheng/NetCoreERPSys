using Microsoft.EntityFrameworkCore;
using NetCoreERPSys.DataAccess.Repository.IRepository;
using NetCoreERPSys.Models;

namespace NetCoreERPSys.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        private readonly DbSet<Category> dbSet;

        public ICategoryRepository Category { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(Category category)
        {
            _db.Update(category);
        }
    }
}
