using System.Collections.Generic;

public class EventManager
{
    public static EventManager instance = null;

    public void InitEvents()
    {
        RegistEvent(0, null);
        RegistEvent(1, null);
        RegistEvent(2, OpenFreeDoor);
        RegistEvent(6, OpenNormalDoor);
        RegistEvent(7, RemoveEventAtBlock);

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
                if (!OnBattle(caller.Uuid,eventData.dataId))
                    return false;
                break;
            case Constant.EventType.Chat:
                if (!OnChat((int)eventData.dataId, caller))
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
        if (!eventList.ContainsKey(eventId))
            return true;
        var cb = eventList[eventId];
        return cb == null || cb(caller, eventData.dataId);
    }

    private bool OnSend(int destinationPosx, int destinationPosy, int targetMapId = 0)
    {
        if (targetMapId > 0)
            MapManager.instance.ShowMap(targetMapId);
        PlayerController.instance.ShowPlayer(destinationPosx, destinationPosy);
        return false;
    }

    private bool OnGetItem(int itemId)
    {
        return false;
    }

    private bool OnGetItem(int itemId, int itemPosx, int itemPosy)
    {

        return false;
    }

    private bool OnBattle(long enemyUuid, long playerUuid)
    {
        MainScene.instance.StartBattle(enemyUuid, playerUuid);
        return false;
    }

    private bool OnChat(int chatId, Modal caller)
    {
        var data = DataCenter.instance.GetChatById(chatId);
        MainScene.instance.ChatBegan(data, caller);
        return data.canOn;
    }

    private bool OnChoice(int choiceId)
    {

        return false;
    }

    private bool OnGame(int gameId)
    {

        return false;
    }

    private void RegistEvent(int eventId, Constant.EventCallback cb)
    {
        eventList.Add(eventId, cb);
    }

    private readonly Dictionary<int, Constant.EventCallback> eventList = new Dictionary<int, Constant.EventCallback>();

    // The special event callbacks are below

    private bool OpenFreeDoor(Modal caller, long data)
    {
        AudioController.instance.PlaySound(AudioController.openDoorSound);
        caller.GoToRunState();
        return false;
    }

    private bool OpenNormalDoor(Modal caller, long data)
    {
        switch (caller.ModId)
        {
            case 148:
                if (PlayerController.instance.YellowKey <= 0)
                    return false;
                --PlayerController.instance.YellowKey;
                break;
            case 10:
                if (PlayerController.instance.BlueKey <= 0)
                    return false;
                --PlayerController.instance.BlueKey;
                break;
            case 79:
                if (PlayerController.instance.RedKey <= 0)
                    return false;
                --PlayerController.instance.RedKey;
                break;
            case 99999: // TODO: have not set the green door's modal
                if (PlayerController.instance.GreenKey <= 0)
                    return false;
                --PlayerController.instance.GreenKey;
                break;
            case 19:    // brozen
            case 100:   // silver
            case 44:    // gold
                return false;
        }
        AudioController.instance.PlaySound(AudioController.openDoorSound);
        caller.GoToRunState();
        return false;
    }

    private bool RemoveEventAtBlock(Modal caller, long data)
    {
        if (data == 0)
            if (caller == null)
                MapManager.instance.RemoveEventOn(PlayerController.instance.posx, PlayerController.instance.posy);
            else
                MapManager.instance.RemoveEventOn(caller.PosX, caller.PosY, caller.MapId - 1);
        else
            MapManager.instance.RemoveEventOn((int)(data / 100 % 100), (int)(data % 100), (int)(data / 10000));
        return false;
    }
}
