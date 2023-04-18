using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netsuite.Services.Entity
{
    public interface IAzureStorage
    {
        CloudStorageAccount StorageAccount { get; }

        CloudBlobClient BlobClient { get; }

        CloudBlobContainer Container { get; }
    }
}
