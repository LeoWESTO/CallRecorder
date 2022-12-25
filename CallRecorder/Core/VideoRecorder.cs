using System;
using System.Diagnostics;

namespace CallRecorder.Core
{
    public class VideoRecorder
    {
        private Process ffmpeg;
        public readonly string TempVideoFilemane = "video.mp4";
        public VideoRecorder()
        {

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
            catch(Exception ex) { Utils.Log(ex.Message); }
        }
        public void Stop()
        {
            try
            {
                Utils.Log("Завершение захвата видео");
                ffmpeg.StandardInput.WriteLine("q");
                while (!ffmpeg.HasExited) ;
            }
            catch(Exception ex) { Utils.Log(ex.Message); }
        }
    }
}
