namespace Netsuite.Core
{
    public class AppSettings
    {
        public string Message { get; set; }
        public string StorageConnectionString { get; set; }
        public string PGPSecKeyBlobFileName { get; set; }
        public string PGPPubKeyBlobFileName { get; set; }
        public string StorageContainerName { get; set; }
        public string PGPPassPhrase { get; set; }
        
    }
}
