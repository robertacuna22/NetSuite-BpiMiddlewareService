namespace Netsuite.Core
{
    public class AppSettings
    {
        public string Message { get; set; }
        public string StorageConnectionString { get; set; }
        public string StorageContainerName { get; set; }
        public string PGPPassPhrase { get; set; }
        public string BPIHostName { get; set; }
        public int BPIPort { get; set; }
        public string BPIUserName { get; set; }
        public string BPIPassword { get; set; }
        public string NetSuiteHostName { get; set; }
        public int NetSuitePort { get; set; }
        public string NetSuiteUserName { get; set; }
        public string NetSuitePassword { get; set; }
        public string BPIPGPSecKeyFileName { get; set; }
        public string BPIPGPPubKeyFileName { get; set; }
        public string NetSuitePGPSecKeyFileName { get; set; }
        public string NetSuitePGPPubKeyFileName { get; set; }

    }
}
