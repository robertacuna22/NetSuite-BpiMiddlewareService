using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Netsuite.Core;
using Netsuite.Services.IContract;

namespace Netsuite.Services
{
    public class AzureStorage : IAzureStorage
    {
        private readonly AppSettings _appSettings;
        public AzureStorage(IOptions<AppSettings> settings)
        {
            _appSettings = settings.Value;
        }

        public CloudStorageAccount StorageAccount
        {
            get { return CloudStorageAccount.Parse(_appSettings.StorageConnectionString); }
        }

        public CloudBlobClient BlobClient
        {
            get { return StorageAccount.CreateCloudBlobClient(); }
        }

        public CloudBlobContainer Container
        {
            get { return BlobClient.GetContainerReference(_appSettings.StorageContainerName); }
        }
    }
}
