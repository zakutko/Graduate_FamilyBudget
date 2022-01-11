using Eir.Services;
using FamilyBudget.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyBudget
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _currentEnvironment = env;
        }

        private readonly IHostEnvironment _currentEnvironment;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = "";
            if (_currentEnvironment.IsDevelopment())
            {
                connectionString = "Host=localhost;Port=5432;Database=FamilyBudgetDB;Username=postgres;Password=123";
            } else
            {
                connectionString = "Host=rc1b-3m4lexln73k8vtbf.mdb.yandexcloud.net;Port=6432;SSL Mode=Require;Database=FamilyBudgetDB;Username=User_Id;Password=User_Password;Trust Server Certificate=true;";
                connectionString = connectionString.Replace("User_Id", Configuration["UserId"]);
                connectionString = connectionString.Replace("User_Password", Configuration["Password"]);
            }

            services.AddEntityFrameworkNpgsql().AddDbContext<ApplicationDbContext>((sp, opt) =>
                {
                    opt.UseNpgsql(connectionString, b => b.RemoteCertificateValidationCallback(SSL.RemoteCertificateValidation));
                    if (_currentEnvironment.IsDevelopment())
                    {
                        opt.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddDebug()));
                    }
                    else
                    {
                        opt.UseInternalServiceProvider(sp);
                    }
                });

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.Password.RequiredLength = 2;   // минимальная длина
                options.Password.RequireNonAlphanumeric = false;   // требуются ли не алфавитно-цифровые символы
                options.Password.RequireLowercase = false; // требуются ли символы в нижнем регистре
                options.Password.RequireUppercase = false; // требуются ли символы в верхнем регистре
                options.Password.RequireDigit = false; // требуются ли цифры
                options.SignIn.RequireConfirmedAccount = false; // обязательно нужно подтвердить аккаунт по почте
            }).AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
