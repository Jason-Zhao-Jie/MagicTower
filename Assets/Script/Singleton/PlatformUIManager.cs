using UnityEngine;

#if UNITY_WINDOWS

#endif
#if UNITY_ANDROID

#endif

public class PlatformUIManager : MonoBehaviour
{
	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
			
	}

    public static bool ShowMessageBox(string message){

        return false;
    }
}
