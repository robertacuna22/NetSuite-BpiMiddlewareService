

using CsvHelper.Configuration;
using CsvHelper;
using Netsuite.Services.Entity;

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
