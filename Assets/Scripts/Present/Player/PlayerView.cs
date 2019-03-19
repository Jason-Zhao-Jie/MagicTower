using UnityEngine;
using UnityEngine.UI;

public class PlayerView : AView
{
    public PlayerView() : base(null)
    {
    }

    // Use this for initialization
    void Awake()
    {
        var heroPanel = transform.Find("HeroPanel");
        var itemPanel = transform.Find("ItemPanel");

        mapNameText = heroPanel.transform.Find("MapName").GetComponent<Text>();
        roleNameText = heroPanel.transform.Find("Name").GetComponent<Text>();
        portrait = heroPanel.transform.Find("Portrait").GetComponent<Image>();
        levelText = heroPanel.transform.Find("Level").GetComponent<Text>();
        expText = heroPanel.transform.Find("Exp").GetComponent<Text>();
        lifeText = heroPanel.transform.Find("Life").GetComponent<Text>();
        attackText = heroPanel.transform.Find("Attack").GetComponent<Text>();
        defenseText = heroPanel.transform.Find("Defense").GetComponent<Text>();
        speedText = heroPanel.transform.Find("Speed").GetComponent<Text>();
        goldText = itemPanel.transform.Find("Gold").GetComponent<Text>();
        yellowKeyText = itemPanel.transform.Find("YellowKey").GetComponent<Text>();
        blueKeyText = itemPanel.transform.Find("BlueKey").GetComponent<Text>();
        redKeyText = itemPanel.transform.Find("RedKey").GetComponent<Text>();
    }

    void Start()
    {

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

    private Text mapNameText;
    private Text roleNameText;
    private Image portrait;
    private Text levelText;
    private Text expText;
    private Text lifeText;
    private Text attackText;
    private Text defenseText;
    private Text speedText;
    private Text goldText;
    private Text yellowKeyText;
    private Text blueKeyText;
    private Text redKeyText;
}
