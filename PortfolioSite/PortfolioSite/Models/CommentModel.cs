using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortfolioSite.Models
{
    public class CommentModel
    {
        public int Id { get; set; }
        public int BlogItemId { get; set; }
        public string BlogItemName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime DateSent { get; set; }
        public string FormattedDateSent => DateSent.ToString("MMM dd yyyy");
        public string Content { get; set; }
        public int SubCommentCount { get; set; }
        public IList<CommentModel> SubComments { get; set; }
        public int Level { get; set; }
    }
}