using System.Collections.Generic;
public class EventManager
{
    public static EventManager instance = null;

    public EventManager()
    {
        eventList = new Dictionary<EventName, Constant.EventCallback>{
            {EventName.NormalBattle, OnBattle},
            {EventName.OpenFreeDoor, OpenFreeDoor},
            {EventName.NormalSend, OnSend},
            {EventName.NormalChat, OnChat},
            {EventName.NormalChoice, OnChoice},
            {EventName.CallGame, OnGame},
            {EventName.GetPropertyItem, OnGetPropertyItem},
            {EventName.GetResourceItem, OnGetResourceItem},
            {EventName.OpenNormalDoor, OpenNormalDoor},
        };
    }

    public bool DispatchEvent(int eventId, Modal caller, long eventData)
    {
        if (eventId == 0 || !eventList.ContainsKey((EventName)eventId))
            return true;
        var cb = eventList[(EventName)eventId];
        return cb == null || cb(caller, eventData);
    }
    public enum EventName
    {
        None = 0,
        NormalBattle = 1,
        OpenFreeDoor = 2,
        NormalSend = 3,
        NormalChat = 4,
        NormalChoice = 5,
        CallGame = 6,
        GetPropertyItem = 7,
        GetResourceItem = 8,
        OpenNormalDoor = 9,
    }

    public readonly Dictionary<EventName, Constant.EventCallback> eventList = null;

    // The special event callbacks are below
    private bool OnSend(Modal caller, long eventData)
    {
        int posx = (int)(eventData / 100 % 100);
        int posy = (int)(eventData % 100);
        int mapId = (int)(eventData / 10000);
        if (mapId > 0 && mapId != MapManager.instance.CurrentMap.mapId)
            MapManager.instance.ShowMap(mapId);
        PlayerController.instance.ShowPlayer(posx, posy);
        return false;
    }

    private bool OnGetPropertyItem(Modal caller, long eventData)
    {
        return false;
    }

    private bool OnGetResourceItem(Modal caller, long eventData)
    {
        return false;
    }

    private bool OnBattle(Modal caller, long eventData)
    {
        MainScene.instance.StartBattle(caller.Uuid);
        return false;
    }

    private bool OnChat(Modal caller, long eventData)
    {
        var data = DataCenter.instance.GetChatById((int)eventData);
        MainScene.instance.ChatBegan(data, caller);
        return data.canOn;
    }

    private bool OnChoice(Modal caller, long eventData)
    {
        int choiceId = (int)eventData;

        return false;
    }

    private bool OnGame(Modal caller, long eventData)
    {
        int gameId = (int)eventData;

        return false;
    }

    private bool OpenFreeDoor(Modal caller, long blockData)
    {
        AudioController.instance.PlaySound(AudioController.openDoorSound);
        caller.GoToRunState();
        return false;
    }

    private bool OpenNormalDoor(Modal caller, long blockData)
    {

        return false;
    }




}
