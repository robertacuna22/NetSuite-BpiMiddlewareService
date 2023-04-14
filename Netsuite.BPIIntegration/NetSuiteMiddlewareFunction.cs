using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Netsuite.Services.IContract;

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
        public void Run([BlobTrigger("rootcontainer/{name}")]Stream inputBlob, string name, ILogger log)
        {
            _orderPaymentSyncService.GetPaymentMessage(log);

            using (var streamReader = new StreamReader(inputBlob))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    log.LogInformation(line);
                }
            }

            log.LogInformation("Practice makes perfect");
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {inputBlob.Length} Bytes");
        }
    }
}
