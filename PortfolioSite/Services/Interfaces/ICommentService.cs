using PortfolioModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ICommentService
    {
        IList<Comment> GetComments(string blogPostName, int pageNum = 1, int perPage = 10);
        Comment AddComment(string blogName, string userName, string email, string comment);
    }
}
