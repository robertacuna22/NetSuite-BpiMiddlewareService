// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventGrid;
using System.IO;
using System.Reflection.Metadata;
using Netsuite.Services.IContract;
using Newtonsoft.Json;

namespace NetSuiteTriggerFunction.BPIIntegration
{
    public class NetSuiteTriggerFunctionMiddleware
    {

        private readonly IOrderPaymentSyncService _orderPaymentSyncService;
        public NetSuiteTriggerFunctionMiddleware(IOrderPaymentSyncService orderPaymentSyncService)
        {
            _orderPaymentSyncService = orderPaymentSyncService;
        }

        [FunctionName("NetSuiteEventTriggerFileEncryption")]
        public void NetSuiteEventTriggerFileEncryption([EventGridTrigger]EventGridEvent eventGridEvent,
                [Blob("{data.url}", FileAccess.Read, Connection = "BlobConnectionString")] Stream inputBlob,
            ILogger log)
        {
            log.LogInformation("Start sending payment process from netsuite to BPI.");

            try
            {

                log.LogInformation($"Get event grid information");
                var eventInformation = JsonConvert.DeserializeObject<EventGridInfo>(eventGridEvent.Data.ToString());

                if (eventInformation != null)
                {
                    log.LogInformation($"{eventInformation.url} is the name of blobFile");

                    var hreflink = new Uri(eventInformation.url);
                    string filename = System.IO.Path.GetFileName(hreflink.LocalPath);

                    log.LogInformation($"Get the usefull url file name {filename}");

                    log.LogInformation($"Call the function to encrypt the payment data to send on BPI file portal.");
                    _orderPaymentSyncService.SyncPaymentEncryptedInfoToBPI(filename, log);
                }
                         
                log.LogInformation(eventGridEvent.Data.ToString());

            }
            catch (Exception ex)
            {

                log.LogInformation(ex.Message.ToString());
            }

            log.LogInformation("End Process");

        }

        [FunctionName("NetSuiteEventTriggerFileDecryption")]
        public void NetSuiteEventTriggerFileDecryption([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log)
        {
            log.LogInformation("Start the process of data encryption.");

            try
            {

                log.LogInformation($"Get event grid information");
                var eventInformation = JsonConvert.DeserializeObject<EventGridInfo>(eventGridEvent.Data.ToString());

                if (eventInformation != null)
                {
                    log.LogInformation($"{eventInformation.url} is the name of blobFile");

                    var hreflink = new Uri(eventInformation.url);
                    string filename = System.IO.Path.GetFileName(hreflink.LocalPath);

                    log.LogInformation($"Get the usefull url file name {filename}");

                    log.LogInformation($"Call the function to encrypt the data from BPI");
                    _orderPaymentSyncService.SyncPaymentDecryptedInfoFromBPI(filename, log);
                }

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
