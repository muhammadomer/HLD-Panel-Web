using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hld.WebApplication.Data;
using Hld.WebApplication.Models;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using static Hld.WebApplication.Models.PolicyTypes;

namespace Hld.WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            //var builder = new ConfigurationBuilder()
            //      .SetBasePath(env.ContentRootPath)
            //      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            //      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            //      .AddConfiguration(configuration)
            //      .AddEnvironmentVariables();

            Configuration = configuration;
           
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddDbContext<ApplicationDbContext>(options =>
             options.UseMySql(
                 Configuration.GetConnectionString("bb2HldNet")));
            services.AddIdentity<AppUser, IdentityRole>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddDistributedMemoryCache();
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(20);//You can set Time   
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver
                    = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("EditRole", policy => policy.RequireClaim(PolicyTypes.Teams.Manage));
                options.AddPolicy("DeleteRole", policy => policy.RequireClaim(PolicyTypes.Teams.AddRemove));
                
                options.AddPolicy("Access to Zinc Property Page", policy => { policy.RequireClaim("Access to Zinc Property Page", "Access to Zinc Property Page"); });
                options.AddPolicy("Access to dashboard", policy => { policy.RequireClaim("Access to dashboard", "Access to dashboard"); });
                options.AddPolicy("Access to Add or Edit Product", policy => { policy.RequireClaim("Access to Add or Edit Product", "Access to Add or Edit Product"); });
                options.AddPolicy("Access to BB property page", policy => { policy.RequireClaim("Access to BB property page", "Access to BB property page"); });
                options.AddPolicy("Access to update DS Qty", policy => { policy.RequireClaim("Access to update DS Qty", "Access to update DS Qty"); });
                options.AddPolicy("Access to Orders", policy => { policy.RequireClaim("Access to Orders", "Access to Orders"); });
                options.AddPolicy("Access to Order Tab", policy => { policy.RequireClaim("Access to Order Tab", "Access to Order Tab"); });
                options.AddPolicy("Access to Import PO", policy => { policy.RequireClaim("Access to Import PO", "Access to Import PO"); });
                options.AddPolicy("Access to PO", policy => { policy.RequireClaim("Access to PO", "Access to PO"); });
                options.AddPolicy("Access to Inventory", policy => { policy.RequireClaim("Access to Inventory", "Access to Inventory"); });
                options.AddPolicy("Access to Inventory Tab", policy => { policy.RequireClaim("Access to Inventory Tab", "Access to Inventory Tab"); });
                options.AddPolicy("Access to BulkUpdate Tab", policy => { policy.RequireClaim("Access to BulkUpdate Tab", "Access to BulkUpdate Tab"); });
                options.AddPolicy("Access to Setting Tab", policy => { policy.RequireClaim("Access to Setting Tab", "Access to Setting Tab"); });
                options.AddPolicy("Access to Admin Tab", policy => { policy.RequireClaim("Access to Admin Tab", "Access to Admin Tab"); });
                options.AddPolicy("Access to Product Link in PO", policy => { policy.RequireClaim("Access to Product Link in PO", "Access to Product Link in PO"); });
                options.AddPolicy("Access to P&L on Order list view page", policy => { policy.RequireClaim("Access to P&L on Order list view page", "Access to P&L on Order list view page"); });
                options.AddPolicy("Access to Shipment List", policy => { policy.RequireClaim("Access to Shipment List", "Access to Shipment List"); });
                options.AddPolicy("Access to Create & Edit Shipment", policy => { policy.RequireClaim("Access to Create & Edit Shipment", "Access to Create & Edit Shipment"); });
         
                options.AddPolicy("Access to View Shipment", policy => { policy.RequireClaim("Access to View Shipment", "Access to View Shipment"); });
                options.AddPolicy("Access to Receive Shipment", policy => { policy.RequireClaim("Access to Receive Shipment", "Access to Receive Shipment"); });
                options.AddPolicy("Case Pack managemant", policy => { policy.RequireClaim("Case Pack managemant", "Case Pack managemant"); });

                options.AddPolicy("ShippersPolicy", policy =>
                 policy.RequireRole("Shippers"));
                options.AddPolicy("VendorPolicy", policy =>
                 policy.RequireRole("Vendor"));

            });

            //services.ConfigureApplicationCookie(opts => opts.LoginPath = "/Authentication /save");
            services.ConfigureApplicationCookie(opts => opts.LoginPath = "/Authentication/Authenticate");

          

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
               
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseCookiePolicy();
           
            var config = this.Configuration.GetAWSLoggingConfigSection();

            loggerFactory.AddAWSProvider(config);
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    //template: "{controller=Home}/{action=Index}/{id?}");
                    template: "{controller=Authentication}/{action=Authenticate}/{id?}");
            });
        }
    }
}
