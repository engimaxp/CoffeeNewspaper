using System.IO;

namespace CN_Core
{
    public static class FileHelper
    {
        /// <summary>
        /// Delete a file
        /// </summary>
        public static void DumpFile(string filename)
        {
            if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
            {
                File.Delete(filename);
            }
        }
    }
}
