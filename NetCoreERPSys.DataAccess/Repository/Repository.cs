using Microsoft.EntityFrameworkCore;
using NetCoreERPSys.DataAccess.Repository.IRepository;

namespace NetCoreERPSys.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;

        // _db.Categories.ToList() 就是调用了 DbSet<Category> 类型的对象上的 CRUD 方法.
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>(); // _db.Categories == dbSet
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T Get(System.Linq.Expressions.Expression<Func<T, bool>> filter)
        {
            // IQueryable<T> query -> 准备从 Categories 表里取数据.
            IQueryable<T> query = dbSet;
            query = query.Where(filter);

            // dbSet.Where(filter).FirstOrDefault() 完全正确.
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll()
        {
            IQueryable<T> query = dbSet;
            return query.ToList();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}
