using System.Linq.Expressions;

namespace NetCoreERPSys.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        // 几乎所有的 LINQ to Objects 和 LINQ to Entities (EF Core) 的查询,
        // 返回的都是 IEnumerable<T> 或其变体 IQueryable<T>.
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);

        // 描述了如何从一堆 T 类型的对象中筛选.
        // var category = _repository.Get(c => c.Id == 5);
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false);

        void Add(T entity);

        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entity);
    }
}
