using ZStore.Core;

namespace ZStore.Application.Repository.IRepository
{
    public interface IShopingCartRepository : IRepository<ShopingCart>
    {
        void Update(ShopingCart obj);

    }
}
