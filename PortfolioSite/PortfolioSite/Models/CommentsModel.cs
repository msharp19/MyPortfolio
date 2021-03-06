﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortfolioSite.Models
{
    public class CommentsModel
    {

        public int CommentCount { get; set; }
        public IList<CommentModel> Comments {get;set;}
        public string Username { get; set; }
        public string Email { get; set; }
        public string Comment { get; set; }
        public int TotalCommentCount { get; set; }
        public int ParentId { get; set; }
        public int Level { get; set; }
        public int PageNum { get; set; }
        public int TotalPages { get; set; }
        public int PerPage { get; set; }

        public CommentsModel()
        {
            Comments = new List<CommentModel>();
        }
    }
}