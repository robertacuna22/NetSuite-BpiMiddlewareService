using Microsoft.Extensions.Configuration;

namespace Netsuite.Core.ConfigurationProviders
{
    public class DotNetEnvironmentVariableConfigurationSource : IConfigurationSource
    {
        private readonly string _environmentVariablePrefix;

        public DotNetEnvironmentVariableConfigurationSource(string prefix = "Values")
        { 
            _environmentVariablePrefix = prefix;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new DotNetEnvironmentVariableConfigurationProvider(_environmentVariablePrefix);
        }
    }
}
