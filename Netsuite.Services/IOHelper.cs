using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netsuite.Services
{
    public static class IOHelper
    {
        private static string _TempAppDirectoryName;
        public static string TempRootFile { get; set; } = @"C:\TempBlobDownload";
        public static string TempAppDirectoryName
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_TempAppDirectoryName))
                {
                    var ts = DateTime.UtcNow.Ticks.ToString();
                    _TempAppDirectoryName = Path.Combine(Path.GetTempPath(), TempRootFile, ts);
                }
                return _TempAppDirectoryName;
            }
        }

        public static string BuildTempFileName(string fileName)
        {
            return Path.Combine(TempAppDirectoryName, fileName);
        }
    }
}
