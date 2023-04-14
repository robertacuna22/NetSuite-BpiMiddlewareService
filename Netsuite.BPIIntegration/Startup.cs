using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Netsuite.Core;
using Netsuite.Core.Extensions;
using Netsuite.Services;
using Netsuite.Services.IContract;
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

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuratioBuilder = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", true)
                .AddDotNetDotEnvVariables(optional: true) //<-- Read a .env file and load it as a config eg Netsuite__ConnectionString=<connectionString>
                .AddDotNetEnvironmentVariables("Values__");

            configuration = configuratioBuilder.Build();

            builder.Services.AddOptions();
            builder.Services.Configure<AppSettings>(configuration.GetSection("Values"));

            builder.Services.AddTransient<IOrderPaymentSyncService, OrderPaymentSyncService>();

        }
    }
}
