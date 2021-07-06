using DataService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BlazorApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDirectoryBrowser();
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddRazorPages();
            services.AddServerSideBlazor().AddCircuitOptions(o =>
            {
                o.DetailedErrors = true;
            });
            //services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
            services.AddSingleton<IStockQueries, StockQueries>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();


            //var provider = new FileExtensionContentTypeProvider();
            //// Add new mappings
            //provider.Mappings[".myapp"] = "application/x-msdownload";
            //provider.Mappings[".htm3"] = "text/html";
            //provider.Mappings[".image"] = "image/png";
            //// Replace an existing mapping
            //provider.Mappings[".rtf"] = "application/x-msdownload";
            //// Remove MP4 videos.
            //provider.Mappings.Remove(".mp4");

            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(
            //        Path.Combine(env.WebRootPath, "photo")),
            //    RequestPath = "/photo",
            //    ContentTypeProvider = provider
            //});

            //app.UseDirectoryBrowser(new DirectoryBrowserOptions
            //{
            //    FileProvider = new PhysicalFileProvider(
            //        Path.Combine(env.WebRootPath, "photo")),
            //    RequestPath = "/photo"
            //});



            //    app.UseFileServer(new FileServerOptions
            //    {
            //        FileProvider = new PhysicalFileProvider(
            //Path.Combine(env.ContentRootPath, "photo")),
            //        RequestPath = "/photo",
            //        EnableDirectoryBrowsing = true
            //    });

            //app.UseFileServer(enableDirectoryBrowsing: true);UseStaticFiles
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
