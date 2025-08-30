using NetCoreERPSys.Models;

namespace NetCoreERPSys.DataAccess.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        // 除了 IRepository 定义的方法, 另加上本接口额外的方法.
        void Update(Category category);

        void Save();
    }
}
