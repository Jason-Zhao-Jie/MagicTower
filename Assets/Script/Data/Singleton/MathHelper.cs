public static class MathHelper
{
	public const int MAX_HURT_ATTACK = 40;
	
    public struct BattleForecast
    {
        public int rounds;
        public int damage;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="attack"></param>
    /// <param name="critical"></param>
    /// <param name="targetDefense"></param>
    /// <param name="targetSpeed"></param>
    /// <returns> -1 means missing attack, others means real heat </returns>
    public static int GetHurt(int attack, double critical, int targetDefense, int targetSpeed)
    {
        var miss = new System.Random().Next(0, 99);
        if (miss <= targetSpeed)
            return -1;
        var damage = attack - targetDefense;
        if (attack <= targetDefense)
            if (attack > targetDefense - MAX_HURT_ATTACK)
                damage = 1;
            else
                return 0;
        var crit = new System.Random().Next(0, 999999);
        if (((double)crit) / 1000000 - critical/100 < double.Epsilon)
            return -damage - damage / 2;
        return damage;
    }

    /// <summary>
    /// Calculate the damage that will caught by enemy
    /// </summary>
    /// <param name="selfAttack"></param>
    /// <param name="selfDefense"></param>
    /// <param name="selfSpeed"></param>
    /// <param name="selfCritical"></param>
    /// <param name="tarAttack"></param>
    /// <param name="tarDefense"></param>
    /// <param name="tarSpeed"></param>
    /// <param name="tarCritical"></param>
    /// <param name="tarLife"></param>
    /// <returns></returns>
    public static BattleForecast CalcForeCast(int selfAttack, int selfDefense, int selfSpeed, float selfCritical,
                                              int tarAttack, int tarDefense, int tarSpeed, float tarCritical, int tarLife)
    {
        float selfDamage = 0;
        if (selfAttack <= tarDefense)
            if (selfAttack > tarDefense - MAX_HURT_ATTACK)
                selfDamage = 1;
            else
                selfDamage = 0;
        else
            selfDamage = (selfAttack - tarDefense) * (1 + selfCritical / 2) * (99 - tarSpeed) / 100;

        float tarDamage = 0;
        if (tarAttack <= selfDefense)
            if (tarAttack > selfDefense - MAX_HURT_ATTACK)
                tarDamage = 1;
            else
                tarDamage = 0;
        else
            tarDamage = (tarAttack - selfDefense) * (1 + tarCritical / 2) * (99 - selfSpeed) / 100;

        return new BattleForecast()
        {
            rounds = (tarLife - 1) / (int)selfDamage + 1,
            damage = (int)tarDamage * (tarLife - 1) / (int)selfDamage

        };
    }

    /// <summary>
    ///     自动寻路算法数学函数
    /// </summary>
    /// <param name="mapBlockData"> 
    ///     整个地图的信息, 由 <see cref="MapManager.ConvertCurrentMapToFinderArray()"/> 转化而来
    ///     地图块中,
    ///     0表示最高优先级可行块
    ///     2表示有道具的路块, 优先级低于0
    ///     4表示有怪物的路块, 优先级低于2
    ///     9表示不可行路块, 包括墙体, 门, NPC, 另一玩家(将来拓展). 如果此块已被另一寻路路线使用, 也会被标记为9, 防止回路或绕远路
    /// </param>
    /// <param name="startPos"> 寻路起始点 </param>
    /// <param name="endPos"> 寻路终点, 不检测寻路终点是否可行走 </param>
    /// <returns> 最佳路径. 如为null则表示无法到达 </returns>
    public static UnityEngine.Vector2[] AutoFindBestRoad(int[][] mapBlockData, int startPosx, int startPosy, int endPosx, int endPosy)
    {
        var ret = new System.Collections.Generic.List<UnityEngine.Vector2>();
        
        // TODO 最优路径寻路算法
        mapBlockData[startPosx][startPosy] = 9;
        var ableRoad = new System.Collections.Generic.List<System.Collections.Generic.List<UnityEngine.Vector2>>();
        // 广度优先遍历
        var founded = GetNearlyPos(mapBlockData, startPosx, startPosy);
        // 逐个记录路径
        foreach(var elem in founded)
        {

        }

        return ret.ToArray();
    }

    /// <summary>
    /// 寻找邻近可走地图块
    /// </summary>
    /// <param name="mapBlockData">整个地图的信息, 由 <see cref="MapManager.ConvertCurrentMapToFinderArray()"/> 转化而来</param>
    /// <param name="startPos">要搜索的源位置</param>
    /// <returns>
    ///     返回一个字典, 其键表示可用块的位置, 其值为一个数值, 表示了该地图块信息的等级
    /// </returns>
    private static System.Collections.Generic.Dictionary<UnityEngine.Vector2, int> GetNearlyPos(int[][] mapBlockData, int posx, int posy)
    {
        var ret = new System.Collections.Generic.Dictionary<UnityEngine.Vector2, int>();
        // 左边块
        if (posx > 0)
        {
            var leftInfo = mapBlockData[posx - 1][posy];
            if (leftInfo < 9)
                ret.Add(new UnityEngine.Vector2(posx - 1, posy), leftInfo);
        }
        // 右边块
        if (posx < mapBlockData.Length - 1)
        {
            var rightInfo = mapBlockData[posx + 1][posy];
            if (rightInfo < 9)
                ret.Add(new UnityEngine.Vector2(posx + 1, posy), rightInfo);
        }
        // 下边块
        if (posy > 0)
        {
            var downInfo = mapBlockData[posx][posy - 1];
            if (downInfo < 9)
                ret.Add(new UnityEngine.Vector2(posx, posy - 1), downInfo);
        }
        // 上边块
        if (posy < mapBlockData[posx].Length - 1)
        {
            var upInfo = mapBlockData[posx][posy + 1];
            if (upInfo < 9)
                ret.Add(new UnityEngine.Vector2(posx, posy + 1), upInfo);
        }
        return ret;
    }

}
