using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Logs
{
    public class Logger
    {
        private readonly string _callingClassName;

        string fileName;

        // FIXME: add these to a config file
        public LogTypes MinLogLevel { get; set; } = LogTypes.INFO;
        public LogTypes MaxLogLevel { get; set; } = LogTypes.VERBOSE;


        public string ClassName { get => _callingClassName;  }

        public Logger(string callingClassName)
        {
            this._callingClassName = callingClassName;

            CreateNewLogFile();
        }

        public enum LogTypes
        {
            INFO,
            DEBUG,
            WARNING,
            ERROR,
            VERBOSE
        }

        private void CreateNewLogFile()
        {
            if (!Directory.Exists(LogManager.LogFolderLocation))
                Directory.CreateDirectory(LogManager.LogFolderLocation);

            if (string.IsNullOrEmpty(fileName))
            {
                var createdLog = new FileStream($"{LogManager.LogFolderLocation}{_callingClassName}_{DateTime.Now:yyyyMMdd_HHmmss}.log", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                createdLog.Close();

                fileName = createdLog.Name;
            }
        }

        private bool ShouldLog(LogTypes type) => (LogTypes)(MaxLogLevel - type) >= MinLogLevel; 

        private void Log(LogTypes type, string message)
        {
            string logMessgae = $"[{DateTime.Now}][{type}]: {message}" + Environment.NewLine;

            File.AppendAllText(fileName, logMessgae);
        }

        public void LogError(string msg, Exception ex = null)
        {
            if (ShouldLog(LogTypes.ERROR))
            {
                Log(LogTypes.ERROR, msg);
                Log(LogTypes.ERROR, ex.Message);
                Log(LogTypes.ERROR, ex.StackTrace);
            }
        }

        public void LogWarning(string msg)
        {
            if (ShouldLog(LogTypes.WARNING))
            {
                Log(LogTypes.WARNING, msg);
            }
        }

        public void LogDebug(string msg)
        {
            if (ShouldLog(LogTypes.DEBUG))
            {
                Log(LogTypes.DEBUG, msg);
            }
        }

        public void LogVerbose(string msg)
        {
            if (ShouldLog(LogTypes.VERBOSE))
            {
                Log(LogTypes.VERBOSE, msg);
            }
        }

        public void LogInfo(string msg)
        {
            if (ShouldLog(LogTypes.INFO))
            {
                Log(LogTypes.INFO, msg);
            }
        }
    }
}
