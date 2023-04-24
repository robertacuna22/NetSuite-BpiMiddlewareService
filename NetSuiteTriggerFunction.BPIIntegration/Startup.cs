using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Netsuite.Core;
using Netsuite.Core.Extensions;
using Netsuite.Services;
using Netsuite.Services.IContract;

[assembly: FunctionsStartup(typeof(NetSuiteTriggerFunction.BPIIntegration.Startup))]
namespace NetSuiteTriggerFunction.BPIIntegration
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

            builder.Services.AddTransient<IOrderPaymentSyncService, OrderPaymentSyncService>()
                .AddTransient<IAzureStorage, AzureStorage>()
                .AddTransient<ISFTPSendingFileSyncService, SFTPSendingFileSyncService>();

        }
    }
}
