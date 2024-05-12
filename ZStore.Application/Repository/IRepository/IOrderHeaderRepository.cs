using ZStore.Core;

namespace ZStore.Application.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader obj);
        void UpdateStatus(int id ,string orderStatus ,string? paymentStatus=null);
        void UpdateStipePaymentID(int id ,string sessionId ,string? paymentIntentId);
    }
}
