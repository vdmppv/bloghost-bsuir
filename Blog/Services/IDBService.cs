using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models;

namespace Blog.Services
{
    public interface IDBService
    {
        Article GetPost(string id);
        Article GetPostBySlug(string slug);
        List<Article> GetAllPosts();
        List<Message> GetAllMessages();
        void RemovePost(string id);
        void AddPost(Article article);
        void UpdatePost(Article article);
        Task<bool> SaveChangesAsync();
    }
}
