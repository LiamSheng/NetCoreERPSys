using NetCoreERPSys.DataAccess.Repository.IRepository;
using NetCoreERPSys.Models;

namespace NetCoreERPSys.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            this._db = db;
        }

        public void Update(Product obj)
        {
            this._db.Products.Update(obj);
        }
    }
}
