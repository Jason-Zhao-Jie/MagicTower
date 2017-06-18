using System.Collections.Generic;

public class EventManager
{
    public static EventManager instance = null;

    public void InitEvents()
    {
        RegistEvent(0, null);
        RegistEvent(1, null);
        RegistEvent(2, OpenFreeDoor);
    }

    public bool DispatchEvent(int eventId, Modal caller)
    {
        if (eventId == 0)
            return true;
        var eventData = DataCenter.instance.GetEventDataById(eventId);
        if (eventData == null)
        {
            UnityEngine.Debug.LogWarning("The event in current block called, but no event data found. Event id: " + eventId.ToString());
            return true;
        }
        switch ((Constant.EventType)eventData.typeId)
        {
            case Constant.EventType.Others:
                break;
            case Constant.EventType.Send:
                if (!OnSend((int)(eventData.dataId / 100 % 100), (int)(eventData.dataId % 100), (int)(eventData.dataId / 10000)))
                    return false;
                break;
            case Constant.EventType.GetItem:
                if (eventData.dataId % 100 >= 99)
                {
                    if (!OnGetItem((int)(eventData.dataId / 10000)))
                        return false;
                }
                else if (!OnGetItem((int)(eventData.dataId / 10000), (int)(eventData.dataId / 100 % 100), (int)(eventData.dataId % 100)))
                    return false;
                break;
            case Constant.EventType.Battle:
                if (!OnBattle((int)eventData.dataId))
                    return false;
                break;
            case Constant.EventType.Chat:
                if (!OnChat((int)eventData.dataId, caller.ModId))
                    return false;
                break;
            case Constant.EventType.Choice:
                if (!OnChoice((int)eventData.dataId))
                    return false;
                break;
            case Constant.EventType.Game:
                if (!OnGame((int)eventData.dataId))
                    return false;
                break;
            default:
                return false;
        }
        var cb = eventList[eventId];
        return cb == null || cb(caller);
    }

    bool OnSend(int destinationPosx, int destinationPosy, int targetMapId = 0)
    {
        if (targetMapId > 0)
            MapManager.instance.ShowMap(targetMapId);
        PlayerController.instance.ShowPlayer(destinationPosx, destinationPosy);
        return false;
    }

    bool OnGetItem(int itemId)
    {

        return false;
    }

    bool OnGetItem(int itemId, int itemPosx, int itemPosy)
    {

        return false;
    }

    bool OnBattle(int monsterId)
    {

        return false;
    }

    bool OnChat(int chatId, int modId)
    {
        // TODO
        return false;
    }

    bool OnChoice(int choiceId)
    {

        return false;
    }

    bool OnGame(int gameId)
    {

        return false;
    }

    void RegistEvent(int eventId, Constant.EventCallback cb)
    {
        eventList.Add(eventId, cb);
    }

    private readonly Dictionary<int, Constant.EventCallback> eventList = new Dictionary<int, Constant.EventCallback>();

    // The special event callbacks are below

    private bool OpenFreeDoor(Modal caller)
    {
        caller.GoToRunState();
        return false;
    }
}
