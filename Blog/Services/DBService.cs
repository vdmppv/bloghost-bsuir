using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Blog.Models;
using Blog.Data;

namespace Blog.Services
{
    public class DBService : IDBService
    {
        private ApplicationDbContext _ctx;
        public DBService(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public void AddPost(Article article)
        {
            _ctx.Articles.Add(article);
        }

        public List<Article> GetAllPosts()
        {
            return _ctx.Articles.ToList();
        }

        public List<Message> GetAllMessages()
        {
            return _ctx.Messages.ToList();
        }

        public Article GetPost(string id)
        {
            return _ctx.Articles
                .Include(p => p.Comments)
                .FirstOrDefault(p => p.Id == id);
        }

        public Article GetPostBySlug(string slug)
        {
            return _ctx.Articles
                .Include(p => p.Comments)
                .FirstOrDefault(p => p.Slug == slug);
        }

        public void RemovePost(string id)
        {
            _ctx.Articles.Remove(GetPost(id));
        }

        public void UpdatePost(Article article)
        {
            _ctx.Articles.Update(article);
        }

        public async Task<bool> SaveChangesAsync()
        {
            if (await _ctx.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }
    }
}
