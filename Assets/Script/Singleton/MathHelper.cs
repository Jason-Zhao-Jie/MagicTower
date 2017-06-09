public static class MathHelper
{
	public const int MAX_HURT_ATTACK = 40;
	
    public struct BattleForecast
    {
        public int rounds;
        public int damage;
    }

    public static int GetHurt(int attack, float critical, int targetDefense, int targetSpeed)
    {
        var miss = new System.Random().Next(0, 99);
        if (miss <= targetSpeed)
            return -1;
        if (attack <= targetDefense)
            if (attack > targetDefense - MAX_HURT_ATTACK)
                return 1;
            else
                return 0;
        var damage = attack - targetDefense;
        var crit = new System.Random().Next(0, 999999);
        if (((float)crit) / 1000000 - critical < float.Epsilon)
            return -damage - damage / 2;
        return damage;
    }

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

}
