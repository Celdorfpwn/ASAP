using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkDb
{
    public class EntityFrameworkFactory : IRepositoryFactory
    {
        public IRepository NewRepository
        {
            get { return new EntityFrameworkRepository(); }
        }
    }
}
