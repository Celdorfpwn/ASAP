using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository;

namespace EntityFrameworkDb
{
    public class EntityFrameworkRepository : IRepository
    {

        private DatabaseContext _database { get;set; }

        public EntityFrameworkRepository()
        {
            _database = new DatabaseContext();
        }

        public void Add<Entity>(Entity entity) where Entity : class
        {
            _database.Set<Entity>().Add(entity);
        }

        public IQueryable<Entity> All<Entity>() where Entity : class
        {
            return _database.Set<Entity>().AsQueryable();
        }

        public void Edit<Entity>(Entity entity) where Entity : class
        {
            _database.Entry<Entity>(entity).State = EntityState.Modified;
        }

        public Entity Get<Entity>(params object[] ids) where Entity : class
        {
            return _database.Set<Entity>().Find(ids);
        }

        public void Remove<Entity>(Entity entity) where Entity : class
        {
            _database.Set<Entity>().Remove(entity);
        }

        public void Save()
        {
            _database.SaveChanges();
        }

        public void Dispose()
        {
            _database.Dispose();
        }
    }
}
