using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    public static EventManager instance = null;

    public delegate bool EventCallback(Modal caller);
    public enum EventType
    {
        Unknown,
        Send,       // Send player to another place in this or another floor, most stairs include this event.
        GetItem,    // Get an item, most items include this event
        Battle,     // Only have a fight, most normal monsters have this event.
        Chat,       // Chat with somebody
        Choice,     // Need to make a choice and will call another event.
        Game,       // Will play a small game
        Others,     // Call a self-determine function to do something, like "OpenDoor"
    }

    public enum ChatType
    {
        None,
        Bubble,
        Tip,
        Center,
        Bottom,
        Top

    }

    public void InitEvents()
    {
        RegistEvent(0, null);
    }

    public bool DispatchEvent(int eventId, Modal caller)
    {
        var eventData = DataCenter.instance.GetEventDataById(eventId);
        if (eventData == null)
        {
            Debug.LogWarning("The event in current block called, but no event data found. Event id: " + eventId.ToString());
            return true;
        }
        switch ((EventType)eventData.typeId)
        {
            case EventType.Others:
                break;
            case EventType.Send:
                if (!OnSend((int)(eventData.dataId / 100 % 100), (int)(eventData.dataId % 100), (int)(eventData.dataId / 10000)))
                    return false;
                break;
            case EventType.GetItem:
                if (eventData.dataId % 100 >= 99)
                {
                    if (!OnGetItem((int)(eventData.dataId / 10000)))
                        return false;
                }
                else if (!OnGetItem((int)(eventData.dataId / 10000), (int)(eventData.dataId / 100 % 100), (int)(eventData.dataId % 100)))
                    return false;
                break;
            case EventType.Battle:
                if (!OnBattle((int)eventData.dataId))
                    return false;
                break;
            case EventType.Chat:
                if (!OnChat((int)eventData.dataId))
                    return false;
                break;
            case EventType.Choice:
                if (!OnChoice((int)eventData.dataId))
                    return false;
                break;
            case EventType.Game:
                if (!OnGame((int)eventData.dataId))
                    return false;
                break;
            default:
                return false;
        }
        var cb = eventList[eventId];
        return cb == null ? true : cb(caller);
    }

    public bool OnSend(int destinationPosx, int destinationPosy, int targetMapId = -1)
    {

        return false;
    }

    public bool OnGetItem(int itemId)
    {

        return false;
    }

    public bool OnGetItem(int itemId, int itemPosx, int itemPosy)
    {

        return false;
    }

    public bool OnBattle(int monsterId)
    {

        return false;
    }

    public bool OnChat(int chatId)
    {

        return false;
    }

    public bool OnChoice(int choiceId)
    {

        return false;
    }

    public bool OnGame(int gameId)
    {

        return false;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void RegistEvent(int eventId, EventCallback cb)
    {
        eventList.Add(eventId, cb);
    }

    private Dictionary<int, EventCallback> eventList = new Dictionary<int, EventCallback>();

    // The special event callbacks are below

}
