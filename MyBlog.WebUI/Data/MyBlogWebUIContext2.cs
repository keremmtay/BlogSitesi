using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyBlog.Entities.Concrete;

namespace MyBlog.WebUI.Data
{
    public class MyBlogWebUIContext2 : DbContext
    {
        public MyBlogWebUIContext2 (DbContextOptions<MyBlogWebUIContext2> options)
            : base(options)
        {
        }

        public DbSet<MyBlog.Entities.Concrete.BlogUser> BlogUser { get; set; } = default!;
    }
}
