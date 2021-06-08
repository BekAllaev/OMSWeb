using Hangfire; //NuGet - Hangfire
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OMSWeb.Common.DataMock;
using OMSWeb.Data.Access.DAL;
using OMSWeb.Queries.Caching.Configuration;
using OMSWeb.Queries.Caching.Enums;
using OMSWeb.Queries.Caching.Services;
using System;

namespace OMSWeb
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
            var dataSource = Configuration["IsDataFake"];

            services.AddControllersWithViews().
                AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddSingleton<FakeDataGenerator>();

            if (dataSource is "Bogus")
                services.AddDbContext<NorthwindContext>(options => options.UseInMemoryDatabase("NorthwindContext"));
            else if(dataSource is "SqlServer")
                services.AddDbContext<NorthwindContext>(options => options.UseSqlServer(Configuration.GetConnectionString("SqlConnection")));

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OMSWeb", Version = "v1" });
            });

            services.Configure<CacheConfiguration>(Configuration.GetSection("CacheConfiguration"));
            services.AddMemoryCache();
            services.AddTransient<MemoryCacheService>();

            //Register delegate that returns cache service implementation thus no need in service registration
            //that depends on this cache service
            services.AddTransient<Func<CacheTech, ICacheService>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case CacheTech.Memory:
                        return serviceProvider.GetService<MemoryCacheService>();
                    default:
                        return serviceProvider.GetService<MemoryCacheService>();
                }
            });

            //services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("SqlConnection")));
            services.AddHangfireServer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OMSWeb v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseHangfireDashboard("/jobs");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
