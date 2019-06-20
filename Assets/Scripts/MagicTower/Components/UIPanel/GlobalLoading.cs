using System.Collections.Generic;
using System.Collections.ObjectModel;
using MagicTower.Present.Manager;

using UnityEngine;

namespace MagicTower.Components {
    public enum UIType {
        Default,
        DataEditor,
        StartPanel,
        MainMenu,
        TipBar,
        TopChat,
        BottomChat,
        AlertDialog,
        InfoDialog,
        ChoiceDialog,
        BattleDialog,
        SaveLoadDialog,
        SettingDialog,
        ModalSelector,
    }

    namespace UIPanel {
        public class GlobalLoading : MonoBehaviour {
            // Start is called before the first frame update
            private void Awake() {
                // 加载 Sprites
                modSprites = new Dictionary<string, Sprite[]>();
                foreach(var i in modSpriteList) {
                    modSprites[i.name] = i.sp;
                    if(i.sp == null || i.sp.Length <= 0) {
                        Game.DebugLogError("The Modal ", i.name, " Has No Sprite");
                    } else {
                        for(var n = 0; n < i.sp.Length; ++n) {
                            if(i.sp[n] == null) {
                                Game.DebugLogError("The Modal ", i.name, " Has A Null Sprite In Index ", n);
                            }
                        }
                    }
                }
                modSpriteList = null;

                // 加载 Audio Clips
                audioClips = new Dictionary<string, AudioClip>();
                foreach(var i in audioList) {
                    audioClips[i.name] = i.clip;
                    if(i.clip == null) {
                        Game.DebugLogError("The Audio ", i.name, " Has A Null Clip Value");
                    }
                }
                modSpriteList = null;

                // 加载 Player Prefabs
                players = new Dictionary<string, GameObject>();
                foreach(var i in playerList) {
                    players[i.name] = i.prefab;
                    if(i.prefab == null) {
                        Game.DebugLogError("The Player ", i.name, " Has A Null Prefab Object");
                    }
                }
                modSpriteList = null;

                // 加载 UIs
                ui = new Dictionary<UIType, GameObject> {
                    { UIType.Default, null },
                    { UIType.DataEditor, dataEditorPanel },
                    { UIType.StartPanel, startPanel },
                    { UIType.MainMenu, mainMenu },
                    { UIType.TipBar, tipBar },
                    { UIType.TopChat, topChat },
                    { UIType.BottomChat, bottomChat },
                    { UIType.AlertDialog, alertDialog },
                    { UIType.InfoDialog, infoDialog },
                    { UIType.ChoiceDialog, choiceDialog },
                    { UIType.BattleDialog, battleDialog },
                    { UIType.SaveLoadDialog, saveLoadDialog },
                    { UIType.SettingDialog, settingDialog },
                    { UIType.ModalSelector, modalSelector },
                };
            }

            public GameObject ShowUI(UIType type) {
                if(showedUI.ContainsKey(type)) {
                    var ret = showedUI[type];
                    ret.SetActive(true);
                    return ret;
                } else {
                    var ret = Instantiate(ui[type], DialogCanvas);
                    showedUI.Add(type, ret);
                    ret.transform.SetAsLastSibling();
                    // ret.transform.localPosition = new Vector3(0, 0, ret.transform.localPosition.z);
                    return ret;
                }
            }

            public GameObject GetUI(UIType type) {
                if(showedUI.ContainsKey(type)) {
                    return showedUI[type];
                } else {
                    return null;
                }
            }

            public bool HideUI(UIType type) {
                if(showedUI.ContainsKey(type)) {
                    Destroy(showedUI[type]);
                    showedUI.Remove(type);
                    return true;
                }
                return false;
            }

            [HideInInspector]
            public Transform DialogCanvas;

            [System.Serializable]
            public struct SpriteInfo {
                public string name;
                public Sprite[] sp;
            }
            public SpriteInfo[] modSpriteList;
            public Dictionary<string, Sprite[]> modSprites;

            [System.Serializable]
            public struct AudioInfo {
                public string name;
                public AudioClip clip;
            }
            public AudioInfo[] audioList;
            public Dictionary<string, AudioClip> audioClips;

            public GameObject modalSprite;
            public GameObject modalImage;

            [System.Serializable]
            public class PlayerPrefabs {
                public string name;
                public GameObject prefab;
            }
            public PlayerPrefabs[] playerList;
            public Dictionary<string, GameObject> players;

            public GameObject dataEditorPanel = null;
            public GameObject startPanel = null;
            public GameObject mainMenu = null;
            public GameObject tipBar = null;
            public GameObject topChat = null;
            public GameObject bottomChat = null;
            public GameObject alertDialog = null;
            public GameObject infoDialog = null;
            public GameObject choiceDialog = null;
            public GameObject battleDialog = null;
            public GameObject saveLoadDialog = null;
            public GameObject settingDialog = null;
            public GameObject modalSelector = null;

            private Dictionary<UIType, GameObject> ui;
            private Dictionary<UIType, GameObject> showedUI = new Dictionary<UIType, GameObject>();
        }
    }

}
