
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GreatShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreatShop.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        /********* Every time a new model is created remember to add it below! ***********/
        public DbSet<Category> Category { get; set; }
      
        public DbSet<Product> Product { get; set; }

        public DbSet<AppUser> AppUser { get; set; }

    }
}