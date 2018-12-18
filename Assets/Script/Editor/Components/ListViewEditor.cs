using UnityEditor;

[CustomEditor(typeof(ListView))]
public class ListViewEditor : UnityEditor.UI.ScrollRectEditor {
    ListView listview;

	protected override void OnEnable () {
        base.OnEnable();
        listview = target as ListView;
	}

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space();
        listview.DefaultElement = EditorGUILayout.ObjectField("Default element", null, typeof(UnityEngine.RectTransform), true) as UnityEngine.RectTransform;
    }
}
