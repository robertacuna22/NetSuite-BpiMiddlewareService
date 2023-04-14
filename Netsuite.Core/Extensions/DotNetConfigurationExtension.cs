using Netsuite.Core.ConfigurationProviders;
using Microsoft.Extensions.Configuration;

namespace Netsuite.Core.Extensions
{
    public static class DotNetConfigurationExtension
    {
        public static IConfigurationBuilder AddDotNetEnvironmentVariables(this IConfigurationBuilder config, string envVarPrefix)
        { 
            config.Add(new DotNetEnvironmentVariableConfigurationSource(envVarPrefix));

            return config;
        }

        public static IConfigurationBuilder AddDotNetDotEnvVariables(this IConfigurationBuilder config, FileInfo file, bool optional = false)
        {
            config.Add(new DotNetDotEnvConfigurationSource(file, optional));

            return config;
        }

        public static IConfigurationBuilder AddDotNetDotEnvVariables(this IConfigurationBuilder config, bool optional = false)
        {
            config.Add(new DotNetDotEnvConfigurationSource(new FileInfo(".env"), optional));

            return config;
        }
    }
}
