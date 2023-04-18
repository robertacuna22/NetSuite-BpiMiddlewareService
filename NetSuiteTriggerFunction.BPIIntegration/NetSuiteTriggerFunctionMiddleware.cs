// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventGrid;
using System.IO;
using System.Threading.Tasks;
using System.Reflection.Metadata;
using Netsuite.Services.IContract;
using CsvHelper.Configuration;
using CsvHelper;
using Netsuite.Services.Entity;
using System.Linq;

namespace NetSuiteTriggerFunction.BPIIntegration
{
    public class NetSuiteTriggerFunctionMiddleware
    {

        private readonly IOrderPaymentSyncService _orderPaymentSyncService;
        public NetSuiteTriggerFunctionMiddleware(IOrderPaymentSyncService orderPaymentSyncService)
        {
            _orderPaymentSyncService = orderPaymentSyncService;
        }

        [FunctionName("NetSuiteTriggerFunctionMiddleware")]
        public void Run([EventGridTrigger]EventGridEvent eventGridEvent,
                [Blob("{data.url}", FileAccess.Read, Connection = "BlobConnectionString")] Stream inputBlob, string name,
            ILogger log)
        {
            log.LogInformation("Start sending payment process from netsuite to BPI.");

            try
            {
                log.LogInformation("Call the function to process the payment.");
                _orderPaymentSyncService.SyncPaymentEncryptedInfoToBPI(inputBlob, name, log);
              
                log.LogInformation(eventGridEvent.Data.ToString());

            }
            catch (Exception ex)
            {

                log.LogInformation(ex.Message.ToString());
            }

            log.LogInformation("End Process");

        }
    }
}
