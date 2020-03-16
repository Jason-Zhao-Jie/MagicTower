using System.Collections.Generic;
using UnityEngine;
using MagicTower.Present.Manager;

namespace MagicTower.Components.DataEditor
{
    public class EventEditorPanel : MonoBehaviour
    {
        public int EventId {
            get; set;
        }

        public long[] SelectedKey {
            get; set;
        }

        public System.Action<long[]> ApplyCallback { get; set; }

        private void Awake()
        {
            foreach(var i in childEditorPrefabs)
            {
                if(i.prefab)
                {
                    prefabs.Add(i.name, i.prefab);
                }
            }
        }

        private void Start()
        {
            if (prefabs.ContainsKey((EventManager.EventName)EventId))
            {
                noSettingRoot.SetActive(false);
                var childPanel = Instantiate(prefabs[(EventManager.EventName)EventId], root);
                childPanel.transform.localScale = Vector3.one;
                childPanel.GetComponent<EventEditorPanels.Common_EventEditor>().parent = this;
            }
            else
            {
                noSettingRoot.SetActive(true);
            }
        }

        public void OnClickConfirm()
        {
            // 退出窗口
            ApplyCallback(SelectedKey);
            Destroy(gameObject);
        }

        [System.Serializable]
        public class EventEditorPrefab
        {
            public EventManager.EventName name;
            public GameObject prefab;
        }

        public EventEditorPrefab[] childEditorPrefabs;
        public RectTransform root;
        public GameObject noSettingRoot;

        private IDictionary<EventManager.EventName, GameObject> prefabs = new Dictionary<EventManager.EventName, GameObject>();
    }

}
