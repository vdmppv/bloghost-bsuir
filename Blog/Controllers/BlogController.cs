using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Models;
using Blog.ViewModels;
using Blog.Services;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;

namespace Blog.Controllers
{
    public class BlogController : Controller
    {
        //private readonly IBlogService _blog;
        private readonly IDBService _repo;
        private readonly IConfiguration _config;
        private readonly EmailService _emailService;


        public BlogController(IDBService repo, IConfiguration config, EmailService emailService)
        {
            _repo = repo;
            _config = config;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var articles = _repo.GetAllPosts();
            return View(articles);
        }


        [Route("/get")]
        [HttpGet]
        public IActionResult GetPosts()
        {
            var articles = _repo.GetAllPosts();
            return Ok(articles);
        }

        [Route("/blog/edit/")]
        [Authorize(Roles = "admin, moder")]
        public ActionResult New()
        {
            return View("Edit", new Article());
        }

        [Route("/blog/{slug?}")]
        [Authorize(Roles = "admin, moder"), HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(Article article)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", new Article());
            }

            var existing = _repo.GetPost(article.Id);

            if (existing == null)
            {
                _repo.AddPost(article);
                // BackgroundJob.Enqueue(() => emailService.SendEmailAsync(_config["user:email"], "You successfully added a post to your blog", "Article: " + article.Excerpt, _config));
            }
            else
            {
                existing.Title = article.Title.Trim();
                existing.Slug = !string.IsNullOrWhiteSpace(article.Slug) ? article.Slug.Trim() : Article.CreateSlug(article.Title);
                existing.Content = article.Content.Trim();
                existing.Excerpt = article.Excerpt.Trim();

                _repo.UpdatePost(existing);
            }
            await _repo.SaveChangesAsync();

            return Redirect(article.GetLink());

        }

        [Route("/blog/edit/{id?}")]
        [HttpGet, Authorize(Roles = "admin, moder")]
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View(new Article());
            }

            var article = _repo.GetPost(id);

            if (article != null)
            {
                _repo.SaveChangesAsync();
                return View(article);
            }

            return NotFound();
        }


        [Route("/blog/{slug?}")]
        public IActionResult Details(string slug)
        {
            var article = _repo.GetPostBySlug(slug);

            if (article == null)
                return NotFound();

            return View("Post", article);
        }

        [Route("/blog/deletepost/{id}")]
        [HttpPost, Authorize(Roles = "admin, moder"), AutoValidateAntiforgeryToken]
        public IActionResult DeletePost(string id)
        {
            var existing = _repo.GetPost(id);

            if (existing != null)
            {
                _repo.RemovePost(existing.Id);
                _repo.SaveChangesAsync();
                return Redirect("/");
            }

            return NotFound();
        }

        [Route("/blog/comment/{articleId}")]
        [HttpPost]
        public async Task<IActionResult> AddComment(string articleId, Comment comment)
        {
            var article = _repo.GetPost(articleId);

            if (!ModelState.IsValid)
            {
                return View("Post", new Article());
            }

            if (article == null)
            {
                return NotFound();
            }

            comment.ArticleId = articleId;
            comment.IsAdmin = User.Identity.IsAuthenticated;
            comment.Content = comment.Content.Trim();

            if (User.Identity.IsAuthenticated)
            {
                if (User.Identity.Name.Trim() == "admin")
                {
                    comment.Author = "Vadim Popov".Trim();
                    comment.Email = "vadim.popov00@bk.ru".Trim();
                }
                comment.Author = User.Identity.Name.Trim();
                comment.Email = "vadasdfoasdfov@bk.ru".Trim();
            }
            else
            {
                comment.Author = comment.Author.Trim();
                comment.Email = comment.Email.Trim();
            }
            
            await _emailService.SendEmailAsync(_config["user:email"], "New comment from: " + comment.Author,
                                                                            "Comment: " + comment.Content + '\n' + "Article: " + articleId);                                                              

            article.Comments.Add(comment);
            _repo.UpdatePost(article);
            await _repo.SaveChangesAsync();
            return Redirect(article.GetLink() + "#" + comment.Id);
        }



        [Route("/blog/comment/{articleId}/{commentId}")]
        [Authorize(Roles = "admin, moder")]
        public IActionResult DeleteComment(string articleId, string commentId)
        {
            var article = _repo.GetPost(articleId);

            if (article == null)
            {
                return NotFound();
            }

            var comment = article.Comments.FirstOrDefault(c => c.Id.Equals(commentId, StringComparison.OrdinalIgnoreCase));

            if (comment == null)
            {
                return NotFound();
            }

            article.Comments.Remove(comment);
            _repo.UpdatePost(article);
            _repo.SaveChangesAsync();

            return Redirect(article.GetLink() + "#comments");
        }
    }
}