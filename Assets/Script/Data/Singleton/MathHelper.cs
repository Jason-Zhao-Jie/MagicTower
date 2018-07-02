public static class MathHelper
{
	public const int MAX_HURT_ATTACK = 40;
	
    /// <summary>
    /// Ԥ���˺����ݽṹ��
    /// </summary>
    public struct BattleForecast
    {
        public int rounds;
        public int damage;
    }

    /// <summary>
    /// ���㵥���˺�
    /// </summary>
    /// <param name="attack">�˺��߹���</param>
    /// <param name="critical">�˺��߱���</param>
    /// <param name="targetDefense">�����߷���</param>
    /// <param name="targetSpeed">����������</param>
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
    /// ����Ԥ�⽫���ܵ������˺�
    /// </summary>
    /// <param name="selfAttack">�ҷ�������</param>
    /// <param name="selfDefense">�ҷ�������</param>
    /// <param name="selfSpeed">�ҷ�����</param>
    /// <param name="selfCritical">�ҷ�����</param>
    /// <param name="tarAttack">�з�����</param>
    /// <param name="tarDefense">�з�����</param>
    /// <param name="tarSpeed">�з�����</param>
    /// <param name="tarCritical">�з�����</param>
    /// <param name="tarLife">�з�����ֵ</param>
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
    ///     �Զ�Ѱ·�㷨��ѧ����
    /// </summary>
    /// <param name="mapBlockData"> 
    ///     ������ͼ����Ϣ, �� <see cref="MapManager.ConvertCurrentMapToFinderArray()"/> ת������
    ///     ��ͼ����,
    ///     0��ʾ������ȼ����п�
    ///     2��ʾ�е��ߵ�·��, ���ȼ�����0
    ///     4��ʾ�й����·��, ���ȼ�����2
    ///     9��ʾ������·��, ����ǽ��, ��, NPC, ��һ���(������չ). ����˿��ѱ���һѰ··��ʹ��, Ҳ�ᱻ���Ϊ9, ��ֹ��·����Զ·
    /// </param>
    /// <param name="startPos"> Ѱ·��ʼ�� </param>
    /// <param name="endPos"> Ѱ·�յ�, �����Ѱ·�յ��Ƿ������ </param>
    /// <returns> ���·��. ��Ϊnull���ʾ�޷����� </returns>
    public static UnityEngine.Vector2Int[] AutoFindBestRoad(int[][] mapBlockData, int startPosx, int startPosy, int endPosx, int endPosy)
    {
        if (startPosx == endPosx && startPosy == endPosy)
            return null;
        if(IsPosNearly(new UnityEngine.Vector2Int(startPosx, startPosy), new UnityEngine.Vector2Int(endPosx, endPosy)))
        {
            return new UnityEngine.Vector2Int[] { new UnityEngine.Vector2Int(startPosx, startPosy), new UnityEngine.Vector2Int(endPosx, endPosy) };
        }

        var ret = new System.Collections.Generic.List<UnityEngine.Vector2Int>();
        
        // ��Ѱ���пɵ���·��
        mapBlockData[startPosx][startPosy] = 9;
        var ableRoad = new LinkedTree<UnityEngine.Vector2Int, int>(new UnityEngine.Vector2Int(startPosx, startPosy), 9, null);
        ITree<UnityEngine.Vector2Int, int>[] nextStep = new LinkedTree<UnityEngine.Vector2Int, int>[] { ableRoad };
        while (nextStep != null && nextStep.Length > 0)
        {
            var newNextStep = new System.Collections.Generic.List<ITree<UnityEngine.Vector2Int, int>>();
            foreach (var stepElem in nextStep)
            {
                // ������ȱ���
                var founded = GetNearlyPos(mapBlockData, stepElem.Tag.x, stepElem.Tag.y);
                int realUsefulCount = 0;
                // �����¼·��
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

        // �ӿ�ѡ·������Ѱ���·��
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

        // ��¼���·��
        if (maxVal == 9 || road == null || road.Length <= 0)
            return null;
        foreach(var i in road)
        {
            ret.Add(i.Tag);
        }

        return ret.ToArray();
    }

    /// <summary>
    /// Ѱ���ڽ����ߵ�ͼ��
    /// </summary>
    /// <param name="mapBlockData">������ͼ����Ϣ, �� <see cref="MapManager.ConvertCurrentMapToFinderArray()"/> ת������</param>
    /// <param name="startPos">Ҫ������Դλ��</param>
    /// <returns>
    ///     ����һ������
    /// </returns>
    private static UnityEngine.Vector2Int[] GetNearlyPos(int[][] mapBlockData, int posx, int posy)
    {
        var ret = new System.Collections.Generic.List<UnityEngine.Vector2Int>();
        // ��߿�
        if (posx > 0)
        {
            var leftInfo = mapBlockData[posx - 1][posy];
            if (leftInfo < 9)
                ret.Add(new UnityEngine.Vector2Int(posx - 1, posy));
        }
        // �ұ߿�
        if (posx < mapBlockData.Length - 1)
        {
            var rightInfo = mapBlockData[posx + 1][posy];
            if (rightInfo < 9)
                ret.Add(new UnityEngine.Vector2Int(posx + 1, posy));
        }
        // �±߿�
        if (posy > 0)
        {
            var downInfo = mapBlockData[posx][posy - 1];
            if (downInfo < 9)
                ret.Add(new UnityEngine.Vector2Int(posx, posy - 1));
        }
        // �ϱ߿�
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
