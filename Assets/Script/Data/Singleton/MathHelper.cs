public static class MathHelper
{
	public const int MAX_HURT_ATTACK = 40;
	
    /// <summary>
    /// 预测伤害数据结构体
    /// </summary>
    public struct BattleForecast
    {
        public int rounds;
        public int damage;
    }

    /// <summary>
    /// 计算单次伤害
    /// </summary>
    /// <param name="attack">伤害者攻击</param>
    /// <param name="critical">伤害者暴击</param>
    /// <param name="targetDefense">受伤者防御</param>
    /// <param name="targetSpeed">受伤者敏捷</param>
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
    /// 计算预测将会受到的总伤害
    /// </summary>
    /// <param name="selfAttack">我方攻击力</param>
    /// <param name="selfDefense">我方防御力</param>
    /// <param name="selfSpeed">我方敏捷</param>
    /// <param name="selfCritical">我方暴击</param>
    /// <param name="tarAttack">敌方攻击</param>
    /// <param name="tarDefense">敌方防御</param>
    /// <param name="tarSpeed">敌方敏捷</param>
    /// <param name="tarCritical">敌方暴击</param>
    /// <param name="tarLife">敌方生命值</param>
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
            damage = (int)tarDamage * (tarLife - 1) / (int)selfDamage,
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
    public static UnityEngine.Vector2Int[] AutoFindBestRoad(int[][] mapBlockData, int startPosx, int startPosy, int endPosx, int endPosy)
    {
        if (startPosx == endPosx && startPosy == endPosy)
            return null;
        if(IsPosNearly(new UnityEngine.Vector2Int(startPosx, startPosy), new UnityEngine.Vector2Int(endPosx, endPosy)))
        {
            return new UnityEngine.Vector2Int[] { new UnityEngine.Vector2Int(startPosx, startPosy), new UnityEngine.Vector2Int(endPosx, endPosy) };
        }

        var ret = new System.Collections.Generic.List<UnityEngine.Vector2Int>();
        
        // 搜寻所有可到达路径
        mapBlockData[startPosx][startPosy] = 9;
        var ableRoad = new LinkedTree<UnityEngine.Vector2Int, int>(new UnityEngine.Vector2Int(startPosx, startPosy), 9, null);
        ITree<UnityEngine.Vector2Int, int>[] nextStep = new LinkedTree<UnityEngine.Vector2Int, int>[] { ableRoad };
        while (nextStep != null && nextStep.Length > 0)
        {
            var newNextStep = new System.Collections.Generic.List<ITree<UnityEngine.Vector2Int, int>>();
            foreach (var stepElem in nextStep)
            {
                // 广度优先遍历
                var founded = GetNearlyPos(mapBlockData, stepElem.Tag.x, stepElem.Tag.y);
                int realUsefulCount = 0;
                // 逐个记录路径
                foreach (var elem in founded)
                {
                    int mapValue = mapBlockData[elem.x][elem.y];
                    if (mapValue < 9)
                    {
                        mapBlockData[elem.x][elem.y] = 9;
                        stepElem.AddChild(elem, mapValue);
                        realUsefulCount += 1;
                    }
                    if(elem.x != endPosx || elem.y != endPosy)
                    {
                        newNextStep.Add(stepElem[elem]);
                    }
                }
                if(realUsefulCount <= 0)
                {
                    var parent = stepElem.GetParent();
                    parent.RemoveChild(stepElem.Tag);
                    while(parent.ChildrenCount <= 0)
                    {
                        var newParent = parent.GetParent();
                        if (newParent == null)
                            return null;
                        newParent.RemoveChild(parent.Tag);
                        parent = newParent;
                    }
                }
            }
            nextStep = newNextStep.ToArray();
        }

        // 从可选路径中搜寻最佳路径
        ableRoad.EnumeratorType = ETreeTraversalWay.LeavesOnly;
        if (ableRoad.ChildrenCount == 0)
            return null;
        ITree<UnityEngine.Vector2Int, int>[] road = null;
        int maxVal = 9;
        int maxNum = 999;
        foreach(var leaf in ableRoad)
        {
            var tmpRoad = leaf.GetBranchRoad();
            int tmpMaxVal = 0;
            int tmpMaxNum = 0;
            foreach (var roadBlock in tmpRoad)
            {
                if (roadBlock.ChildrenCount <= 0 || roadBlock == tmpRoad[0])
                    continue;
                else if(roadBlock.Value > tmpMaxVal)
                {
                    tmpMaxVal = roadBlock.Value;
                    tmpMaxNum = 1;
                }else if(roadBlock.Value == tmpMaxVal)
                {
                    tmpMaxNum++;
                }
            }
            if(tmpMaxVal < maxVal)
            {
                road = tmpRoad;
                maxVal = tmpMaxVal;
                maxNum = tmpMaxNum;
            }else if(tmpMaxVal == maxVal && tmpMaxNum < maxNum)
            {
                road = tmpRoad;
                maxNum = tmpMaxNum;
            }
        }

        // 记录最佳路径
        if (maxVal == 9 || road == null || road.Length <= 0)
            return null;
        foreach(var i in road)
        {
            ret.Add(i.Tag);
        }

        return ret.ToArray();
    }

    /// <summary>
    /// 寻找邻近可走地图块
    /// </summary>
    /// <param name="mapBlockData">整个地图的信息, 由 <see cref="MapManager.ConvertCurrentMapToFinderArray()"/> 转化而来</param>
    /// <param name="startPos">要搜索的源位置</param>
    /// <returns>
    ///     返回一个数组
    /// </returns>
    private static UnityEngine.Vector2Int[] GetNearlyPos(int[][] mapBlockData, int posx, int posy)
    {
        var ret = new System.Collections.Generic.List<UnityEngine.Vector2Int>();
        // 左边块
        if (posx > 0)
        {
            var leftInfo = mapBlockData[posx - 1][posy];
            if (leftInfo < 9)
                ret.Add(new UnityEngine.Vector2Int(posx - 1, posy));
        }
        // 右边块
        if (posx < mapBlockData.Length - 1)
        {
            var rightInfo = mapBlockData[posx + 1][posy];
            if (rightInfo < 9)
                ret.Add(new UnityEngine.Vector2Int(posx + 1, posy));
        }
        // 下边块
        if (posy > 0)
        {
            var downInfo = mapBlockData[posx][posy - 1];
            if (downInfo < 9)
                ret.Add(new UnityEngine.Vector2Int(posx, posy - 1));
        }
        // 上边块
        if (posy < mapBlockData[posx].Length - 1)
        {
            var upInfo = mapBlockData[posx][posy + 1];
            if (upInfo < 9)
                ret.Add(new UnityEngine.Vector2Int(posx, posy + 1));
        }
        ret.Sort((UnityEngine.Vector2Int x, UnityEngine.Vector2Int y) =>
        {
            return mapBlockData[x.x][x.y] - mapBlockData[y.x][y.y];
        });
        return ret.ToArray();
    }

    private static bool IsPosNearly(UnityEngine.Vector2Int pos1, UnityEngine.Vector2Int pos2)
    {
        return ((pos1.x == pos2.x) && (System.Math.Abs(pos1.y - pos2.y) <= 1)) || ((System.Math.Abs(pos1.x - pos2.x) <= 1) && (pos1.y == pos2.y));
    }
}
