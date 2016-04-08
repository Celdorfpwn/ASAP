using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IRepository : IDisposable
    {
        IQueryable<Entity> All<Entity>() where Entity :class;
        Entity Get<Entity>(params object[] ids) where Entity : class;
        void Add<Entity>(Entity entity) where Entity : class;
        void Edit<Entity>(Entity entity) where Entity : class;
        void Remove<Entity>(Entity entity) where Entity : class;
        void Save();

    }
}
