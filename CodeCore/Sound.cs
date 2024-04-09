using Microsoft.Extensions.DependencyInjection;
using NAudio.Wave;

namespace CodeCore
{
    public class Sound
    {
        static string SoundDir = Path.Combine(Environment.CurrentDirectory, "sound");

        static Dictionary<string, AudioInfo> Devices = new Dictionary<string, AudioInfo>();
        private static void AddAudio(string file)
        {
            if (!Devices.ContainsKey(file))
            {
                var outputDevice = new WaveOutEvent();
                var audioFile = new AudioFileReader(file);
                outputDevice.Init(audioFile);
                Devices.Add(file, new AudioInfo(outputDevice, audioFile));
            }
        }

        public static bool PlayAudio(SoundType sound)
        {
            var audioFile = $"{SoundDir}\\{sound}.wav";

            if (!File.Exists(audioFile)) return false;
            try
            {
                AddAudio(audioFile);
                Devices[audioFile].reader.Position = 0;
                Devices[audioFile].device.Play();
                return true;
            }
            catch (Exception ex)
            {
                Util.Injection.GetService<ILogger>()?.Error(ex, "语音播报出错");
                return false;
            }
        }
        record AudioInfo(WaveOutEvent device, AudioFileReader reader);
    }

    public enum SoundType
    {
        验票失败,
        请通行,
        请看摄像头,
    }
}
