using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OMSWeb;
using OMSWeb.Common.DataMock;
using OMSWeb.Data.Access.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//https://exceptionnotfound.net/ef-core-inmemory-asp-net-core-store-database/
var host = CreateHostBuilder(args).Build();
bool isDataFake;

var config = host.Services.GetRequiredService<IConfiguration>();

var dataSource = config["DataSource"];

if (dataSource is "SqlServer")
    isDataFake = false;
else
    isDataFake = true;

if (isDataFake)
{
    //1. Find the service layer within our scope.
    using (var scope = host.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        //2. Get the instance of NorthwindContext 
        var context = services.GetRequiredService<NorthwindContext>();
        var generator = services.GetRequiredService<FakeDataGenerator>();

        //3. Call the DataGenerator to create sample data
        FakeDataSeeder.Initialize(context, generator);
    }
}

//Continue to run the application
host.Run();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
