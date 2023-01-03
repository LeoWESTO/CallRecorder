using Microsoft.Win32;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CallRecorder.Core
{
    public class Detector
    {
        public delegate void DetectorHandler();
        public event DetectorHandler RecordStarted;
        public event DetectorHandler RecordStoped;
        public DateTime RecordStartTime { get; private set; }
        public DateTime RecordStopTime { get; private set; }

        private bool isRecordWorking = false;
        
        public Detector()
        {

        }
        public void StartDetection()
        {
            Task.Run(() =>
            {
                try
                {
                    bool camInUse, micInUse;
                    while (true)
                    {
                        camInUse = IsWebCamInUse();
                        micInUse = IsMicInUse();
                        if (camInUse && !isRecordWorking)
                        {
                            //был выключен, а теперь включен
                            RecordStartTime = DateTime.Now;
                            isRecordWorking = true;
                            RecordStarted?.Invoke();
                        }
                        if (!micInUse && isRecordWorking)
                        {
                            //был включен, а теперь выключен
                            if ((DateTime.Now - RecordStartTime).TotalSeconds > 5) //защита от "миганий"
                            {
                                RecordStopTime = DateTime.Now;
                                isRecordWorking = false;
                                RecordStoped?.Invoke();
                            }
                        }
                        Thread.Sleep(100);
                    }
                }
                catch(Exception ex) { Utils.Log($"{MethodBase.GetCurrentMethod().Name} {ex.Message}"); }
            });
        }
        private static bool IsWebCamInUse()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam\NonPackaged"))
                {
                    foreach (var subKeyName in key.GetSubKeyNames())
                    {
                        using (var subKey = key.OpenSubKey(subKeyName))
                        {
                            if (subKey.GetValueNames().Contains("LastUsedTimeStop"))
                            {
                                var endTime = subKey.GetValue("LastUsedTimeStop") is long ? (long)subKey.GetValue("LastUsedTimeStop") : -1;
                                if (endTime <= 0)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex) { Utils.Log($"{MethodBase.GetCurrentMethod().Name} {ex.Message}"); }
            
            return false;
        }
        private static bool IsMicInUse()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\microphone\NonPackaged"))
                {
                    foreach (var subKeyName in key.GetSubKeyNames().Where(s => !s.Contains("CallRecorder")))
                    {
                        using (var subKey = key.OpenSubKey(subKeyName))
                        {
                            if (subKey.GetValueNames().Contains("LastUsedTimeStop"))
                            {
                                var endTime = subKey.GetValue("LastUsedTimeStop") is long ? (long)subKey.GetValue("LastUsedTimeStop") : -1;
                                if (endTime <= 0)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { Utils.Log($"{MethodBase.GetCurrentMethod().Name} {ex.Message}"); }

            return false;
        }
    }
}
