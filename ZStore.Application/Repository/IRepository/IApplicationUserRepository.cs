using ZStore.Core;

namespace ZStore.Application.Repository.IRepository
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        void Update(ApplicationUser obj);

    }
}
