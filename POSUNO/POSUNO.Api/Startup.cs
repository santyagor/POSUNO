using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using POSUNO.Api.Data;
using POSUNO.Api.Data.Entities;
using POSUNO.Api.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSUNO.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddIdentity<User, IdentityRole>(cfg =>
             {
                 cfg.User.RequireUniqueEmail = true;
                 cfg.Password.RequireDigit = false;
                 cfg.Password.RequiredUniqueChars = 0;
                 cfg.Password.RequireLowercase = false;
                 cfg.Password.RequireNonAlphanumeric = false;
                 cfg.Password.RequireUppercase = false;
             }).AddEntityFrameworkStores<DataContext>();
            services.AddDbContext<DataContext>(cfg =>
            {
                cfg.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            services
                .AddAuthentication()
                .AddCookie()
                .AddJwtBearer(cfg =>
                {
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = Configuration["Tokens:Issuer"],
                        ValidAudience = Configuration["Tokens:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]))
                    };
                });

            services.AddTransient<SeedDb>();//se usa una sola vez
            services.AddScoped<IUserHelper, UserHelper>(); // instancia cada vez q se necesita
            //services.AddSingleton  mantiene una unica instancia en memoria todo el tiempo
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
            });
        }
    }
}
