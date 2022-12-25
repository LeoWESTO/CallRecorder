using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CallRecorder.Core
{
    public class Recorder
    {
        private VideoRecorder videoRecorder;
        private AudioRecorder audioRecorder;
        public Recorder()
        {
            videoRecorder = new VideoRecorder();
            audioRecorder = new AudioRecorder();
        }
        public void StartRecord()
        {
            try
            {
                Parallel.Invoke(
                    () => videoRecorder.Start(),
                    () => audioRecorder.Start()
                );
            }
            catch(Exception ex) { Utils.Log(ex.Message); }
        }
        public void StopRecord()
        {
            Parallel.Invoke(
                () => videoRecorder.Stop(),
                () => audioRecorder.Stop()
            );
            try
            {
                Process proc = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        Arguments = $"-i .\\{audioRecorder.TempMicFilename} -i .\\{audioRecorder.TempSysFilename} -filter_complex amix=inputs=2:duration=longest .\\{audioRecorder.TempAudioFilename}",
                        FileName = "ffmpeg.exe",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                    }
                };
                proc.Start();
                while (!proc.HasExited) ;
            }
            catch(Exception ex){ Utils.Log(ex.Message); }
        }
        public void MergeRecord()
        {
            try
            {
                Process proc = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        Arguments = $"-i {videoRecorder.TempVideoFilemane} -i {audioRecorder.TempAudioFilename} -c:v copy -c:a aac record.mp4",
                        FileName = "ffmpeg.exe",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                    }
                };
                proc.Start();
                while (!proc.HasExited) ;
            }
            catch (Exception ex) { Utils.Log(ex.Message); }
        }
    }
}
