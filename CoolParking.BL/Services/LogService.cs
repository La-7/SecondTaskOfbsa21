using CoolParking.BL.Interfaces;
using System;
using System.IO;
using System.Text;

namespace CoolParking.BL.Services
{
    public class LogService : ILogService
    {
        public string LogPath { get; set; }

        public LogService(string logPath)
        {
            LogPath = logPath;
        }

        public string Read()
        {
            string result = null;

            FileInfo fileInfo = new FileInfo(LogPath);

            if (!fileInfo.Exists)
            {
                throw new InvalidOperationException("File is not found");
            }

            using (StreamReader sr = new StreamReader(LogPath, Encoding.Default))
            {
                result = sr.ReadToEnd();
            }

            return result;
        }

        public void Write(string logInfo)
        {
            using (StreamWriter sw = new StreamWriter(LogPath, true, Encoding.Default))
            {
                sw.WriteLine(logInfo);
            }
        }
    }
}


