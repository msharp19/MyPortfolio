using PortfolioModel;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class CommentRepository : BaseRepository, ICommentRepository
    {
        public CommentRepository(MsportfolioEntities entities) : base(entities)
        {}

        public Comment AddComment(Comment comment)
        {
            return _entities.Comments.Add(comment);
        }

        public IEnumerable<Comment> GetComments(string blogName)
        {
            return _entities.Comments.Where(x => x.BlogItem.Name == blogName);
        }

        public IEnumerable<Comment> GetCommentsByParentId(int parentId)
        {
            return _entities.Comments.Where(x => x.ParentId == parentId);
        }

        public Comment GetComment(int commentId)
        {
            return _entities.Comments.FirstOrDefault(x => x.Id == commentId);
        }

        public void UpdateComment(Comment comment)
        {
            _entities.Entry(comment).State = System.Data.Entity.EntityState.Modified;
        }

        public IEnumerable<Comment> GetCommentsByBlogId(int blogId){
            return _entities.Comments.Where(x => x.BlogItemId == blogId);
        }
    }
}
