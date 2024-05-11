using ZStore.Core;

namespace ZStore.Application.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader obj);

    }
}
