using System.Collections.Generic;

public class AudioController
{
    public const int itemGetSound = 20;
    public const int stepSound = 56;
    public const int openDoorSound = 53;
    public const int stairSound = 55;
    public const int disableSound = 17;

    public static AudioController instance = null;
    public UnityEngine.AudioSource MusicSource
    {
        get { return musicSource; }
        set { musicSource = value; musicId = -1; }
    }

    public void AddSoundSource(UnityEngine.AudioSource source) {
        soundSource.Add(source);
        source.volume = soundVolume;
        source.mute = soundMute;
    }

    public void ClearSoundSource() {
        soundSource.Clear();
    }

    public bool PlayMusicLoop(int id)
    {
        if (id == musicId)
            return true;
        musicSource.clip = UnityEngine.Resources.Load<UnityEngine.AudioClip>(Constant.AUDIO_DIR + DataCenter.instance.audios[id].path);
        musicSource.Play();
        musicId = id;
        return true;
    }

    public bool PlayMusicList(int[] idList, int loopTimes = 1)
    {
        // TODO: Realize the list playing function
        return false;
    }

    public bool StopMusic()
    {
        musicSource.Stop();
        return true;
    }

    public bool PlaySound(int id, int times = 1)
	{
        if (soundSource.Count <= index)
            index = 0;
        if (soundSource.Count <= 0)
            return false;
        soundSource[index].clip = UnityEngine.Resources.Load<UnityEngine.AudioClip>(Constant.AUDIO_DIR + DataCenter.instance.audios[id].path);
        soundSource[index].Play();
        ++index;
        return true;
    }

    public void SetMusicMute(bool mute)
    {
        musicSource.mute = mute;
    }

    public void SetSoundMute(bool mute)
    {
        soundMute = mute;
        foreach(var v in soundSource) {
            v.mute = mute;
        }
    }

    public void SetMusicVolume(int volume)
    {
        musicSource.volume = volume;
    }

    public void SetSoundVolume(int volume) {
        soundVolume = volume;
        foreach (var v in soundSource) {
            v.volume = volume;
        }
    }

    private UnityEngine.AudioSource musicSource;
    private List<UnityEngine.AudioSource> soundSource = new List<UnityEngine.AudioSource>();
    private bool soundMute = false;
    private int soundVolume = 100;
    private int index = 0;
    private int musicId = -1;
}
