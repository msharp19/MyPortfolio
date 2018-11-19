using AutoMapper;
using PortfolioModel;
using PortfolioSite.Enums;
using PortfolioSite.Models;
using Services;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CaptchaMvc.HtmlHelpers;

namespace PortfolioSite.Controllers
{
    public class BlogController : Controller
    {

        private MsportfolioEntities entities = new MsportfolioEntities();
        private ICommentService _commentService;
        private IMapper _mapper;

        public BlogController()
        {
            _commentService = new CommentService(entities);
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Comment, CommentModel>();
            });
            _mapper = config.CreateMapper();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddComment(string blogName, string userName, string email, string comment)
        {
            var isValid = this.IsCaptchaValid("Captcha is not valid");
            if (isValid)
            {
                var added = _commentService.AddComment(blogName, userName, email, comment);
                if (added != null) return new JsonResult()
                {
                    Data = new { Status = "Success", Message = "You any good at maths?" },
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet
                };
            }
            return new JsonResult()
            {
                Data = new { Status = "Failure", Message = "You any good at maths?" },
                JsonRequestBehavior = JsonRequestBehavior.DenyGet
            };
        }

        public ActionResult BlogPost(BlogPost blogPost)
        {
            var model = new BlogItemModel(blogPost);
            var commentModel = GetCommentModelForBlog(blogPost);
            model.CommentModel = commentModel;
            model.BlogPost = blogPost;
            return View(blogPost.ToString(), model);
        }

        private CommentsModel GetCommentModelForBlog(BlogPost blogPost)
        {
            var model = new CommentsModel();
            var comments = _commentService.GetComments(blogPost.ToString());
            model.Comments = _mapper.Map<IList<CommentModel>>(comments);
            return model;
        }
    }
}