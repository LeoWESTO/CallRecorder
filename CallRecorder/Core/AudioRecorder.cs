using NAudio.Wave;
using System;

namespace CallRecorder.Core
{
    public class AudioRecorder
    {
        private WaveInEvent micSource = null;
        private WaveFileWriter micFile = null;
        public readonly string TempMicFilename = "mic.wav";

        private WasapiLoopbackCapture sysSourse;
        private WaveFileWriter sysFile;
        public readonly string TempSysFilename = "sys.wav";

        public readonly string TempAudioFilename = "audio.wav";
        public AudioRecorder()
        {
        }
        public void Start()
        {
            try
            {
                micSource = new WaveInEvent() { WaveFormat = new WaveFormat(16000, 1) };
                sysSourse = new WasapiLoopbackCapture();

                sysSourse.DataAvailable += SysSourse_DataAvailable;
                sysSourse.RecordingStopped += SysSourse_RecordingStopped;
                micSource.DataAvailable += MicSource_DataAvailable;
                micSource.RecordingStopped += MicSource_RecordingStopped;

                micFile = new WaveFileWriter(TempMicFilename, micSource.WaveFormat);
                sysFile = new WaveFileWriter(TempSysFilename, sysSourse.WaveFormat);

                Utils.Log("Захват аудио");
                sysSourse.StartRecording();
                micSource.StartRecording();
                
            }
            catch(Exception ex) { Utils.Log(ex.Message); }
        }
        public void Stop()
        {
            try
            {
                sysSourse?.StopRecording();
                micSource?.StopRecording();
            }
            catch(Exception ex) { Utils.Log(ex.Message); }
        }
        private void SysSourse_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (sysFile != null)
            {
                sysFile.Write(e.Buffer, 0, e.BytesRecorded);
                sysFile.Flush();
            }
        }
        private void SysSourse_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (sysSourse != null)
            {
                sysSourse.Dispose();
                sysSourse = null;
            }

            if (sysFile != null)
            {
                sysFile.Dispose();
                sysFile = null;
            }
        }
        private void MicSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (micFile != null)
            {
                micFile.Write(e.Buffer, 0, e.BytesRecorded);
                micFile.Flush();
            }
        }
        private void MicSource_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (micSource != null)
            {
                micSource.Dispose();
                micSource = null;
            }

            if (micFile != null)
            {
                micFile.Dispose();
                micFile = null;
            }
        }
    }
}
