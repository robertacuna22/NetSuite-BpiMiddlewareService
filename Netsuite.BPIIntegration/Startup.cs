using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: FunctionsStartup(typeof(Netsuite.BPIIntegration.Startup))]
namespace Netsuite.BPIIntegration
{
    public class Startup : FunctionsStartup
    {
        private IConfigurationRoot configuration;
        private string _connectionString = "";

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuratioBuilder = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", true);

            var serviceProvider = builder.Services.BuildServiceProvider();
            configuration = configuratioBuilder.Build();

            _connectionString = configuration.GetValue<string>("Values:BlobConnectionString");

            builder.Services.AddOptions();
        }
    }
}
