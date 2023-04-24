using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Blob;
using Netsuite.Core;
using Netsuite.Services.Entity;
using Netsuite.Services.IContract;
using PgpCore;
using System.ComponentModel;


namespace Netsuite.Services
{
    public class OrderPaymentSyncService : IOrderPaymentSyncService
    {
        private readonly AppSettings _appSettings;
        private readonly IAzureStorage _azureStorage;
        private readonly ISFTPSendingFileSyncService _sFTPBpiPaymentService;

        public OrderPaymentSyncService(IOptions<AppSettings> settings, IAzureStorage azureStorage, 
            ISFTPSendingFileSyncService sFTPBpiPaymentService)
        {
            _appSettings = settings.Value;
            _azureStorage = azureStorage;
            _sFTPBpiPaymentService = sFTPBpiPaymentService;        
        }

        public async Task GetPaymentMessage()
        {
            await Task.Run(() => { });
        }

        public void SyncPaymentDecryptedInfoFromBPI(string blobName, ILogger log)
        {
           
            log.LogInformation("Create TMP directory");
            Directory.CreateDirectory(IOHelper.TempAppDirectoryName);

            log.LogInformation("Create Instance of TMP file names");
            var tempFileResult = GetTemporaryFile(blobName, $"{blobName}.pgp", _appSettings.NetSuitePGPSecKeyFileName);

            log.LogInformation("Start the process to stream the blob name to source temporary file.");
            StreamTheBlobNameToTempFile(_azureStorage.Container, blobName, tempFileResult.SourceFileName);

            if (!VerifyIsEncrypteFileUsingPubKey(tempFileResult.SourceFileName, _appSettings.NetSuitePGPPubKeyFileName, log))
            {
                log.LogInformation($"The target file name is already decrypted!");
                return;
            }

            log.LogInformation("Start the process to stream the blob key to key temporary file.");
            StreamTheBlobKeyFileToTempFile(_azureStorage.Container, _appSettings.NetSuitePGPSecKeyFileName, tempFileResult.KeyFileName);

            PGPDecryptFile(tempFileResult);

            log.LogInformation("The target file has been decrypted successfully.");

            log.LogInformation("Start sending the decrypted file to NetSuite Storage Portal.");
            _sFTPBpiPaymentService.SendDecryptedPaymentInfo(tempFileResult.TargetFileName, blobName);

            log.LogInformation($"Decrypted File Name {blobName}");

            Directory.Delete(IOHelper.TempAppDirectoryName, true);
        }


        public void SyncPaymentEncryptedInfoToBPI(string blobName, ILogger log)
        {
         
            log.LogInformation($"Create TMP directory {IOHelper.TempAppDirectoryName}");
            Directory.CreateDirectory(IOHelper.TempAppDirectoryName);
          
            log.LogInformation("Create Instance of TMP file names");
            var tempFileResult = GetTemporaryFile(blobName, $"{blobName}.pgp", _appSettings.BPIPGPPubKeyFileName);

            log.LogInformation("Start the process to stream the blob name to source temporary file.");
            StreamTheBlobNameToTempFile(_azureStorage.Container, blobName, tempFileResult.SourceFileName);

            if (VerifyIsEncrypteFileUsingPubKey(tempFileResult.SourceFileName, _appSettings.BPIPGPPubKeyFileName, log))
            {
                log.LogInformation($"The target file name is already incrypted!");
                return;
            }

            log.LogInformation("Start the process to stream the blob key to key temporary file.");
            StreamTheBlobKeyFileToTempFile(_azureStorage.Container, _appSettings.BPIPGPPubKeyFileName, tempFileResult.KeyFileName);

            log.LogInformation("Start the process to file encrytion!");
            PGPEncryptFile(tempFileResult);

            log.LogInformation("The Target File has been encrypted successfully :)");

            log.LogInformation("Start sending the encrypted file to BPI portal.");
            _sFTPBpiPaymentService.SendEncryptedPaymentInfo(tempFileResult.TargetFileName, blobName);

            log.LogInformation("The ecrypted File has been sent successfully to BPI.");

            log.LogInformation($"Delete Temp Directory file path {IOHelper.TempAppDirectoryName}");
            Directory.Delete(IOHelper.TempAppDirectoryName, true);
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

            using (var sourceStream = new FileStream(tempSourceFile, FileMode.OpenOrCreate))
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
                SourceFileName = IOHelper.BuildTempFileName(tempblobName),
                TargetFileName = IOHelper.BuildTempFileName(targeBlogName),
                KeyFileName = IOHelper.BuildTempFileName(keyBlobName)
            };

            return tempFile;
        }

        private bool VerifyIsEncrypteFileUsingPubKey(string targFilePath, string keyBlobFileName, ILogger log)
        {
            log.LogInformation("Start process to verify the file is encrypted by pubkey");
            var verified = false;
            var pubKeyFilePath = IOHelper.BuildTempFileName($"pubkey_{keyBlobFileName}");

            log.LogInformation("Stream blob into temporary file.");
            StreamTheBlobKeyFileToTempFile(_azureStorage.Container, keyBlobFileName, pubKeyFilePath);

            try
            {
                EncryptionKeys encryptionKeys;

                using (var publicKeyStream = new FileStream(pubKeyFilePath, FileMode.Open))                
                       encryptionKeys = new EncryptionKeys(publicKeyStream);
                               
                PGP pgp = new PGP(encryptionKeys);
                // Reference input file

                using (FileStream inputFileStream = new FileStream(targFilePath, FileMode.Open))
                       verified = pgp.VerifyStreamAsync(inputFileStream).Result;
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.Message);
            }

            return verified;
        }
    }
}
