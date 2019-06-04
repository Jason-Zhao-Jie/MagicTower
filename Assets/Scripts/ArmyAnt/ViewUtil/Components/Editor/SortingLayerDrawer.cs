using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/*
--------------------- 
作者：鱼蛋-Felix
来源：CSDN
原文：https://blog.csdn.net/egostudio/article/details/51751149 
版权声明：本文为博主原创文章，转载请附上博文链接！
*/

namespace ArmyAnt.ViewUtil.Components.Editor {

    [CustomPropertyDrawer(typeof(Attribute.SortingLayerSettingAttribute))]
    public class SortingLayerDrawer : PropertyDrawer {
        const string NONE = "<None>";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if(property.propertyType != SerializedPropertyType.String) {
                EditorGUI.LabelField(position, "ERROR:", "Sorting Layer property may only apply to type string (name)");
                return;
            }
            position = EditorGUI.PrefixLabel(position, label);
            string value = property.stringValue;
            if(string.IsNullOrEmpty(value))
                value = NONE;
            if(GUI.Button(position, value, EditorStyles.popup)) {
                Selector(property);
            }
        }

        void Selector(SerializedProperty property) {
            string[] layers = GetSortingLayerNames();
            GenericMenu menu = new GenericMenu();
            for(int i = 0; i < layers.Length; i++) {
                var name = layers[i];
                menu.AddItem(new GUIContent(name), name == property.stringValue, HandleSelect, new Pair { name = name, property = property });
            }
            menu.ShowAsContext();
        }

        void HandleSelect(object val) {
            var pair = val as Pair;
            if(pair.name.Equals(NONE)) {
                pair.property.stringValue = "";
            } else {
                pair.property.stringValue = pair.name;
            }
            pair.property.serializedObject.ApplyModifiedProperties();
        }

        // Get the sorting layer names
        public string[] GetSortingLayerNames() {
            System.Type internalEditorUtilityType = typeof(UnityEditorInternal.InternalEditorUtility);
            System.Reflection.PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            return (string[])sortingLayersProperty.GetValue(null, new object[0]);
        }

        private class Pair {
            public string name;
            public SerializedProperty property;
        }
    }

}
