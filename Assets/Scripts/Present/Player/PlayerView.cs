using UnityEngine;
using UnityEngine.UI;

public class PlayerView : AView
{
    private const string str_level_title = "str_ui_level";
    private const string str_exp_title = "str_ui_exp";
    public const string str_life_title = "str_ui_life";
    public const string str_attack_title = "str_ui_attack";
    public const string str_defense_title = "str_ui_defense";
    public const string str_speed_title = "str_ui_speed";

    public PlayerView() : base(null)
    {
    }

    void Start()
    {
        levelTitleText.text = Game.Config.StringInternational.GetValue(str_level_title);
        expTitleText.text = Game.Config.StringInternational.GetValue(str_exp_title);
        lifeTitleText.text = Game.Config.StringInternational.GetValue(str_life_title);
        attackTitleText.text = Game.Config.StringInternational.GetValue(str_attack_title);
        defenseTitleText.text = Game.Config.StringInternational.GetValue(str_defense_title);
        speedTitleText.text = Game.Config.StringInternational.GetValue(str_speed_title);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public Player ShowPlayer(int playerId, bool reset = false)
    {
        if (reset)
        {
            if (Player != null)
                Player.RemoveSelf();
            Player = null;
        }
        var modalData = Game.Config.modals[playerId];
        if (Player == null || reset)
        {
            Player = Game.ObjPool.GetAnElement<Player>(modalData.id, ObjectPool.ElementType.Sprite, Constant.PREFAB_DIR + modalData.prefabPath);
            Player.MainPlayer = true;
        }
        if (HasStarted)
        {
            (Controller as PlayerController)?.SyncPlayerData();
        }
        return Player;
    }

    public bool HasStarted
    {
        get { return mapNameText != null && roleNameText != null; }
    }

    public string MapName
    {
        get { return mapNameText.text; }
        set { mapNameText.text = value; }
    }

    public string RoleName
    {
        get { return roleNameText.text; }
        set { roleNameText.text = value; }
    }

    public Sprite Portrait
    {
        get { return portrait.sprite; }
        set { portrait.sprite = value; }
    }

    public string Level
    {
        get { return levelText.text; }
        set { levelText.text = value; }
    }

    public string Experience
    {
        get { return expText.text; }
        set { expText.text = value; }
    }

    public string Life
    {
        get { return lifeText.text; }
        set { lifeText.text = value; }
    }

    public string Attack
    {
        get { return attackText.text; }
        set { attackText.text = value; }
    }

    public string Defense
    {
        get { return defenseText.text; }
        set { defenseText.text = value; }
    }

    public string Speed
    {
        get { return speedText.text; }
        set { speedText.text = value; }
    }

    public string Gold
    {
        get { return goldText.text; }
        set { goldText.text = value; }
    }

    public string YellowKey
    {
        get { return yellowKeyText.text; }
        set { yellowKeyText.text = value; }
    }

    public string BlueKey
    {
        get { return blueKeyText.text; }
        set { blueKeyText.text = value; }
    }

    public string RedKey
    {
        get { return redKeyText.text; }
        set { redKeyText.text = value; }
    }

    public Player Player { get; private set; }

    [Tooltip("地图楼层名字对象")]
    [Space(4)]
    public Text mapNameText;
    [Tooltip("玩家角色名字对象")]
    [Space(4)]
    public Text roleNameText;
    [Tooltip("玩家角色头像")]
    [Space(4)]
    public Image portrait;
    [Tooltip("等级标题对象")]
    [Space(4)]
    public Text levelTitleText;
    [Tooltip("等级数字对象")]
    [Space(4)]
    public Text levelText;
    [Tooltip("经验值标题对象")]
    [Space(4)]
    public Text expTitleText;
    [Tooltip("经验值对象")]
    [Space(4)]
    public Text expText;
    [Tooltip("生命值标题对象")]
    [Space(4)]
    public Text lifeTitleText;
    [Tooltip("生命值对象")]
    [Space(4)]
    public Text lifeText;
    [Tooltip("攻击值标题对象")]
    [Space(4)]
    public Text attackTitleText;
    [Tooltip("攻击值对象")]
    [Space(4)]
    public Text attackText;
    [Tooltip("防御值标题对象")]
    [Space(4)]
    public Text defenseTitleText;
    [Tooltip("防御值对象")]
    [Space(4)]
    public Text defenseText;
    [Tooltip("敏捷值标题对象")]
    [Space(4)]
    public Text speedTitleText;
    [Tooltip("敏捷值对象")]
    [Space(4)]
    public Text speedText;
    [Tooltip("金币数量对象")]
    [Space(4)]
    public Text goldText;
    [Tooltip("黄钥匙数量对象")]
    [Space(4)]
    public Text yellowKeyText;
    [Tooltip("蓝钥匙数量对象")]
    [Space(4)]
    public Text blueKeyText;
    [Tooltip("红钥匙数量对象")]
    [Space(4)]
    public Text redKeyText;
}
