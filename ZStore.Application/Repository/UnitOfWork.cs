using ZStore.Data;
using ZStore.Application.Repository.IRepository;


namespace ZStore.Application.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext db;

        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public UnitOfWork(ApplicationDbContext _db)
        {
            db = _db;
            Category = new CategoryRepository(db);
            Product = new ProductRepository(db);
        }
        public void Save()
        {
            db.SaveChanges();
        }
    }
}
