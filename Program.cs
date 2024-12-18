using Microsoft.EntityFrameworkCore;
using StarterKit.Models;
using StarterKit.Services;
using Microsoft.Extensions.Logging;

namespace StarterKit
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Specify the URLs for the application (only HTTP)
            builder.WebHost.UseUrls("http://localhost:3000");

            // Add logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            builder.Services.AddControllersWithViews();
            builder.Services.AddDistributedMemoryCache();
            
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSession(options => 
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true; 
                options.Cookie.IsEssential = true; 
            });

            // Register services
            builder.Services.AddScoped<ILoginService, LoginService>();
            builder.Services.AddScoped<IEventService, EventService>();

            // Database context
            builder.Services.AddDbContext<DatabaseContext>(
                options => options.UseSqlite(builder.Configuration.GetConnectionString("SqlLiteDb")));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Remove HTTPS redirection
            // app.UseHttpsRedirection(); // Comment this line if you want to avoid HTTPS
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Ensure database is created
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                context.Database.EnsureCreated();
            }

            app.Run();
        }
    }
}