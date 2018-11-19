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
                .Skip(perPage * (pageNum - 1))
                .Take(perPage);
            return comments.Any() ? comments.ToList() : new List<Comment>();
        }

        public Comment AddComment(string blogName, string userName, string email, string comment)
        {
            var blogPost = _blogRepository.GetBlogByName(blogName);
            if (blogPost != null)
            {
                return _commentRepository.AddComment(new Comment()
                {
                    BlogItemId = blogPost.Id,
                    Content = comment,
                    ParentId = null,
                    SubCommentCount = 0,
                    Username = userName,
                    Email = email
                });
            }
            return null;
        }
    }
}
