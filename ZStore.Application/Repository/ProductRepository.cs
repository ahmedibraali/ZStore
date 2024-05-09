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
            var objFromDb = db.Products.FirstOrDefault(u=>u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Title=obj.Title;
                objFromDb.Description=obj.Description;
                objFromDb.ISBN=obj.ISBN;
                objFromDb.Author=obj.Author;
                objFromDb.CategoryId=obj.CategoryId;
                objFromDb.ListPrice=obj.ListPrice;
                objFromDb.Price=obj.Price;
                objFromDb.Price100=obj.Price100;
                objFromDb.Price50=obj.Price50;
                if(obj.ImageUrl !=null)
                {
                    objFromDb.ImageUrl=obj.ImageUrl;
                }
            }
          // db.Products.Update(obj);
        }
    }
}
