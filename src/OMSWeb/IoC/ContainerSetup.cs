using Hangfire; //NuGet - Hangfire
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OMSWeb.Common.DataMock;
using OMSWeb.Data.Access.DAL;
using OMSWeb.Services.Maps;
using OMSWeb.Queries.Caching.Configuration;
using OMSWeb.Queries.Caching.Enums;
using OMSWeb.Queries.Caching.Services;
using OMSWeb.Queries.Queries;
using OMSWeb.Services.Pagination;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace OMSWeb.IoC
{
    /// <summary>
    /// Methods that add/configure dependencies or service
    /// Motivation for this class is to remove addition and configuration of dependencies from <see cref="Startup"/> class
    /// </summary>
    public static class ContainerSetup
    {
        public static void Setup(IServiceCollection services, IConfiguration configuration)
        {
            AddUoW(services, configuration);
            AddQueryProcessors(services);
            AddCaching(services, configuration);
            AddPagination(services);
            ConfigureAutoMapper(services);
        }

        private static void AddUoW(IServiceCollection services, IConfiguration configuration)
        {
            var dataSource = configuration["DataSource"];

            services.AddSingleton<FakeDataGenerator>();

            if (dataSource is "Bogus")
                services.AddDbContext<NorthwindContext>(options => options.UseInMemoryDatabase("NorthwindContext"), ServiceLifetime.Singleton);
            else if (dataSource is "SqlServer")
                services.AddDbContext<NorthwindContext>(options => options.UseSqlServer(configuration.GetConnectionString("SqlConnection")), ServiceLifetime.Singleton);

            services.AddScoped<IUnitOfWork>(ctx => new EFUnitOfWork(ctx.GetRequiredService<NorthwindContext>()));
        }

        private static void AddQueryProcessors(IServiceCollection services)
        {
            var exampleProcessorType = typeof(CategoryQueryProcessor);
            var types = (from t in exampleProcessorType.GetTypeInfo().Assembly.GetTypes()
                         where t.Namespace == exampleProcessorType.Namespace
                             && t.GetTypeInfo().IsClass
                             && t.GetTypeInfo().GetCustomAttribute<CompilerGeneratedAttribute>() == null
                         select t).ToArray();

            foreach (var type in types)
            {
                var interfaceQ = type.GetTypeInfo().GetInterfaces().First();
                services.AddScoped(interfaceQ, type);
            }
        }

        private static void AddCaching(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CacheConfiguration>(configuration.GetSection("CacheConfiguration"));
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

            services.AddHangfire(x => x.UseSqlServerStorage(configuration.GetConnectionString("SqlConnection")));
            services.AddHangfireServer();
        }

        private static void AddPagination(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IUriService>(o =>
            {
                var accessor = o.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext.Request;
                var uri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());
                return new UriService(uri);
            });
        }

        private static void ConfigureAutoMapper(IServiceCollection services)
        {
            var mapperConfig = AutoMapperConfigurator.Configure();
            var mapper = mapperConfig.CreateMapper();
            services.AddSingleton(x => mapper);
            services.AddTransient<IAutoMapper, AutoMapperAdapter>();
        }
    }
}
