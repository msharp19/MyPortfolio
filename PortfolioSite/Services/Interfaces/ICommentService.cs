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
        IList<Comment> GetComments(string blogPostName, int pageNum = 1, int perPage = 20);
        Comment AddComment(string blogName, string userName, string email, string comment, int? replyId);
        bool UpdateSubCommentCount(int replyId);
        int GetTotalCommentCount(string blogPostName);
        IList<Comment> GetCommentsByParentId(int parentId, int pageNum = 1, int perPage = 10);
        int GetTotalSubCommentCountByParentId(int parentId);
        IList<Comment> GetCommentsByBlogId(int blogId, int pageNum = 1, int perPage = 10);
        int GetTotalCommentCountByBlogId(int blogId);
    }
}
