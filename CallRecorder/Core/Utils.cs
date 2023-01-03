using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using File = System.IO.File;

namespace CallRecorder.Core
{
    public static class Utils
    {
        private static Queue<string> _msgQueue = new Queue<string>();
        public static void KillFFMPEG()
        {
            try
            {
                foreach (var process in Process.GetProcessesByName("ffmpeg"))
                {
                    process.Kill();
                }
            }
            catch (Exception ex) { Utils.Log($"{MethodBase.GetCurrentMethod().Name} {ex.Message}"); }
        }
        public static void Log(string message)
        {
            var logMsg = $"{DateTime.Now.ToString("G")} | {message}{Environment.NewLine}";
            _msgQueue.Enqueue(logMsg);
        }
        public static void StartLogging()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        if (_msgQueue.Count > 0)
                        {
                            File.AppendAllText("log.txt", _msgQueue.Dequeue());
                        }
                        Thread.Sleep(100);
                    }
                    catch (Exception ex) { Utils.Log($"{MethodBase.GetCurrentMethod().Name} {ex.Message}"); }
                }
            });
        }
        public static void CheckAutorun()
        {
            try
            {
                WshShell wshShell = new WshShell();

                string startUpFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

                IWshShortcut shortcut = (IWshShortcut)wshShell.CreateShortcut(
                    startUpFolderPath + "\\" +
                    Application.ProductName + ".lnk");

                shortcut.TargetPath = Application.ExecutablePath;
                shortcut.WorkingDirectory = Application.StartupPath;
                shortcut.Save();
            }
            catch (Exception ex) { Utils.Log($"{MethodBase.GetCurrentMethod().Name} {ex.Message}"); }
        }
    }
}
