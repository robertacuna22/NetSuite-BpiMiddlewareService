using Microsoft.Extensions.Options;
using Netsuite.Core;
using Netsuite.Services.IContract;
using Renci.SshNet;

namespace Netsuite.Services
{
    public class SFTPSendingFileSyncService : ISFTPSendingFileSyncService
    {
        private readonly AppSettings _appSettings;

        public SFTPSendingFileSyncService(IOptions<AppSettings> settings)
        {
            _appSettings = settings.Value;

        }

        public void SendDecryptedPaymentInfo(string decryptedFilePath, string outPutFileName)
        {
            var client = new SftpClient(_appSettings.NetSuiteHostName,
             _appSettings.NetSuitePort,
             _appSettings.NetSuiteUserName,
             _appSettings.NetSuitePassword);

            client.Connect();

            //upload to bpi account
            using (var fileStream = new FileStream(decryptedFilePath, FileMode.Open))
            {
                client.UploadFile(fileStream, outPutFileName, true);
            }
        }

        public void SendEncryptedPaymentInfo(string encryptedFilePath, string outPutFileName)
        {
            var client = new SftpClient(_appSettings.BPIHostName,
                _appSettings.BPIPort,
                _appSettings.BPIUserName,
                _appSettings.BPIPassword);

            client.Connect();

            //upload to bpi account
            using (var fileStream = new FileStream(encryptedFilePath, FileMode.Open))
            {
                client.UploadFile(fileStream, outPutFileName, true);
            }
        }
    }
}
