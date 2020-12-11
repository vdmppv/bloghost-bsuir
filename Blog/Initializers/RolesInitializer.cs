using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Initializers
{
    public class RolesInitializer
    {
        ApplicationDbContext _ctx;

        public RolesInitializer(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        //var roleManager = new RoleManager<IdentityRole>(new RoleStoreBase<IdentityRole>);
    }
}
