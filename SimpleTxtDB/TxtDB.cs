using System;
using System.IO;
using System.Text;

namespace SimpleTxtDB
{
    public class TxtDB
    {
        private const int BLOCKSTEP = 500;
        private readonly string dbfile = null;

        /// <summary>
        /// 根据dbname创建数据库文件
        /// </summary>
        /// <param name="dbname">文件名称</param>
        /// <param name="directory">文件路径</param>
        public TxtDB(string dbname,string directory=null)
        {
            if (string.IsNullOrEmpty(directory))
                directory = AppDomain.CurrentDomain.BaseDirectory;
            if (!directory.EndsWith("\\"))
                directory = directory + "\\";

            if (dbname.Contains("\\"))
            {
                string parentDirectory =
                    directory + dbname.Substring(0, dbname.LastIndexOf("\\", StringComparison.Ordinal));
                if (!Directory.Exists(parentDirectory))
                {
                    Directory.CreateDirectory(parentDirectory);
                }
            }

            dbfile = directory + dbname + ".json"; 
        }
        /// <summary>
        /// 插入最后一行
        /// </summary>
        /// <param name="inputline"></param>
        public void AppendAndSave(string inputline)
        {
            if(string.IsNullOrEmpty(inputline)) return;
            using (FileStream fs = new FileStream(dbfile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
            {
                fs.Position = fs.Length;
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    var buffer = inputline.ToCharArray();
                    sw.WriteLine(buffer, 0, buffer.Length);
                }
            }
        }
        /// <summary>
        /// 读取最后一行
        /// </summary>
        /// <returns></returns>
        public string ReadLastLine()
        {
            if (dbfile == null)
            {
                return string.Empty;
            }
            string lastline = null;
            int count = 1;
            int lineSearched = 0;
            try
            {
                using (FileStream fs = new FileStream(dbfile, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                {
                    while (lastline == null && lineSearched <= 1)
                    {
                        if (fs.Length > count * BLOCKSTEP)
                        {
                            fs.Position = fs.Length - count * BLOCKSTEP;
                        }
                        else
                        {
                            fs.Position = 0;
                        }
                        using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                        {
                            string nextline = null;
                            while ((nextline = sr.ReadLine()) != null)
                            {
                                lineSearched++;
                                lastline = nextline;
                            }
                        }
                        if (dbfile.Length <= count * BLOCKSTEP)
                        {
                            break;
                        }
                        count++;
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return lastline??string.Empty;
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        public void DumpFile()
        {
            if (!string.IsNullOrEmpty(dbfile) && File.Exists(dbfile))
            {
                File.Delete(dbfile);
            }
        }
        /// <summary>
        /// 读取所有数据
        /// </summary>
        /// <returns></returns>
        public string ReadAll()
        {
            if (dbfile == null)
            {
                return string.Empty;
            }
            string returnString = null;
            try
            {
                using (FileStream fs = new FileStream(dbfile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                    {
                        returnString = sr.ReadToEnd();
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return returnString;
        }

        /// <summary>
        /// 全部覆写
        /// </summary>
        /// <param name="alltext"></param>
        public void OverWrite(string alltext)
        {
            DumpFile();
            if (string.IsNullOrEmpty(alltext)) return;
            using (FileStream fs = new FileStream(dbfile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    var buffer = alltext.ToCharArray();
                    sw.WriteLine(buffer, 0, buffer.Length);
                }
            }
        }
    }
}
