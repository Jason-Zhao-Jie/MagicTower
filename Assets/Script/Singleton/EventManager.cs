using System.Collections.Generic;
public class EventManager {
    public static EventManager instance = null;

    public EventManager() {
        eventList = new Dictionary<EventName, Constant.EventCallback>{
            {EventName.NormalBattle, OnBattle},
            {EventName.OpenFreeDoor, OpenFreeDoor},
            {EventName.NormalSend, OnSend},
            {EventName.NormalChat, OnChat},
            {EventName.NormalChoice, OnChoice},
            {EventName.CallGame, OnGame},
            {EventName.GetBaseResourceItem, OnGetBaseResourceItem},
            {EventName.GetFunctionItem, OnGetFunctionItem},
            {EventName.OpenNormalDoor, OpenNormalDoor},
            {EventName.RemoveEvent, RemoveEventAtBlock},
        };
    }

    public bool DispatchEvent(int eventId, Modal caller, long eventData) {
        if (eventId == 0 || !eventList.ContainsKey((EventName)eventId))
            return true;
        var cb = eventList[(EventName)eventId];
        return cb == null || cb(caller, eventData);
    }
    public enum EventName {
        None = 0,
        NormalBattle = 1,
        OpenFreeDoor = 2,
        NormalSend = 3,
        NormalChat = 4,
        NormalChoice = 5,
        CallGame = 6,
        GetBaseResourceItem = 7,
        GetFunctionItem = 8,
        OpenNormalDoor = 9,
        RemoveEvent = 10,
    }

    public readonly Dictionary<EventName, Constant.EventCallback> eventList = null;

    // The special event callbacks are below
    private bool OnSend(Modal caller, long eventData) {
        int posx = (int)(eventData / 100 % 100);
        int posy = (int)(eventData % 100);
        int mapId = (int)(eventData / 10000);
        if (mapId > 0 && mapId != MapManager.instance.CurrentMap.mapId) {
            AudioController.instance.PlaySound(AudioController.stairSound);
            MapManager.instance.ShowLoadingCurtain(() => {
                MapManager.instance.ShowMap(mapId);
                PlayerController.instance.ShowPlayer(posx, posy);
            });
            return true;
        } else {
            PlayerController.instance.ShowPlayer(posx, posy);
            return false;
        }
    }

    private bool OnGetBaseResourceItem(Modal caller, long eventData) {
        var lastStatus = DataCenter.instance.Status;
        DataCenter.instance.Status = Constant.EGameStatus.OnEvent;
        var type = (Constant.ResourceType)(eventData % 100);
        var count = (int)(eventData / 100);
        PlayerController.instance.ChangePlayerData(type, count);
        AudioController.instance.PlaySound(AudioController.itemGetSound);
        caller.RemoveSelf(()=> { DataCenter.instance.Status = lastStatus; });
        return false;
    }

    private bool OnGetFunctionItem(Modal caller, long eventData) {
        return false;
    }

    private bool OnBattle(Modal caller, long eventData) {
        MainScene.instance.StartBattle(caller.Uuid);
        return false;
    }

    private bool OnChat(Modal caller, long eventData) {
        var data = DataCenter.instance.chats[(int)eventData];
        MainScene.instance.ChatBegan(data, caller);
        return data.canOn;
    }

    private bool OnChoice(Modal caller, long eventData) {
        int choiceId = (int)eventData;
        var data = DataCenter.instance.choices[choiceId];
        MainScene.instance.ChoiceBegan(data, caller);
        return false;
    }

    private bool OnGame(Modal caller, long eventData) {
        int gameId = (int)eventData;

        return false;
    }

    private bool OpenFreeDoor(Modal caller, long blockData) {
        var lastStatus = DataCenter.instance.Status;
        DataCenter.instance.Status = Constant.EGameStatus.OnEvent;
        AudioController.instance.PlaySound(AudioController.openDoorSound);
        caller.GoToRunState(() => { DataCenter.instance.Status = lastStatus; });
        return false;
    }

    private bool OpenNormalDoor(Modal caller, long data) {
        switch (data) {
            case 9:
                if (PlayerController.instance.YellowKey <= 0)
                    return false;
                --PlayerController.instance.YellowKey;
                break;
            case 10:
                if (PlayerController.instance.BlueKey <= 0)
                    return false;
                --PlayerController.instance.BlueKey;
                break;
            case 11:
                if (PlayerController.instance.RedKey <= 0)
                    return false;
                --PlayerController.instance.RedKey;
                break;
            case 12: // TODO: have not set the green door's modal
                if (PlayerController.instance.GreenKey <= 0)
                    return false;
                --PlayerController.instance.GreenKey;
                break;
            case 13:    // brozen
            case 14:   // silver
            case 15:    // gold
                return false;
        }
        var lastStatus = DataCenter.instance.Status;
        DataCenter.instance.Status = Constant.EGameStatus.OnEvent;
        AudioController.instance.PlaySound(AudioController.openDoorSound);
        caller.GoToRunState(() => { DataCenter.instance.Status = lastStatus; });
        return false;
    }

    private bool RemoveEventAtBlock(Modal caller, long data) {
        if (data == 0)
            if (caller == null)
                MapManager.instance.RemoveEventOn(PlayerController.instance.posx, PlayerController.instance.posy);
            else
                MapManager.instance.RemoveEventOn(caller.PosX, caller.PosY, caller.MapId);
        else
            MapManager.instance.RemoveEventOn((int)(data / 100 % 100), (int)(data % 100), (int)(data / 10000));
        return false;
    }


}
