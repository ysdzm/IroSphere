using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class DisableEditOnPlayAttribute : PropertyAttribute
{
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(DisableEditOnPlayAttribute))]

public class DisableEditOnPlayDrawer : PropertyDrawer
{
	//ゲーム実行中グレーアウト
	public override void OnGUI(Rect aPosition, SerializedProperty aProperty, GUIContent aLabel)
	{
		EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
		EditorGUI.PropertyField(aPosition, aProperty, aLabel, true);
		EditorGUI.EndDisabledGroup();
	}
}
#endif