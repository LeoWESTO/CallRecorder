using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace CallRecorder.Core
{
    public class Recorder
    {
        private VideoRecorder videoRecorder;
        private AudioRecorder audioRecorder;

        public readonly string TempRecordFilename;
        public Recorder(DateTime id)
        {
            videoRecorder = new VideoRecorder(id);
            audioRecorder = new AudioRecorder(id);

            TempRecordFilename = $"record_{id.Ticks}.mp4";
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
            catch(Exception ex) { Utils.Log($"{MethodBase.GetCurrentMethod().Name} {ex.Message}"); }
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
            catch(Exception ex){ Utils.Log($"{MethodBase.GetCurrentMethod().Name} {ex.Message}"); }
        }
        public void MergeRecord()
        {
            try
            {
                Process proc = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        Arguments = $"-i {videoRecorder.TempVideoFilemane} -i {audioRecorder.TempAudioFilename} -c:v copy -c:a aac {TempRecordFilename}",
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
            catch (Exception ex) { Utils.Log($"{MethodBase.GetCurrentMethod().Name} {ex.Message}"); }
        }
        public void DeleteTempFiles()
        {
            Utils.Log("Удаление временных файлов...");
            try
            {
                if (File.Exists(audioRecorder.TempMicFilename)) File.Delete(audioRecorder.TempMicFilename);
                if (File.Exists(audioRecorder.TempSysFilename)) File.Delete(audioRecorder.TempSysFilename);
                if (File.Exists(audioRecorder.TempAudioFilename)) File.Delete(audioRecorder.TempAudioFilename);
                if (File.Exists(videoRecorder.TempVideoFilemane)) File.Delete(videoRecorder.TempVideoFilemane);
                if (File.Exists(TempRecordFilename)) File.Delete(TempRecordFilename);
            }
            catch (Exception ex) { Utils.Log($"{MethodBase.GetCurrentMethod().Name} {ex.Message}"); }
        }
    }
}
