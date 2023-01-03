using System;
using System.Diagnostics;
using System.Reflection;

namespace CallRecorder.Core
{
    public class VideoRecorder
    {
        private Process ffmpeg;
        public readonly string TempVideoFilemane;
        public VideoRecorder(DateTime id)
        {
            TempVideoFilemane = $"video_{id.Ticks}.mp4";
        }
        public void Start()
        {
            var startInfo = new ProcessStartInfo()
            {
                Arguments = $"-f gdigrab -framerate 25 -i desktop -b:v 300k {TempVideoFilemane}",
                FileName = "ffmpeg.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
            };
            try
            {
                Utils.Log("Захват видео");
                ffmpeg = Process.Start(startInfo);
            }
            catch(Exception ex) { Utils.Log($"{MethodBase.GetCurrentMethod().Name} {ex.Message}"); }
        }
        public void Stop()
        {
            try
            {
                Utils.Log("Завершение захвата видео");
                ffmpeg.StandardInput.WriteLine("q");
                while (!ffmpeg.HasExited) ;
            }
            catch(Exception ex) { Utils.Log($"{MethodBase.GetCurrentMethod().Name} {ex.Message}"); }
        }
    }
}
