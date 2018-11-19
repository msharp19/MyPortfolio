using PortfolioModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public abstract class BaseRepository
    {
        protected MsportfolioEntities _entities;

        public BaseRepository(MsportfolioEntities entities)
        {
            _entities = entities;
        }

        public int SaveChanges()
        {
            return _entities.SaveChanges();
        }
    }
}
