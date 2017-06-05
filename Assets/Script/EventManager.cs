using UnityEngine;
using System.Collections;

public class EventManager : MonoBehaviour
{
    public static EventManager instance = null;
    public enum EventType{
        Unknown,
        Send,       // Send player to another place in this or another floor, most stairs include this event.
        GetItem,    // Get an item, most items include this event
        Battle,     // Only have a fight, most normal monsters have this event.
        Chat,       // Chat with somebody
        Choice,     // Need to make a choice and will call another event.
        Game,       // Will play a small game
        Others,     // Call a self-determine function to do something, like "OpenDoor"
    }

    public enum ChatType{
        None,
        Bubble,
        Tip,
        Center,
        Bottom,
        Top

    }

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
			
	}

    public void OnSend(Vector2 destinationPos, int targetMapId = -1){
        
    }

    public void OnGetItem(int itemId, Vector2 itemPos = null){
        
    }

    public void OnBattle(){
        
    }

    public void OnChat(){
        
    }

    public void OnChoice(){
        
    }

    public void OnGame(){
        
    }

}
