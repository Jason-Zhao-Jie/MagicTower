using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.ViewUtil;
using ArmyAnt.ViewUtil.Components;
using MagicTower.Components.Unit;

namespace MagicTower.Components.DataEditor
{

    public class ModalSelectorDlg : MonoBehaviour
    {
        public delegate void SelectedCallback(int selectedId);
        
        // Start is called before the first frame update
        void Awake()
        {
            foreach (var v in Game.Config.modals)
            {
                var item = ModalList.PushbackDefaultItem();
                item.transform.Find("Image").GetComponent<Image>().sprite = Game.GetMods(Game.Config.modals[nowId].prefabPath)[0];
                item.transform.Find("Image").GetComponent<Button>().onClick.RemoveAllListeners();
                item.transform.Find("Image").GetComponent<Button>().onClick.AddListener(() => { OnModalSelected(item); });
                item.transform.Find("Text").GetComponent<Text>().text = Game.Config.StringInternational.GetValue(v.Value.name);
                item.GetComponent<UserData>().SetIntegerData(v.Value.id);
                item.GetComponent<UserData>().SetStringData(v.Value.name);
            }
        }

        private void Start()
        {
            if (selectedItem == null && showed)
            {
                if (nowId == 0)
                {
                    NowModal.sprite = null;
                    NowModalName.text = "none";
                }
                else
                {
                    NowModal.sprite = Game.GetMods(Game.Config.modals[nowId].prefabPath)[0];
                    NowModalName.text = Game.Config.StringInternational.GetValue(Game.Config.modals[nowId].name);
                }
                OnModalSelected(NowShowing);
            }
        }

        public void Init(int nowModalId, SelectedCallback callback) {
            // 设定信息
            nowId = nowModalId;
            selectedCallback = callback;
            selectedItem = null;
            if(isActiveAndEnabled) {
                if(nowModalId == 0) {
                    NowModal.sprite = null;
                    NowModalName.text = "none";
                } else {
                    NowModal.sprite = Game.GetMods(Game.Config.modals[nowModalId].prefabPath)[0];
                    NowModalName.text = Game.Config.StringInternational.GetValue(Game.Config.modals[nowModalId].name);
                }
                OnModalSelected(NowShowing);
            }
            showed = true;
            lastStatus = Game.Status;
            Game.Status = Model.EGameStatus.InEditorDialog;
        }

        public void OnModalSelected(RectTransform sender)
        {
            if (sender == SelectedShowing)   // 点击了右上角原物品
            {
                if (selectedItem != null)
                {
                    nowId = selectedItem.GetComponent<UserData>().GetIntegerData();
                }
                selectedCallback(nowId);
                Game.Status = lastStatus;
                gameObject.SetActive(false);
                return;
            }
            if (selectedItem != null)
            {
                selectedItem.GetComponent<Image>().color = whiteHalf;
            }
            if (sender == NowShowing) // 点击了左上角原物品
            {
                foreach(var i in ModalList)
                {
                    if(i.GetComponent<UserData>().GetIntegerData() == nowId)
                    {
                        sender = i;
                        break;
                    }
                }
                ModalList.ScrollToItem(sender);
            }
            selectedItem = sender;
            sender.GetComponent<Image>().color = Color.blue;
            SelectedModal.sprite = sender.transform.Find("Image").GetComponent<Image>().sprite;
            SelectedModalName.text = Game.Config.StringInternational.GetValue(sender.GetComponent<UserData>().GetStringData());
        }

        // Update is called once per frame
        void Update()
        {

        }

        public RectTransform NowShowing;
        public Image NowModal;
        public Text NowModalName;
        public RectTransform SelectedShowing;
        public Image SelectedModal;
        public Text SelectedModalName;
        public ListView ModalList;
        
        private int nowId;
        private bool showed = false;
        private RectTransform selectedItem;
        private SelectedCallback selectedCallback;
        private Model.EGameStatus lastStatus;
        private static readonly Color whiteHalf = new Color(1, 1, 1, 0.5f);
    }
}
