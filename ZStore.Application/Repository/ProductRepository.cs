using ZStore.Data;
using ZStore.Application.Repository.IRepository;
using ZStore.Core;

namespace ZStore.Application.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext db;

        public ProductRepository(ApplicationDbContext _db):base(_db)
        {
            db = _db;
        }
        public void Update(Product obj)
        {
           db.Products.Update(obj);
        }
    }
}
