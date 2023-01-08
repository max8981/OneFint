using CoreAudio;

namespace 屏幕管理.Systems
{
    internal class Audio
    {
        private static readonly MMDevice _playbackDevice = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        internal static int GetVolume()
        {
            var result = 0;
            if (_playbackDevice.AudioEndpointVolume != null)
                result = (int)(_playbackDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
            return result;
        }
        internal static void SetVolume(int volume)
        {
            volume = Math.Clamp(volume, 0, 100);
            try
            {
                if (_playbackDevice?.AudioEndpointVolume != null)
                    _playbackDevice.AudioEndpointVolume.MasterVolumeLevelScalar = volume / 100f;
            }
            catch (Exception ex)
            {
                Log.Default.Error(ex, "SetVolume", $"Parameter:{volume}");
            }
        }
        internal static void SetVolume(double volume) => SetVolume((int)volume);
    }
}
