using Microsoft.EntityFrameworkCore;
using ZStore.Data;
using ZStore.Application.Repository.IRepository;
using ZStore.Core;

namespace ZStore.Application.Repository
{
    public class ShopingCartRepository : Repository<ShopingCart>, IShopingCartRepository
    {
        private readonly ApplicationDbContext db;
        
        public ShopingCartRepository(ApplicationDbContext _db):base(_db)
        {
            db = _db;
           
        }
       

        public void Update(ShopingCart obj)
        {
            db.ShopingCarts.Update(obj);
        }
    }
}
