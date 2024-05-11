using ZStore.Application.Repository.IRepository;
using ZStore.Core;
using ZStore.Data;

namespace ZStore.Application.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext db;

        public CompanyRepository(ApplicationDbContext _db) : base(_db)
        {
            db = _db;

        }


        public void Update(Company obj)
        {
            db.Companies.Update(obj);
        }
    }
}
