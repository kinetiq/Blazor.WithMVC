using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Demo.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            // Add Razor Pages, which is the typical way to use Blazor.
            services.AddRazorPages();

            // Add Blazor. Note that no extra packages are needed. This is available as part of Core as of 3.0
            services.AddServerSideBlazor();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                // Adds the hub, which is how Blazor components connect to the server via SignalR. 
                endpoints.MapBlazorHub(); 

                // Creates a low-priority route to the _Host file. In effect, all traffic that isn't explicitly routed
                // elsewhere will go to _Host, where Blazor's internal routing will take over. This means your existing
                // MVC routes will continue to work, *except* the default Route, which is a problem and we will work
                // around that in the next commit.
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
