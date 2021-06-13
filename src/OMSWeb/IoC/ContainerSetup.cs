using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OMSWeb.Common.DataMock;
using OMSWeb.Data.Access.DAL;
using OMSWeb.Queries.Caching.Configuration;
using OMSWeb.Queries.Caching.Enums;
using OMSWeb.Queries.Caching.Services;
using OMSWeb.Queries.Queries;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace OMSWeb.IoC
{
    public static class ContainerSetup
    {
        public static void Setup(IServiceCollection services, IConfiguration configuration)
        {
            AddUoW(services, configuration);
            AddQueryProcessors(services);
            AddCaching(services, configuration);
        }

        private static void AddUoW(IServiceCollection services, IConfiguration configuration)
        {
            var dataSource = configuration["DataSource"];

            services.AddSingleton<FakeDataGenerator>();

            if (dataSource is "Bogus")
                services.AddDbContext<NorthwindContext>(options => options.UseInMemoryDatabase("NorthwindContext"));
            else if (dataSource is "SqlServer")
                services.AddDbContext<NorthwindContext>(options => options.UseSqlServer(configuration.GetConnectionString("SqlConnection")));

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

            //services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("SqlConnection")));
            services.AddHangfireServer();
        }
    }
}
