﻿using AutoMapper;
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
            var config = new MapperConfiguration(cfg =>
            {
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

        public ActionResult PredictMnist(string imageRaw)
        {
            try
            {
                var image = ImageFunctions.Base64ToImage(imageRaw);
                image = ImageFunctions.Resize(image, 28,28);
                image.Save(@"C:\Users\waili\Documents\PortfolioProject\PortfolioSite\NetworkTester\training-set\test.jpg");
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
                    Data = new { Status = "Failure", Message = "Error running model" },
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet
                };
            }
        }
    }
}