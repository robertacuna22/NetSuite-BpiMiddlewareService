using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Blob;
using Netsuite.Core;
using Netsuite.Services.Entity;
using Netsuite.Services.IContract;
using PgpCore;


namespace Netsuite.Services
{
    public class OrderPaymentSyncService : IOrderPaymentSyncService
    {
        private readonly AppSettings _appSettings;
        private readonly IAzureStorage _azureStorage;

        public OrderPaymentSyncService(IOptions<AppSettings> settings, IAzureStorage azureStorage)
        {
            _appSettings = settings.Value;
            _azureStorage = azureStorage;
        }

        public async Task GetPaymentMessage()
        {
            await Task.Run(() => { });
        }

        public void SyncPaymentDecryptedInfoToBPI(Stream inputBlob, string blobName, ILogger log)
        {
            log.LogInformation("Create TMP directory");
            Directory.CreateDirectory(IOHelper.TempAppDirectoryName);

            log.LogInformation("Get csv input blob from azure trigger container.");

            log.LogInformation("Create Instance of TMP file names");
            var tempFileResult = GetTemporaryFile(blobName, $"{blobName}.pgp", _appSettings.PGPSecKeyBlobFileName);

            StreamTheBlobNameToTempFile(_azureStorage.Container, blobName, tempFileResult.SourceFileName);
            StreamTheBlobKeyFileToTempFile(_azureStorage.Container, _appSettings.PGPSecKeyBlobFileName, tempFileResult.KeyFileName);

            PGPDecryptFile(tempFileResult);

            UploadPGPFileResultToTheSameBlobFile(blobName, tempFileResult.TargetFileName);

            Directory.Delete(IOHelper.TempRootFile, true);
        }


        public void SyncPaymentEncryptedInfoToBPI(Stream inputBlob, string blobName, ILogger log)
        {

            log.LogInformation("Create TMP directory");
            Directory.CreateDirectory(IOHelper.TempAppDirectoryName);

            log.LogInformation("Get csv input blob from azure trigger container.");

            log.LogInformation("Create Instance of TMP file names");
            var tempFileResult = GetTemporaryFile(blobName, $"{blobName}.pgp", _appSettings.PGPPubKeyBlobFileName);

            StreamTheBlobNameToTempFile(_azureStorage.Container, blobName, tempFileResult.SourceFileName);
            StreamTheBlobKeyFileToTempFile(_azureStorage.Container, _appSettings.PGPPubKeyBlobFileName, tempFileResult.KeyFileName);

            PGPEncryptFile(tempFileResult);

            UploadPGPFileResultToTheSameBlobFile(blobName, tempFileResult.TargetFileName);

            Directory.Delete(IOHelper.TempRootFile, true);
        }

        private void UploadPGPFileResultToTheSameBlobFile(string blobName, string targetFile)
        {
            // write to target blob
            using (var blobStream = new FileStream(targetFile, FileMode.Open))
            {
                var targetBlobClient = new BlobClient(_appSettings.StorageConnectionString, _appSettings.StorageContainerName, blobName);
                targetBlobClient.UploadAsync(blobStream, true).Wait();
            }
        }

        private void PGPEncryptFile(TempFile tempFile)
        {
            if (tempFile == null)
                throw new ArgumentNullException("No to null file");

            // Encrypt           
            using (var pgp = new PGP())
            {
                using (FileStream inputFileStream = new FileStream(tempFile.SourceFileName, FileMode.Open))
                {
                    using (Stream outputFileStream = File.Create(tempFile.TargetFileName))
                    {
                        using (Stream publicKeyStream = new FileStream(tempFile.KeyFileName, FileMode.Open))
                        {
                            pgp.EncryptStreamAsync(inputFileStream, outputFileStream, publicKeyStream).Wait();
                        }
                    }
                }
            }
        }

        private void PGPDecryptFile(TempFile tempFile)
        {
            if (tempFile == null)
                throw new ArgumentNullException("No to null file");

            // Dencrypt           
            using (var pgp = new PGP())
            {
                using (FileStream inputFileStream = new FileStream(tempFile.SourceFileName, FileMode.Open))
                {
                    using (Stream outputFileStream = File.Create(tempFile.TargetFileName))
                    {
                        using (Stream privateKeyStream = new FileStream(tempFile.KeyFileName, FileMode.Open))
                        {
                            pgp.DecryptStreamAsync(inputFileStream, outputFileStream, privateKeyStream, _appSettings.PGPPassPhrase).Wait();
                        }
                    }
                }
            }
        }

        private void StreamTheBlobNameToTempFile(CloudBlobContainer container, string blobName,  string tempSourceFile)
        {

            CloudBlockBlob _blobInputFile = container.GetBlockBlobReference(blobName);

            using (var sourceStream = new FileStream(tempSourceFile, FileMode.Create))
            {
                _blobInputFile.DownloadToStreamAsync(sourceStream).Wait();

            }

        }

        private void StreamTheBlobKeyFileToTempFile(CloudBlobContainer container, string blobName, string tempSourceFile)
        {

            CloudBlockBlob _blobInputFile = container.GetBlockBlobReference(blobName);

            using (var sourceStream = new FileStream(tempSourceFile, FileMode.Create))
            {
                _blobInputFile.DownloadToStreamAsync(sourceStream).Wait();

            }

        }
        private TempFile GetTemporaryFile(string tempblobName, string targeBlogName, string keyBlobName)
        {
            var tempFile = new TempFile()
            {
                SourceFileName = tempblobName,
                TargetFileName = targeBlogName,
                KeyFileName = keyBlobName
            };

            return tempFile;
        }
    }
}
