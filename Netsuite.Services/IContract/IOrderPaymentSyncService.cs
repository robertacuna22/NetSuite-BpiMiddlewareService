using Microsoft.Extensions.Logging;

namespace Netsuite.Services.IContract
{
    public interface IOrderPaymentSyncService
    {
        Task GetPaymentMessage();

        void SyncPaymentEncryptedInfoToBPI(string blobName, ILogger log);
        void SyncPaymentDecryptedInfoFromBPI(string blobName, ILogger log);
    }
}
