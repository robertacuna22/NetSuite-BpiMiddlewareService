

using CsvHelper.Configuration;
using CsvHelper;
using Netsuite.Services.Entity;
using Microsoft.Extensions.Logging;
using Netsuite.Core;
using Netsuite.Services;
using PgpCore;
using Microsoft.Extensions.Configuration;
using Netsuite.Services.IContract;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using NetSuite.Testing;
using Netsuite.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace NetSuite.Testing
{
    class Program 
    {
        static void Main(string[] args)
        {


            //test
            var filePathInventory = @"C:\Users\Admin\OneDrive\Desktop\AzureFunction\UploadSample\Information.csv";

            using (var streamReader = new StreamReader(filePathInventory))
            {

                var csvConfig = new CsvConfiguration(System.Globalization.CultureInfo.CurrentCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null,
                    BadDataFound = null,

                };

                using (var csv = new CsvReader(streamReader, csvConfig))
                {

                    var paymentInformationList = csv.GetRecords<PaymentInformation>().ToList();

                    paymentInformationList.ForEach(paymentInformation =>
                    {

                    });

                }
            }


        }

      

    }
}
