using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour
{
    static public AudioController instance = null;
	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
			
	}

    public bool PlayMusicLoop(int id){

        return false;
    }

    public bool PlayMusicList(int[] idList, int loopTimes = 1){

        return false;
    }

    public bool PlaySound(int id, int times = 1){

        return false;
    }

    public void SetMusicMute(bool mute){
        
    }

    public void SetSoundMute(bool mute){
        
    }

    public void SetMusicVolume(int volume){
        
    }

    public void SetSoundVolume(int volume){
        
    }


}
