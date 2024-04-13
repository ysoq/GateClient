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

        public static void PlayAudio(SoundType sound, string? appendText = null)
        {
            if (!string.IsNullOrEmpty(appendText))
            {
                PlayAudio(RemoveSpecialCharacters(sound.ToString() + appendText));
            }
            else
            {
                PlayAudio(RemoveSpecialCharacters(sound.ToString()));
            }
        }
        static HttpClient HttpClient { get; set; } = new HttpClient();
        public static async void PlayAudio(string sound)
        {
            var logger = Util.Injection.GetService<ILogger>();
            logger?.Info("sound:", sound);

            var audioFile = GetSoundPath(sound);

            if (!File.Exists(audioFile))
            {
                try
                {
                    var api = Appsettings.Default.Node("api").Value<string>("soundApi");
                    var message = new HttpRequestMessage(HttpMethod.Get, $"{api}/api/tts/v1?text={sound}");
                    var response = await HttpClient.SendAsync(message);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsByteArrayAsync();
                        await File.WriteAllBytesAsync(audioFile, content);
                    }
                }
                catch (Exception ex)
                {
                    logger?.Error(ex);
                }

            }
            if (!File.Exists(audioFile)) return;
            try
            {
                AddAudio(audioFile);
                Devices[audioFile].reader.Position = 0;
                Devices[audioFile].device.Play();
                return;
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "语音播报出错");
                return;
            }
        }

        private static string GetSoundPath(string sound)
        {
            var audioFile = $"{SoundDir}\\{sound}.wav";
            return audioFile;
        }
        record AudioInfo(WaveOutEvent device, AudioFileReader reader);

        public static string RemoveSpecialCharacters(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var cleanedFileName = string.Join("", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
            return cleanedFileName;
        }
    }

    public enum SoundType
    {
        检票失败,
        检票成功,
        请看摄像头,
    }
}
