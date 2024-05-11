using Microsoft.EntityFrameworkCore;
using ZStore.Data;
using ZStore.Application.Repository.IRepository;
using ZStore.Core;

namespace ZStore.Application.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext db;
        
        public OrderDetailRepository(ApplicationDbContext _db):base(_db)
        {
            db = _db;
           
        }
       

        public void Update(OrderDetail obj)
        {
            db.OrderDetails.Update(obj);
        }
    }
}
