using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;

namespace Netsuite.Services.IContract
{
    public interface IAzureStorage
    {
        CloudStorageAccount StorageAccount { get; }

        CloudBlobClient BlobClient { get; }

        CloudBlobContainer Container { get; }
    }
}
