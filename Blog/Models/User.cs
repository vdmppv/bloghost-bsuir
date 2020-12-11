using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Blog.Models
{
    public class User : IdentityUser
    {
        //public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; }
        public string SecondName { get; set; }
        
        public List<Article> Articles { get; set; }
    }
}
