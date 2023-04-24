using System.Security.Cryptography.X509Certificates;

namespace Netsuite.Services
{
    public static class IOHelper
    {
        private static string _TempAppDirectoryName;
        public static string TempRootFile { get; set; } = Path.GetTempPath();
        public static string TempAppDirectoryName
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_TempAppDirectoryName))
                {
                    var ts = DateTime.UtcNow.Ticks.ToString();
                    _TempAppDirectoryName = Path.Combine(TempRootFile, ts);
                }
                return _TempAppDirectoryName;
            }
        }

        public static string BuildTempFileName(string fileName)
        {
            return Path.Combine(TempAppDirectoryName, fileName);
        }

        public static void BuildTempFileKey(string filePath, string key)
        {

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                using (StreamWriter streamOnTempFile = new StreamWriter(fileStream))
                {
                    streamOnTempFile.WriteLine(key.Trim());
                }
            }

        }
    }
}
