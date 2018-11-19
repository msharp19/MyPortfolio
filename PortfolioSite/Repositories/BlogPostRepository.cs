using PortfolioModel;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class BlogPostRepository : BaseRepository, IBlogPostRepository
    {
        public BlogPostRepository(MsportfolioEntities entities): base(entities)
        {}

        public BlogItem GetBlogByName(string blogName)
        {
            return _entities.BlogItems.FirstOrDefault(x => x.Name == blogName);
        }
    }
}
