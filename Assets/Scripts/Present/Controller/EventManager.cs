using System.Collections.Generic;
public class EventManager
{
    public EventManager() {
        eventList = new Dictionary<EventName, EventCallback>{
            {EventName.NormalBattle, OnBattle},
            {EventName.OpenFreeDoor, OpenFreeDoor},
            {EventName.NormalSend, OnSend},
            {EventName.NormalChat, OnChat},
            {EventName.NormalChoice, OnChoice},
            {EventName.CallGame, OnGame},
            {EventName.GetBaseResourceItem, OnGetBaseResourceItem},
            {EventName.PlaySoundBefore, OnPlaySound},
            {EventName.OpenNormalDoor, OpenNormalDoor},
            {EventName.RemoveEvent, RemoveEventAtBlock},
            {EventName.RemoveSelf, RemoveThingAtBlock},
        };
    }

    public bool DispatchEvent(int eventId, Modal caller, long[] eventData) {
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
        PlaySoundBefore = 8,    //播放音效，然后若可能的话执行下一个事件
        OpenNormalDoor = 9,     //打开普通门
        RemoveEvent = 10,       //移除指定处的事件
        RemoveSelf = 11,    // 移除caller自身，例如战斗后移除，获取宝石后移除等
    }

    public IEnumerable<EventName> EventKeyStrings { get { return eventList.Keys; } }

    private delegate bool EventCallback(Modal caller, long[] eventData);

    private readonly Dictionary<EventName, EventCallback> eventList = null;

    // The special event callbacks are below
    private bool OnSend(Modal caller, long[] eventData)
    {
        int mapId = (int)eventData[0];
        int posx = (int)eventData[1];
        int posy = (int)eventData[2];
        if (mapId > 0 && mapId != Game.Map.CurrentMap.mapId)
        {
            Game.Managers.Audio.PlaySound(AudioManager.stairSound);
            var status = Game.Status;
            Game.Map.ShowLoadingCurtain(() =>
            {
                Game.Map.ShowMap(mapId);
                Game.Player.ShowPlayer(posx, posy);
                return true;
            }, () =>
            {
                Game.Status = status;
                return false;
            });
            return true;
        }
        Game.Player.ShowPlayer(posx, posy);
        return false;
    }

    private bool OnGetBaseResourceItem(Modal caller, long[] eventData)
    {
        /// 解析 event data 数据
        int index = 0;
        // 要添加的资源
        Constant.ResourceType type = (Constant.ResourceType)System.Convert.ToByte(eventData[index++]);
        int count = System.Convert.ToInt32(eventData[index++]);
        // 交易成功的回调事件
        int successEventId = 0;
        long[] successEventData = null;
        if (eventData.Length > index)
        {
            successEventId = System.Convert.ToInt32(eventData[index++]);
        }
        if (eventData.Length > index)
        {
            successEventData = new long[eventData[index++]];
            for (var i = 0; i < successEventData.Length; ++i)
            {
                successEventData[i] = eventData[index++];
            }
        }
        // 要扣除的花费 TODO : 目前仅支持扣除一种资源，若有多项扣除需求再进行修改
        Constant.ResourceType costType = 0;
        int costId = 0;
        if (eventData.Length > index)
        {
            costType = (Constant.ResourceType)System.Convert.ToByte(eventData[index++]);
            costId = System.Convert.ToInt32(eventData[index++]);
        }
        // 花费价格的变化方式
        Game.VariablePriceType priceIncreaseType = Game.VariablePriceType.NoChange;
        if (eventData.Length > index)
        {
            priceIncreaseType = (Game.VariablePriceType)System.Convert.ToInt32(eventData[index++]);
        }
        int costCount = costId;
        if(priceIncreaseType != Game.VariablePriceType.NoChange)
        {
            costCount = System.Convert.ToInt32(Game.GetNumberData(costId));
        }
        // 要满足的其他条件, 不满足则失败。 TODO : 目前是要求全部满足，若有其他逻辑需求再进行修改
        Dictionary<Constant.ResourceType, int> conditions = null;
        if (eventData.Length > index)
        {
            var currentLength = eventData[index++];
            if (currentLength > 0)
            {
                conditions = new Dictionary<Constant.ResourceType, int>();
                for (var i = 0; i < currentLength; ++i)
                {
                    conditions.Add((Constant.ResourceType)System.Convert.ToByte(eventData[index++]), System.Convert.ToInt32(eventData[index++]));
                }
            }
        }
        // 交易失败的回调事件
        int failedEventId = 0;
        long[] failedEventData = null;
        if (eventData.Length > index)
        {
            failedEventId = System.Convert.ToInt32(eventData[index++]);
        }
        if (eventData.Length > index)
        {
            failedEventData = new long[eventData[index++]];
            for (var i = 0; i < failedEventData.Length; ++i)
            {
                failedEventData[i] = eventData[index++];
            }
        }

        /// 开始执行交易
        var lastStatus = Game.Status;
        Game.Status = Constant.EGameStatus.OnEvent;
        // 检查附加条件
        bool ok = Game.Player.CheckPlayerData(costType, costCount);
        if(conditions != null)
        {
            foreach(var i in conditions)
            {
                if(!Game.Player.CheckPlayerData(i.Key, i.Value))
                {
                    ok = false;
                    break;
                }
            }
        }
        //条件不满足, 失败
        if (!ok)
        {
            Game.Status = lastStatus;
            var ret = DispatchEvent(failedEventId, caller, failedEventData);
            // TODO : 目前这个交易结束事件的返回值无效，交易恒返回false
        }
        else
        {
            // 扣除花费
            if (priceIncreaseType == Game.VariablePriceType.NoChange)
            {
                Game.Player.ChangePlayerData(costType, -costId);
            }
            else
            {
                Game.Player.ChangePlayerData(costType, -System.Convert.ToInt32(Game.GetNumberData(costId, priceIncreaseType)));
            }
            // 获得交易物   TODO : 需要改进以支持特殊交易物的获得，例如特殊道具，或者职业属性等等
            Game.Player.ChangePlayerData(type, count);
            Game.Status = lastStatus;
            // 调取成功交易事件
            var ret = DispatchEvent(successEventId, caller, successEventData);
            // TODO : 目前这个交易结束事件的返回值无效，交易恒返回false
        }
        return false;
    }

    private bool OnPlaySound(Modal caller, long[] eventData)
    {
        Game.Managers.Audio.PlaySound(System.Convert.ToInt32(eventData[0]));
        if (eventData.Length > 1)
        {
            int lastEventId = System.Convert.ToInt32(eventData[1]);
            long[] lastEventData = null;
            if (eventData.Length > 2)
            {
                lastEventData = new long[eventData.Length - 2];
                for(var i = 2; i < eventData.Length; ++i)
                {
                    lastEventData[i - 2] = eventData[i];
                }
            }
            return DispatchEvent(lastEventId, caller, lastEventData);
        }
        return false;
    }

    private bool OnBattle(Modal caller, long[] eventData) {
        (Game.CurrentScene as MainScene)?.StartBattle(caller);
        return false;
    }

    private bool OnChat(Modal caller, long[] eventData) {
        var data = Game.Config.chats[System.Convert.ToInt32(eventData[0])];
        (Game.CurrentScene as MainScene)?.ChatBegan(data, caller);
        return data.canOn;
    }

    private bool OnChoice(Modal caller, long[] eventData) {
        int choiceId = System.Convert.ToInt32(eventData[0]);
        var data = Game.Config.choices[choiceId];
        (Game.CurrentScene as MainScene)?.StartChoice(data, caller);
        return false;
    }

    private bool OnGame(Modal caller, long[] eventData) {
        var gameId = System.Convert.ToInt32(eventData[0]);
        // TODO

        return false;
    }

    private bool OpenFreeDoor(Modal caller, long[] eventData) {
        var lastStatus = Game.Status;
        Game.Status = Constant.EGameStatus.OnEvent;
        Game.Managers.Audio.PlaySound(AudioManager.openDoorSound);
        caller.GoToRunState(() => { Game.Status = lastStatus; });
        return false;
    }

    private bool OpenNormalDoor(Modal caller, long[] eventData) {
        switch (eventData[0]) {
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
        var lastStatus = Game.Status;
        Game.Status = Constant.EGameStatus.OnEvent;
        Game.Managers.Audio.PlaySound(AudioManager.openDoorSound);
        caller.GoToRunState(() => { Game.Status = lastStatus; });
        return false;
    }

    private bool RemoveEventAtBlock(Modal caller, long[] data) {
        if (data == null || data.Length < 3)
            if (caller == null)
                Game.Map.RemoveEventOn(Game.Player.PlayerPosX, Game.Player.PlayerPosY);
            else
                Game.Map.RemoveEventOn(caller.PosX, caller.PosY, caller.MapId);
        else
            Game.Map.RemoveEventOn((int)data[1], (int)data[2], (int)data[0]);
        return false;
    }

    private bool RemoveThingAtBlock(Modal caller, long[] data)
    {
        var lastStatus = Game.Status;
        Game.Status = Constant.EGameStatus.OnEvent;
        caller.RemoveSelf(() => { Game.Status = lastStatus; });
        return false;
    }

    ////////////// 辅助函数 ////////////////////////////////////

}
