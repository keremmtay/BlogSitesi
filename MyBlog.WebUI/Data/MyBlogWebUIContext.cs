using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyBlog.Entities.Concrete;

namespace MyBlog.WebUI.Data
{
    public class MyBlogWebUIContext : DbContext
    {
        public MyBlogWebUIContext (DbContextOptions<MyBlogWebUIContext> options)
            : base(options)
        {
        }

        public DbSet<MyBlog.Entities.Concrete.Category> Category { get; set; } = default!;
    }
}
