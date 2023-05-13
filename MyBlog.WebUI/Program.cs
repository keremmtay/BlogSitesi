using MyBlog.BusinessLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyBlog.WebUI.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MyBlogWebUIContext2>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyBlogWebUIContext2") ?? throw new InvalidOperationException("Connection string 'MyBlogWebUIContext2' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();
// Session
builder.Services.AddSession(option => { option.IdleTimeout = TimeSpan.FromMinutes(30); });

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
