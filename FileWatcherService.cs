using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService.FileListener.Demo
{
    public partial class FileWatcherService : ServiceBase
    {
        public FileWatcherService()
        {
            InitializeComponent();
            string logDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
        }

        protected override void OnStart(string[] args)
        {
            WriteLog("Service started");
        }

        protected override void OnStop()
        {
            WriteLog("Service stopped");
        }

        public void WriteLog(string message)
        {
            eventLog.WriteEntry(message);
            string logFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\FileWatcherServiceLog_" + DateTime.Now.ToShortDateString().Replace("/", "_") +".txt";

            if (!File.Exists(logFilePath))
            {
                using (StreamWriter sw = new StreamWriter(logFilePath))
                {
                    sw.WriteLine($"{message} - {DateTime.Now}");
                }
            }
            else
            {
                using(StreamWriter sw = File.AppendText(logFilePath))
                {
                    sw.WriteLine($"{message} - {DateTime.Now}");
                }
            }
        }

        private void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            WriteLog($"Changed {e.FullPath}");
        }

        private void fileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            WriteLog($"File created {e.FullPath}");
            string destination = @"C:\Folder2\" + Path.GetFileName(e.FullPath);

            File.Move(e.FullPath, destination);

            WriteLog($"File was moved to {destination}");
        }

        private void fileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            WriteLog($"File deleted {e.FullPath}");
        }
    }
}
