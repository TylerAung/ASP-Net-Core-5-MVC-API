using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Contracts;
using LoggerService;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repository;
using CompanyEmployees.Extensions.ContentNegotiation;
using Microsoft.AspNetCore.Identity;
using Entities.Models;

namespace CompanyEmployees.Extensions
{
    public static class ServicesExtensions
    {

        public static void ConfigureCors(this IServiceCollection services)
            => services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
        });

        public static void ConfigureIISIntegration(this IServiceCollection services) 
            => services.Configure<IISOptions>(options => {

           });

        public static void ConfigureLoggerService(this IServiceCollection services) => 
            services.AddScoped<ILoggerManager, LoggerManager>();

        public static void ConfigureSqlContext(this IServiceCollection services,
            IConfiguration configuration) =>
            services.AddDbContext<RepositoryContext>(opts => 
            opts.UseSqlServer(configuration.GetConnectionString("sqlConnection"), b=>
            b.MigrationsAssembly("CompanyEmployees"))
            );
        public static void ConfigureRepositoryManager(this IServiceCollection services) 
            => services.AddScoped<IRepositoryManager, RepositoryManager>();

        public static IMvcBuilder AddCustomCSVFormatter(this IMvcBuilder builder) 
            => builder.AddMvcOptions(config 
                => config.OutputFormatters.Add(new CsvOutputFormatter()));

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentityCore<User>(o => {
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 10;
                o.User.RequireUniqueEmail = true;
            });
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
            builder.AddEntityFrameworkStores<RepositoryContext>().AddDefaultTokenProviders();
        }

    }
}
