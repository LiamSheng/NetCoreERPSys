using NetCoreERPSys.Models;
using NetCoreERPSys.DataAccess.Repository.IRepository;

namespace NetCoreERPSys.DataAccess.Repository.IRepository
{
    public interface ICompanyRepository : IRepository<Company>
    {
        void Update(Company obj);
    }
}