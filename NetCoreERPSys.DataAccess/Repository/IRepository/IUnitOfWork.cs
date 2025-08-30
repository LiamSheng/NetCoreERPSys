using NetCoreERPSys.Models;

namespace NetCoreERPSys.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }

        void Save();
    }
}
