namespace ZStore.Application.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        ICompanyRepository Company { get; }
        IProductRepository Product { get; }
        IShopingCartRepository ShopingCart { get; }
        IApplicationUserRepository ApplicationUser { get; }
        void Save();
    }
}
