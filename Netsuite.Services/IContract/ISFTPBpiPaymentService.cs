
namespace Netsuite.Services.IContract
{
    public interface ISFTPSendingFileSyncService
    {
        void SendEncryptedPaymentInfo(string encryptedFilePath, string outPutFileName);
        void SendDecryptedPaymentInfo(string decryptedFilePath, string outPutFileName);
    }
}
