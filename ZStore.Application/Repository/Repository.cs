using Microsoft.EntityFrameworkCore;
using ZStore.Data;
using ZStore.Application.Repository.IRepository;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace ZStore.Application.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext db;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext _db)
        {
            db = _db;
            dbSet = db.Set<T>();
            db.Products.Include(u => u.Category).Include(u=>u.CategoryId);
        }
        public void Add(T entity)
        {
            dbSet.Add(entity);

        }

        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<T> query;
            if (tracked) 
                query = dbSet;
            else
                query = dbSet.AsNoTracking();
            
            query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var incudeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incudeProp);
                }
            }
            return query.FirstOrDefault();


        }
        //Category , CoverType 
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if(filter != null)
                query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach(var incudeProp in includeProperties
                    .Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries ))
                {
                    query = query.Include(incudeProp);
                }
            }
            return query.ToList();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }
    }
}
