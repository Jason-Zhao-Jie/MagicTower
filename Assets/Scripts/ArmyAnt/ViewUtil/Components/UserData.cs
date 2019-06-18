using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArmyAnt.ViewUtil.Components
{
    public class UserData : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetIntegerData(string tag, int data)
        {
            if (integerStringTagData == null)
            {
                integerStringTagData = new Dictionary<string, int>();
            }
            integerStringTagData[tag] = data;
        }

        public int GetIntegerData(string tag)
        {
            if (integerStringTagData != null && integerStringTagData.ContainsKey(tag))
            {
                return integerStringTagData[tag];
            }
            return 0;
        }

        public void SetIntegerData(int tag, int data)
        {
            if (integerIntegerTagData == null)
            {
                integerIntegerTagData = new Dictionary<int, int>();
            }
            integerIntegerTagData[tag] = data;
        }

        public int GetIntegerData(int tag)
        {
            if (integerIntegerTagData != null && integerIntegerTagData.ContainsKey(tag))
            {
                return integerIntegerTagData[tag];
            }
            return 0;
        }

        public void SetIntegerData(int data)
        {
            defaultIntegerData = data;
        }

        public int GetIntegerData()
        {
            return defaultIntegerData;
        }

        public void SetStringData(string tag, string data) {
            if(stringStringTagData == null) {
                stringStringTagData = new Dictionary<string, string>();
            }
            stringStringTagData[tag] = data;
        }

        public string GetStringData(string tag) {
            if(stringStringTagData != null && stringStringTagData.ContainsKey(tag)) {
                return stringStringTagData[tag];
            }
            return null;
        }

        public void SetStringData(int tag, string data) {
            if(stringIntegerTagData == null) {
                stringIntegerTagData = new Dictionary<int, string>();
            }
            stringIntegerTagData[tag] = data;
        }

        public string GetStringData(int tag) {
            if(stringIntegerTagData != null && stringIntegerTagData.ContainsKey(tag)) {
                return stringIntegerTagData[tag];
            }
            return "";
        }

        public void SetStringData(string data) {
            defaultStringData = data;
        }

        public string GetStringData() {
            return defaultStringData;
        }

        public void SetGameObject(string tag, GameObject data) {
            if(gameObjectStringTagData == null) {
                gameObjectStringTagData = new Dictionary<string, GameObject>();
            }
            gameObjectStringTagData[tag] = data;
        }

        public GameObject GetGameObject(string tag) {
            if(gameObjectStringTagData != null && gameObjectStringTagData.ContainsKey(tag)) {
                return gameObjectStringTagData[tag];
            }
            return null;
        }

        public void SetGameObject(int tag, GameObject data) {
            if(gameObjectIntegerTagData == null) {
                gameObjectIntegerTagData = new Dictionary<int, GameObject>();
            }
            gameObjectIntegerTagData[tag] = data;
        }

        public GameObject GetGameObject(int tag) {
            if(gameObjectIntegerTagData != null && gameObjectIntegerTagData.ContainsKey(tag)) {
                return gameObjectIntegerTagData[tag];
            }
            return null;
        }

        public void SetGameObject(GameObject data) {
            defaultGameObjectData = data;
        }

        public GameObject GetGameObject() {
            return defaultGameObjectData;
        }

        private Dictionary<string, int> integerStringTagData = null;
        private Dictionary<int, int> integerIntegerTagData = null;
        private Dictionary<string, string> stringStringTagData = null;
        private Dictionary<int, string> stringIntegerTagData = null;
        private Dictionary<string, GameObject> gameObjectStringTagData = null;
        private Dictionary<int, GameObject> gameObjectIntegerTagData = null;
        private int defaultIntegerData = 0;
        private string defaultStringData = null;
        private GameObject defaultGameObjectData = null;
    }

}