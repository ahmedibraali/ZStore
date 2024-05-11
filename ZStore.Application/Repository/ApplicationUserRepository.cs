using Microsoft.EntityFrameworkCore;
using ZStore.Data;
using ZStore.Application.Repository.IRepository;
using ZStore.Core;

namespace ZStore.Application.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext db;
        
        public ApplicationUserRepository(ApplicationDbContext _db):base(_db)
        {
            db = _db;
           
        }
       

        public void Update(ApplicationUser obj)
        {
            db.ApplicationUsers.Update(obj);
        }
    }
}
