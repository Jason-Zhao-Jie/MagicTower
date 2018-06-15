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
    public static UnityEngine.Vector2[] AutoFindBestRoad(int[][] mapBlockData, int startPosx, int startPosy, int endPosx, int endPosy)
    {
        var ret = new System.Collections.Generic.List<UnityEngine.Vector2>();
        
        // TODO ����·��Ѱ·�㷨
        mapBlockData[startPosx][startPosy] = 9;
        var ableRoad = new System.Collections.Generic.List<System.Collections.Generic.List<UnityEngine.Vector2>>();
        // ������ȱ���
        var founded = GetNearlyPos(mapBlockData, startPosx, startPosy);
        // �����¼·��
        foreach(var elem in founded)
        {

        }

        return ret.ToArray();
    }

    /// <summary>
    /// Ѱ���ڽ����ߵ�ͼ��
    /// </summary>
    /// <param name="mapBlockData">������ͼ����Ϣ, �� <see cref="MapManager.ConvertCurrentMapToFinderArray()"/> ת������</param>
    /// <param name="startPos">Ҫ������Դλ��</param>
    /// <returns>
    ///     ����һ���ֵ�, �����ʾ���ÿ��λ��, ��ֵΪһ����ֵ, ��ʾ�˸õ�ͼ����Ϣ�ĵȼ�
    /// </returns>
    private static System.Collections.Generic.Dictionary<UnityEngine.Vector2, int> GetNearlyPos(int[][] mapBlockData, int posx, int posy)
    {
        var ret = new System.Collections.Generic.Dictionary<UnityEngine.Vector2, int>();
        // ��߿�
        if (posx > 0)
        {
            var leftInfo = mapBlockData[posx - 1][posy];
            if (leftInfo < 9)
                ret.Add(new UnityEngine.Vector2(posx - 1, posy), leftInfo);
        }
        // �ұ߿�
        if (posx < mapBlockData.Length - 1)
        {
            var rightInfo = mapBlockData[posx + 1][posy];
            if (rightInfo < 9)
                ret.Add(new UnityEngine.Vector2(posx + 1, posy), rightInfo);
        }
        // �±߿�
        if (posy > 0)
        {
            var downInfo = mapBlockData[posx][posy - 1];
            if (downInfo < 9)
                ret.Add(new UnityEngine.Vector2(posx, posy - 1), downInfo);
        }
        // �ϱ߿�
        if (posy < mapBlockData[posx].Length - 1)
        {
            var upInfo = mapBlockData[posx][posy + 1];
            if (upInfo < 9)
                ret.Add(new UnityEngine.Vector2(posx, posy + 1), upInfo);
        }
        return ret;
    }

}
