using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using insecure_bank_net.Dao;
using insecure_bank_net.Data;
using insecure_bank_net.Facade;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace insecure_bank_net
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        private IEnumerable<string> VulnerableAssemblies => new List<string> {"Sustainsys.Saml2"};

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

            var SqLiteConnection = new SqliteConnection(Configuration.GetConnectionString("DefaultConnection"));
            SqLiteConnection.Open();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(SqLiteConnection));

            services.AddScoped(typeof(IAccountDao), typeof(AccountDaoImpl));
            services.AddScoped(typeof(IActivityDao), typeof(ActivityDaoImpl));
            services.AddScoped(typeof(ICashAccountDao), typeof(CashAccountDaoImpl));
            services.AddScoped(typeof(ICreditAccountDao), typeof(CreditAccountDaoImpl));
            services.AddScoped(typeof(ITransferDao), typeof(TransferDaoImpl));

            services.AddScoped(typeof(IAuthenticationFacade), typeof(AuthenticationFacade));
            services.AddScoped(typeof(ITransferFacade), typeof(TransferFacadeImpl));
            services.AddScoped(typeof(IStorageFacade), typeof(StorageFacadeImpl));

            services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizePage("/Authentication/Login");
                options.Conventions.AuthorizeFolder("/User");
                options.Conventions.AddPageRoute("/User/Dashboard", "");
            });

            services.AddHttpContextAccessor();

            services.AddSession();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapRazorPages() );

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            PopulateDatabase(context);
            LoadAssemblies(VulnerableAssemblies);
        }

        private void PopulateDatabase(ApplicationDbContext context)
        {
            ApplicationDbContext.connection = context.Database.GetDbConnection();
            using var command = ApplicationDbContext.connection.CreateCommand();
            command.CommandText = "select count(*) from account";
            if (int.Parse(command.ExecuteScalar().ToString()!) == 0)
            {
                using var stream = GetType().Assembly.GetManifestResourceStream("insecure_bank_net.dataload.sql");
                using var reader = new StreamReader(stream!);
                var sql = reader.ReadToEnd();
                foreach (var item in sql.Split(";", StringSplitOptions.RemoveEmptyEntries))
                {
                    context.Database.ExecuteSqlRaw(item);
                }
            }
        }

        private void LoadAssemblies(IEnumerable<string> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                Assembly.Load(assembly);                
            }
        }
    }
}