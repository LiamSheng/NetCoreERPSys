using Microsoft.EntityFrameworkCore;
using NetCoreERPSys.DataAccess.Repository.IRepository;
using System.Linq.Expressions;

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

            // 如果 T 是 Product, 就把 Category 一起 Include 进来.
            _db.Products.Include(u => u.Category);
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T Get(System.Linq.Expressions.Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            if (tracked)
            {
                // IQueryable<T> query -> 准备从 Categories 表里取数据.
                IQueryable<T> query = dbSet;
                query = query.Where(filter);

                if (includeProperties != null)
                {
                    foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProp);
                    }
                }

                // dbSet.Where(filter).FirstOrDefault() 完全正确.
                return query.FirstOrDefault();
            }
            else
            {
                IQueryable<T> query = dbSet.AsNoTracking(); // AsNoTracking() 让 EF Core 不去追踪这个实体.
                query = query.Where(filter);

                if (includeProperties != null)
                {
                    foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProp);
                    }
                }

                // dbSet.Where(filter).FirstOrDefault() 完全正确.
                return query.FirstOrDefault();
            }
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrEmpty(includeProperties))
            {
                // 如果指令字符串是 "Category,CoverType"，
                // .Split() 会将它分割成一个数组: ["Category", "CoverType"]
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    // 循环遍历数组，为每个属性名调用 EF Core 的 .Include() 方法
                    query = query.Include(includeProp);
                }
            }

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
