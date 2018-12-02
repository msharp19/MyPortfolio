using PortfolioModel;
using Repositories;
using Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CommentService : ICommentService
    {
        private ICommentRepository _commentRepository;
        private IBlogPostRepository _blogRepository;

        public CommentService(MsportfolioEntities entities)
        {
            _commentRepository = new CommentRepository(entities);
            _blogRepository = new BlogPostRepository(entities);
        }

        public IList<Comment> GetComments(string blogPostName, int pageNum = 1, int perPage = 10){
            var comments = _commentRepository.GetComments(blogPostName)
                .Where(x => x.ParentId == null)
                .OrderByDescending(x => x.DateSent)
                .Skip(perPage * (pageNum - 1))
                .Take(perPage);
            return comments.Any() ? comments.ToList() : new List<Comment>();
        }

        public IList<Comment> GetCommentsByParentId(int parentId, int pageNum = 1, int perPage = 10)
        {
            var comments = _commentRepository.GetCommentsByParentId(parentId)
                .OrderByDescending(x => x.DateSent)
                .Skip(perPage * (pageNum - 1))
                .Take(perPage);
            return comments.Any() ? comments.ToList() : new List<Comment>();
        }

        public int GetTotalSubCommentCountByParentId(int parentId)
        {
            var commentCount = _commentRepository.GetCommentsByParentId(parentId).Count();
            return commentCount;
        }

        public int GetTotalCommentCount(string blogPostName)
        {
            var commentCount = _commentRepository.GetComments(blogPostName)
               .Where(x => x.ParentId == null).Count();
            return commentCount;
        }

        public Comment AddComment(string blogName, string userName, string email, string comment, int? replyId = null)
        {
            var blogPost = _blogRepository.GetBlogByName(blogName);
            if (blogPost != null)
            {
                var cmt = _commentRepository.AddComment(new Comment()
                {
                    BlogItemId = blogPost.Id,
                    Content = comment,
                    ParentId = replyId,
                    SubCommentCount = 0,
                    Username = userName,
                    Email = email,
                    DateSent= DateTime.Now
                });
                if (replyId != null) UpdateSubCommentCount(replyId.Value);
                _commentRepository.SaveChanges();
                return cmt;
            }
            return null;
        }

        public bool UpdateSubCommentCount(int replyId)
        {
            var comment = _commentRepository.GetComment(replyId);
            var count = comment.SubCommentCount;
            count++;
            comment.SubCommentCount = count;
            if (comment != null)
            {
                _commentRepository.UpdateComment(comment);
                return true;
            }
            return false;
        }

        public IList<Comment> GetCommentsByBlogId(int blogId, int pageNum = 1, int perPage = 10)
        {
            var comments = _commentRepository.GetCommentsByBlogId(blogId)
                .OrderByDescending(x => x.DateSent)
                .Skip(perPage * (pageNum - 1))
                .Take(perPage);
            return comments.Any() ? comments.ToList() : new List<Comment>();
        }

        public int GetTotalCommentCountByBlogId(int blogId)
        {
            return _commentRepository.GetCommentsByBlogId(blogId).Count();
        }
    }
}
