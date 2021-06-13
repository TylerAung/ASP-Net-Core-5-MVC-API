using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompanyEmployees.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using NLog;
using System.IO;
using Contracts;
using CompanyEmployees.Extensions.ActionFilters;

namespace CompanyEmployees
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {//Used to configure services, which is a reusable part of code that adds some functionality to application

            services.ConfigureCors(); //Connecting to Extension Method for Cors
            services.ConfigureIISIntegration(); //Connecting to Extension Method for IIS
            services.ConfigureLoggerService();
            services.ConfigureSqlContext(Configuration);
            services.ConfigureRepositoryManager();
            services.AddAutoMapper(typeof(Startup));
            services.AddScoped<ValidationFilterAttribute>(); //Action Filter for model Validation
            services.AddScoped<ValidateCompanyExistsAttribute>(); //Action Filter with Dependency Injection looking up ID (COY)
            services.AddScoped<ValidateEmployeeForCompanyExistsAttribute>(); //Action Filter with Dependency Injection looking up ID (Emp)
            services.AddAuthentication(); 
            services.ConfigureIdentity();

            services.Configure<ApiBehaviorOptions>(options => {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddControllers(config => {
                config.RespectBrowserAcceptHeader = true;
                config.ReturnHttpNotAcceptable = true;//Restricting Media Types
            }).AddNewtonsoftJson()
              .AddXmlDataContractSerializerFormatters()
              .AddCustomCSVFormatter();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CompanyEmployees", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerManager logger)
        {//Add different middleware components to app request pipeline
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CompanyEmployees v1"));
            }
            else
            {
                app.UseHsts();
            }

            app.ConfigureExceptionHandler(logger);
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("CorsPolicy");
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            app.UseRouting();

            app.UseAuthentication(); 
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=home}/{action=Index}/{id?}");
            });
        }
    }
}

//endpoints.MapControllers();