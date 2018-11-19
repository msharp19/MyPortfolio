using PortfolioSite.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortfolioSite.Models
{
    public class BlogItemModel
    {
        public CommentsModel CommentModel { get; set; }
        public BlogPost BlogPost { get; set; }

        public BlogItemModel(BlogPost blogPost)
        {
            BlogPost = blogPost;
        }
    }
}