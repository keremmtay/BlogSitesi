using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyBlog.Entities.Concrete;

namespace MyBlog.WebUI.Data
{
    public class MyBlogWebUIContext1 : DbContext
    {
        public MyBlogWebUIContext1 (DbContextOptions<MyBlogWebUIContext1> options)
            : base(options)
        {
        }

        public DbSet<MyBlog.Entities.Concrete.Note> Note { get; set; } = default!;
    }
}
