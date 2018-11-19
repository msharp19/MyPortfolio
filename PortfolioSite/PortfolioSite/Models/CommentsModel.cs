using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortfolioSite.Models
{
    public class CommentsModel
    {
        public int CommentCount { get; set; }
        public IList<CommentModel> Comments {get;set;}
        public CommentsModel()
        {
            Comments = new List<CommentModel>();
        }
    }
}