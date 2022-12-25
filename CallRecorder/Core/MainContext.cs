using CallRecorder.Forms;
using CallRecorder.Properties;
using NAudio.Wave;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using File = System.IO.File;
using Timer = System.Timers.Timer;

namespace CallRecorder.Core
{
    public class MainContext : ApplicationContext
    {
        private NotifyIcon trayIcon;
        private Recorder recorder;
        private Detector detector;
        private Timer timer;

        public MainContext()
        {
            Utils.StartLogging();
            Utils.KillFFMPEG();
            Utils.CheckAutorun();
            Directory.CreateDirectory("Records");

            //иконка
            trayIcon = new NotifyIcon()
            {
                Icon = Resources.AppIcon,
                Visible = true
            };

            //таймер на час
            timer = new Timer(60 * 60 * 1000);
            timer.Elapsed += Timer_RefreshCloud;
            timer.AutoReset = true;
            timer.Start();

            //записыватель
            recorder = new Recorder();

            //детектор
            detector = new Detector();
            detector.RecordStarted += Detector_RecordStarted;
            detector.RecordStoped += Detector_RecordStoped;
            detector.StartDetection();
        }

        private async void Timer_RefreshCloud(object sender, System.Timers.ElapsedEventArgs e)
        {
            await CloudManager.RefreshCloudAsync();
        }

        private void Detector_RecordStarted()
        {
            Utils.Log("Начало записи");
            Utils.DeleteTempFiles();

            recorder.StartRecord();
        }
        private void Detector_RecordStoped()
        {
            Utils.Log("Конец записи");

            recorder.StopRecord();

            var minDur = Convert.ToInt32(File.ReadAllText("time_filter.txt"));
            if ((detector.RecordStopTime - detector.RecordStartTime).TotalSeconds > minDur)
            {
                var sr = new SaveRecordForm();
                do { sr.ShowDialog(); } 
                while (string.IsNullOrEmpty(sr.Id));

                recorder.MergeRecord();
                DeleteOldRecords();

                var recordPath = SaveRecordLocal(sr.Id);
                if (!string.IsNullOrEmpty(recordPath))
                    CloudManager.UploadFile(new FileInfo(recordPath));
            }
            else
            {
                Utils.Log($"Запись короче минимальной длины в {minDur} сек!");
            }

            Utils.DeleteTempFiles();
        }
        private void DeleteOldRecords()
        {
            foreach (var f in new DirectoryInfo("Records").GetFiles())
            {
                if ((DateTime.Now - f.CreationTime).TotalDays >= 3)
                {
                    f.Delete();
                }
            }
        }
        private string SaveRecordLocal(string id)
        {
            var recordPath = string.Empty;
            try
            {
                var dur = new AudioFileReader("record.mp4").TotalTime;
                recordPath = Path.Combine("Records", $"{DateTime.Now.ToString("dd-MM-yyyy")}_{(int)dur.TotalSeconds}_{id}.mp4");
                File.Copy(
                    "record.mp4",
                    recordPath, 
                    true);
            }
            catch (Exception ex) { Utils.Log(ex.Message); }

            return recordPath;
        }
    }
}
