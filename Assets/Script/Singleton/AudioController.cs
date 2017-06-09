public class AudioController
{
    public static AudioController instance = null;
    public UnityEngine.AudioSource MusicSource
    {
        get { return musicSource; }
        set { musicSource = value; }
    }

    public UnityEngine.AudioSource SoundSource
    {
        get { return soundSource; }
        set { soundSource = value; }
    }

    public bool PlayMusicLoop(int id)
    {
        musicSource.clip = UnityEngine.Resources.Load<UnityEngine.AudioClip>(DataCenter.instance.GetAudioById(id));
        return true;
    }

    public bool PlayMusicList(int[] idList, int loopTimes = 1)
    {
        // TODO: Realize the list playing function
        return false;
    }

    public bool PlaySound(int id, int times = 1)
	{
        soundSource.clip = UnityEngine.Resources.Load<UnityEngine.AudioClip>(DataCenter.instance.GetAudioById(id));
        return true;
    }

    public void SetMusicMute(bool mute)
    {
        musicSource.mute = mute;
    }

    public void SetSoundMute(bool mute)
    {
        soundSource.mute = mute;
    }

    public void SetMusicVolume(int volume)
    {
        musicSource.volume = volume;
    }

    public void SetSoundVolume(int volume)
    {
        soundSource.volume = volume;
    }

    private UnityEngine.AudioSource musicSource;
    private UnityEngine.AudioSource soundSource;
}
