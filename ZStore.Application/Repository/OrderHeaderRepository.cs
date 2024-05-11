using Microsoft.EntityFrameworkCore;
using ZStore.Data;
using ZStore.Application.Repository.IRepository;
using ZStore.Core;

namespace ZStore.Application.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext db;
        
        public OrderHeaderRepository(ApplicationDbContext _db):base(_db)
        {
            db = _db;
           
        }
       

        public void Update(OrderHeader obj)
        {
            db.OrderHeaders.Update(obj);
        }
    }
}
