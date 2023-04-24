using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using Netsuite.Services.IContract;
using Azure;
using PgpCore;
using Microsoft.AspNetCore.Mvc;
using Amazon.Runtime.Internal.Auth;
using Netsuite.Services;
using System.Reflection.Metadata.Ecma335;

namespace Netsuite.BPIIntegration
{
    [StorageAccount("BlobConnectionString")]
    public class NetSuiteMiddlewareFunction
    {
        private readonly IOrderPaymentSyncService _orderPaymentSyncService;
        public NetSuiteMiddlewareFunction(IOrderPaymentSyncService orderPaymentSyncService)
        {
            _orderPaymentSyncService = orderPaymentSyncService;
        }

        [FunctionName("NetSuiteMiddlewareFunction")]
        public void Run([BlobTrigger("rootcontainer/{name}")] Stream inputBlob, string name, ILogger log)
        {

            log.LogInformation($"Start the process to encrypt info file and send to BPI file portal.");
            _orderPaymentSyncService.SyncPaymentEncryptedInfoToBPI(name, log);

            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {inputBlob.Length} Bytes");
        }
    }
}
