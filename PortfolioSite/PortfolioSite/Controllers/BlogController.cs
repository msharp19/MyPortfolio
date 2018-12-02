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
using ANN;
using PortfolioSite.Utils;
using CNNV2;
using System.IO;
using System.Drawing;
using System.Web.Routing;

namespace PortfolioSite.Controllers
{
    [AllowAnonymous]
    public class BlogController : Controller
    {
        private MsportfolioEntities _entities = new MsportfolioEntities();
        private ICommentService _commentService;
        private IMapper _mapper;

        public BlogController()
        {
            _commentService = new CommentService(_entities);
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Comment, CommentModel>()
                .ForMember(dest => dest.SubComments, opt => opt.MapFrom(src => src.Comments1));
            });
            _mapper = config.CreateMapper();
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetMoreComments(int? parentId, int? blogId, int currentLevel, int pageNum = 1, int perPage = 2)
        {
            var model = CreateSubCommentsModel(parentId, blogId, currentLevel, pageNum, perPage);
            return PartialView("_SubComments", model);
        }

        private CommentsModel CreateSubCommentsModel(int? parentId, int? blogId, int currentLevel, int pageNum, int perPage)
        {
            IList<Comment> moreComments = null;
            int totalCommentCount = 0;
            if (parentId!=null)
            {
                moreComments = _commentService.GetCommentsByParentId(parentId.Value, pageNum, perPage);
                totalCommentCount = _commentService.GetTotalSubCommentCountByParentId(parentId.Value);
            }
            else
            {
                moreComments = _commentService.GetCommentsByBlogId(blogId.Value);
                totalCommentCount = _commentService.GetTotalCommentCountByBlogId(blogId.Value);
            }
            var totalPagesModulus = totalCommentCount % perPage;
            var totalPages = (totalCommentCount / perPage) + (totalPagesModulus >= 1 ? 1 : 0);
            var model = _mapper.Map<IList<CommentModel>>(moreComments);
            for (int i = 0; i < model.Count; i++) model[i].Level = currentLevel + 1;
            return new CommentsModel()
            {
                Comments = model,
                TotalCommentCount = totalCommentCount,
                PageNum = pageNum,
                ParentId = parentId ?? -1,
                Level = currentLevel,
                PerPage = perPage,
                TotalPages = totalPages
            };
        }

        [HttpPost]
        public ActionResult AddComment(BlogPost blogName, string userName, string email, string comment, int? replyId)
        {
            var isValid = this.IsCaptchaValid("Captcha is not valid");
            if (isValid)
            {
                var added = _commentService.AddComment(blogName.ToString(), userName, email, comment, replyId);
                if (added != null)
                {
                    var dictionary = new RouteValueDictionary() { };
                    dictionary.Add("blogPost", blogName);
                    return new JsonResult()
                    {
                        Data = new { Status = "Success", Message = "Comment added.", Url = Url.Action("BlogPost", "Blog", dictionary, Request.Url.Scheme) },
                        JsonRequestBehavior = JsonRequestBehavior.DenyGet
                    };
                }
            }

            var routeValues = new RouteValueDictionary();
            routeValues.Add("blogPost", blogName.ToString());
            routeValues.Add("userName", userName);
            routeValues.Add("email", email);
            routeValues.Add("comment", comment);
            return new JsonResult()
            {
                Data = new { Status = "Failure", Message = "Captcha wasn't correct!", Url = Url.Action("BlogPost", "Blog", routeValues, Request.Url.Scheme) },
                JsonRequestBehavior = JsonRequestBehavior.DenyGet
            };
        }

        [HttpGet]
        public ActionResult BlogPost(BlogPost blogPost, string userName, string email, string comment)
        {
            var model = new BlogItemModel(blogPost);
            var commentModel = GetCommentModelForBlog(blogPost);
            commentModel.Username = userName;
            commentModel.Email = email;
            commentModel.Comment = comment;
            model.CommentModel = commentModel;
            model.BlogPost = blogPost;
            return View(blogPost.ToString(), model);
        }

        private CommentsModel GetCommentModelForBlog(BlogPost blogPost)
        {
            var model = new CommentsModel();
            var comments = _commentService.GetComments(blogPost.ToString());
            model.Comments = _mapper.Map<IList<CommentModel>>(comments);
            model.CommentCount = model.Comments.Count();
            model.TotalCommentCount = _commentService.GetTotalCommentCount(blogPost.ToString());
            return model;
        }

        [HttpPost]
        public ActionResult RunModel(string connectionId)
        {
            try
            {
                if (!string.IsNullOrEmpty(connectionId))
                {
                    //Get the raw data
                    Functions.SendProgress(connectionId, "Loading Data.", false);
                    var rawData = NetworkFactory.GetInputData();
                    var ideals = NetworkFactory.GetIdealValues();
                    Functions.SendProgress(connectionId, "Loading Complete.", false);

                    Functions.SendProgress(connectionId, "Formatting Data.", false);
                    //Format the data so it can be used in a neural network - all columns will have a value between 0-1 (floating point)
                    var formattedData = NetworkFactory.FormatInputValues(rawData);
                    Functions.SendProgress(connectionId, "Formatting Complete.", false);

                    Functions.SendProgress(connectionId, "Creating Network.", false);
                    //Create Network
                    var network = NetworkFactory.CreateNetwork(formattedData, ideals, 40);
                    Functions.SendProgress(connectionId, "Network Created.", false);

                    Functions.SendProgress(connectionId, "Training Network.", false);
                    //Train network
                    var trainedNetwork = NetworkFactory.TrainNetwork(network, formattedData, ideals, 20000,
                        0.001, connectionId, Functions.SendProgress);
                    Functions.SendProgress(connectionId, "Training Complete.", false);

                    Functions.SendProgress(connectionId, "Testing Network.", false);
                    //Finally test the network
                    NetworkFactory.TestNetwork(network, formattedData, ideals, connectionId, Functions.SendProgress);
                    Functions.SendProgress(connectionId, "Testing Complete.", false);

                    //Return status
                    network = null;
                    trainedNetwork = null;
                    return new JsonResult()
                    {
                        Data = new { Status = "Success", Message = "Complete" },
                        JsonRequestBehavior = JsonRequestBehavior.DenyGet
                    };
                }
                throw new Exception("Empty Connection ID");
            }
            catch (Exception ex)
            {
                return new JsonResult()
                {
                    Data = new { Status = "Failure", Message = "Error running model" },
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet
                };
            }
        }

        [HttpPost]
        public ActionResult PredictMnist(string imageRaw)
        {
            try
            {
                var image = ImageFunctions.Base64ToImage(imageRaw);
                image = ImageFunctions.Resize(image, 28,28);
                var directory = AppDomain.CurrentDomain.BaseDirectory;
                var path = Path.Combine(directory, "CNNModel");
                string outputModelPath = Path.Combine(path, "model.mod");
                var label = CNNManager.TestItems(outputModelPath, image);
                return new JsonResult()
                {
                    Data = new { Status = "Success", Message = "Complete", Label = label },
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet
                };
            }
            catch (Exception ex)
            {
                return new JsonResult()
                {
                    Data = new { Status = "Failure", Message = "Error making prediction." },
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet
                };
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_entities != null)
                {
                    _entities.Dispose();
                    _entities = null;
                }
                _commentService = null;
            }
            base.Dispose(disposing);
        }
    }
}