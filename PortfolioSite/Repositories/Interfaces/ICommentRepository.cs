using PortfolioModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface ICommentRepository
    {
        Comment AddComment(Comment comment);
        IEnumerable<Comment> GetComments(string blogName);
        IEnumerable<Comment> GetCommentsByParentId(int parentId);
        Comment GetComment(int commentId);
    }
}
