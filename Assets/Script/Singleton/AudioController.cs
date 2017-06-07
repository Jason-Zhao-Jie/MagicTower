using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour
{
    public static AudioController instance = null;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public AudioSource MusicSource
    {
        get { return musicSource; }
        set { musicSource = value; }
    }

    public AudioSource SoundSource
    {
        get { return soundSource; }
        set { soundSource = value; }
    }

    public bool PlayMusicLoop(int id)
    {
        musicSource.clip = Resources.Load<AudioClip>(DataCenter.instance.GetAudioById(id));
        return true;
    }

    public bool PlayMusicList(int[] idList, int loopTimes = 1)
    {
        // TODO: Realize the list playing function
        return false;
    }

    public bool PlaySound(int id, int times = 1)
	{
        soundSource.clip = Resources.Load<AudioClip>(DataCenter.instance.GetAudioById(id));
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

    private AudioSource musicSource = null;
    private AudioSource soundSource = null;
}
