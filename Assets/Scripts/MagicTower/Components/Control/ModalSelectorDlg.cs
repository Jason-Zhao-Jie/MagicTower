using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.ViewUtil;
using ArmyAnt.ViewUtil.Components;
using MagicTower.Components.Unit;

namespace MagicTower.Components.Control
{

    public class ModalSelectorDlg : ObjectPool.AViewUnit
    {
        public const string PREFAB_DIR = "ModalSelectorDlg";
        public const int PREFAB_ID = 6;

        public delegate void SelectedCallback(int selectedId);

        public static ModalSelectorDlg ShowDialog(Transform parent, int nowModalId, SelectedCallback callback)
        {
            // 弹出战斗框
            var ret = Game.ObjPool.GetAnElement<ModalSelectorDlg>(PREFAB_ID, ObjectPool.ElementType.Dialog, GetResourcePath());
            // 设定信息
            ret.nowId = nowModalId;
            ret.selectedCallback = callback;
            ret.selectedItem = null;
            if (ret.isActiveAndEnabled)
            {
                if (nowModalId == 0)
                {
                    ret.NowModal.sprite = null;
                    ret.NowModalName.text = "none";
                }
                else
                {
                    ret.NowModal.sprite = Modal.GetResourceBaseSprite(nowModalId);
                    ret.NowModalName.text = Game.Config.StringInternational.GetValue(Game.Config.modals[nowModalId].name);
                }
                ret.OnModalSelected(ret.NowShowing);
            }
            ret.showed = true;
            ret.transform.SetParent(parent, false);
            ret.transform.SetSiblingIndex(1);
            ret.transform.localPosition = new Vector3(0, 0, ret.transform.localPosition.z);
            ret.lastStatus = Game.Status;
            Game.Status = Model.EGameStatus.InEditorDialog;
            return ret;
        }

        public override string ResourcePath { get { return Model.Dirs.DIALOG_DIR + PREFAB_DIR; } }
        public static string GetResourcePath() { return Model.Dirs.DIALOG_DIR + PREFAB_DIR; }

        public override ObjectPool.ElementType GetPoolTypeId()
        {
            return ObjectPool.ElementType.Dialog;
        }

        public override bool OnCreate(ObjectPool.ElementType tid, int elemId, string resourcePath)
        {
            return true;
        }

        public override void OnReuse(ObjectPool.ElementType tid, int elemId)
        {
        }

        public override bool OnUnuse(ObjectPool.ElementType tid, int elemId)
        {
            showed = false;
            Game.Status = lastStatus;
            return true;
        }

        public override bool RecycleSelf()
        {
            return Game.ObjPoolRecycleSelf(this);
        }

        // Start is called before the first frame update
        void Awake()
        {
            foreach (var v in Game.Config.modals)
            {
                var item = ModalList.PushbackDefaultItem();
                item.transform.Find("Image").GetComponent<Image>().sprite = Modal.GetResourceBaseSprite(v.Value.id);
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
                    NowModal.sprite = Modal.GetResourceBaseSprite(nowId);
                    NowModalName.text = Game.Config.StringInternational.GetValue(Game.Config.modals[nowId].name);
                }
                OnModalSelected(NowShowing);
            }
        }

        public void OnModalSelected(RectTransform sender)
        {
            if (sender == SelectedShowing)   // 点击了右上角原物品
            {
                if (selectedItem != null)
                {
                    selectedCallback(selectedItem.GetComponent<UserData>().GetIntegerData());
                    RecycleSelf();
                }
                else
                {
                    selectedCallback(nowId);
                    RecycleSelf();
                }
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
                if(sender == NowShowing)
                {
                    selectedCallback(nowId);
                    RecycleSelf();
                    return;
                }
            }
            selectedItem = sender;
            sender.GetComponent<Image>().color = Color.blue;
            SelectedModal.sprite = sender.transform.Find("Image").GetComponent<Image>().sprite;
            SelectedModalName.text = Game.Config.StringInternational.GetValue(sender.GetComponent<UserData>().GetStringData());
            ModalList.ScrollToItem(selectedItem);
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
