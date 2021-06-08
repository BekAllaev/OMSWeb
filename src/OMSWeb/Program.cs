using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OMSWeb.Common.DataMock;
using OMSWeb.Data.Access.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMSWeb
{
    public class Program
    {
        //https://exceptionnotfound.net/ef-core-inmemory-asp-net-core-store-database/
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var config = host.Services.GetRequiredService<IConfiguration>();

            bool isDataFake = bool.Parse(config["IsDataFake"]);

            if (isDataFake)
            {
                //1. Find the service layer within our scope.
                using (var scope = host.Services.CreateScope())
                {
                    //2. Get the instance of BoardGamesDBContext in our services layer
                    var services = scope.ServiceProvider;

                    var context = services.GetRequiredService<NorthwindContext>();
                    var generator = services.GetRequiredService<FakeDataGenerator>();

                    //3. Call the DataGenerator to create sample data
                    FakeDataSeeder.Initialize(context, generator);
                }
            }

            //Continue to run the application
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
