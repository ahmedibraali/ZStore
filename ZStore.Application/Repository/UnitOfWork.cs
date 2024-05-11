using ZStore.Data;
using ZStore.Application.Repository.IRepository;


namespace ZStore.Application.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext db;

        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IShopingCartRepository ShopingCart { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }

        

        public UnitOfWork(ApplicationDbContext _db)
        {
            db = _db;
            Category = new CategoryRepository(db);
            Product = new ProductRepository(db);
            Company = new CompanyRepository(db);
            ShopingCart = new ShopingCartRepository(db);
            ApplicationUser = new ApplicationUserRepository(db);
        }
        public void Save()
        {
            db.SaveChanges();
        }
    }
}
