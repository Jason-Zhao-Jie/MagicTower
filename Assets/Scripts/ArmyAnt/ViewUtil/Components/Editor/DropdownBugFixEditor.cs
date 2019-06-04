using UnityEditor;

namespace ArmyAnt.ViewUtil.Components.Editor {

    [CustomEditor(typeof(DropDownBugFix))]
    public class DropDownBugFixEditor : UnityEditor.UI.DropdownEditor {
        DropDownBugFix self;
        SerializedProperty prop;
        protected override void OnEnable() {
            base.OnEnable();
            self = target as DropDownBugFix;
            prop = serializedObject.FindProperty("SortingLayer");
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(prop, new UnityEngine.GUIContent("Canvas Sorting Layer"));
            Undo.RecordObject(self, "listview change");
            EditorGUILayout.EndVertical();
        }
    }

}