using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Netsuite.BPIIntegration
{
    [StorageAccount("BlobConnectionString")]
    public class NetSuiteMiddlewareFunction
    {

        [FunctionName("NetSuiteMiddlewareFunction")]
        public void Run([BlobTrigger("rootcontainer/{name}")]Stream inputBlob, string name, ILogger log)
        {
            using (var streamReader = new StreamReader(inputBlob))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    log.LogInformation(line);
                }
            }

            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {inputBlob.Length} Bytes");
        }
    }
}
