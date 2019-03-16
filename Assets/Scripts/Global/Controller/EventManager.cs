using System.Collections.Generic;
public class EventManager {

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
        None = 0,           //无事件
        NormalBattle = 1,   //战斗
        OpenFreeDoor = 2,   //免费门
        NormalSend = 3,     //传送或楼梯
        NormalChat = 4,     //对话或提示
        NormalChoice = 5,   //选择性对话
        CallGame = 6,       //小游戏
        GetBaseResourceItem = 7,    //获取固定基本资源
        GetFunctionItem = 8,    //获取特殊用途道具
        OpenNormalDoor = 9,     //打开普通门
        RemoveEvent = 10,       //移除指定处的事件
        GetVariableResource = 11,   //获取变价资源，用于商店，此事件考虑重构时是否与7合并
    }

    /// <summary>
    /// 定义变价资源的价格缓存位，每个缓存位有其独特的变价算法
    /// </summary>
    public enum VariablePriceType
    {
        NoChange = 0, // 默认值，不变价
        GoldenIncreasing = 1, // 每次+1价格，用于普通金币商店
        KeyStoreDoubling = 2, // 每次价格乘以2，用于后期钥匙商店
    }

    public readonly Dictionary<EventName, Constant.EventCallback> eventList = null;

    // The special event callbacks are below
    private bool OnSend(Modal caller, long eventData)
    {
        int posx = (int)(eventData / 100 % 100);
        int posy = (int)(eventData % 100);
        int mapId = (int)(eventData / 10000);
        if (mapId > 0 && mapId != Game.Map.CurrentMap.mapId)
        {
            Game.Controller.Audio.PlaySound(AudioController.stairSound);
            var status = Game.Data.RuntimeData.Status;
            Game.Map.ShowLoadingCurtain(() =>
            {
                Game.Map.ShowMap(mapId);
                Game.Player.ShowPlayer(posx, posy);
                return true;
            }, () =>
            {
                Game.Data.RuntimeData.Status = status;
                return false;
            });
            return true;
        }
        Game.Player.ShowPlayer(posx, posy);
        return false;
    }

    private bool OnGetBaseResourceItem(Modal caller, long eventData) {
        var lastStatus = Game.Data.RuntimeData.Status;
        Game.Data.RuntimeData.Status = Constant.EGameStatus.OnEvent;
        var type = (Constant.ResourceType)(eventData % 100);
        var count = (int)(eventData / 100);
        Game.Player.ChangePlayerData(type, count);
        Game.Controller.Audio.PlaySound(AudioController.itemGetSound);
        caller.RemoveSelf(()=> { Game.Data.RuntimeData.Status = lastStatus; });
        return false;
    }

    private bool OnGetFunctionItem(Modal caller, long eventData) {
        // TODO
        return false;
    }

    private bool OnBattle(Modal caller, long eventData) {
        MainScene.instance.StartBattle(caller.Uuid);
        return false;
    }

    private bool OnChat(Modal caller, long eventData) {
        var data = Game.Data.Config.chats[(int)eventData];
        MainScene.instance.ChatBegan(data, caller);
        return data.canOn;
    }

    private bool OnChoice(Modal caller, long eventData) {
        int choiceId = (int)eventData;
        var data = Game.Data.Config.choices[choiceId];
        MainScene.instance.StartChoice(data, caller);
        return false;
    }

    private bool OnGame(Modal caller, long eventData) {
        int gameId = (int)eventData;
        // TODO

        return false;
    }

    private bool OpenFreeDoor(Modal caller, long blockData) {
        var lastStatus = Game.Data.RuntimeData.Status;
        Game.Data.RuntimeData.Status = Constant.EGameStatus.OnEvent;
        Game.Controller.Audio.PlaySound(AudioController.openDoorSound);
        caller.GoToRunState(() => { Game.Data.RuntimeData.Status = lastStatus; });
        return false;
    }

    private bool OpenNormalDoor(Modal caller, long data) {
        switch (data) {
            case 9:
                if (Game.Player.YellowKey <= 0)
                    return false;
                --Game.Player.YellowKey;
                break;
            case 10:
                if (Game.Player.BlueKey <= 0)
                    return false;
                --Game.Player.BlueKey;
                break;
            case 11:
                if (Game.Player.RedKey <= 0)
                    return false;
                --Game.Player.RedKey;
                break;
            case 12: // TODO: have not set the green door's modal
                if (Game.Player.GreenKey <= 0)
                    return false;
                --Game.Player.GreenKey;
                break;
            case 13:    // brozen
                        // TODO
            case 14:   // silver
                       // TODO
            case 15:    // gold
                        // TODO
                return false;
        }
        var lastStatus = Game.Data.RuntimeData.Status;
        Game.Data.RuntimeData.Status = Constant.EGameStatus.OnEvent;
        Game.Controller.Audio.PlaySound(AudioController.openDoorSound);
        caller.GoToRunState(() => { Game.Data.RuntimeData.Status = lastStatus; });
        return false;
    }

    private bool RemoveEventAtBlock(Modal caller, long data) {
        if (data == 0)
            if (caller == null)
                Game.Map.RemoveEventOn(Game.Player.PlayerPosX, Game.Player.PlayerPosY);
            else
                Game.Map.RemoveEventOn(caller.PosX, caller.PosY, caller.MapId);
        else
            Game.Map.RemoveEventOn((int)(data / 100 % 100), (int)(data % 100), (int)(data / 10000));
        return false;
    }

    ////////////// 辅助函数 ////////////////////////////////////
    
}
