using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class DeleteData : EditorWindow
{
    [MenuItem("Data/Delete Save")]
    public static void DeleteSaveFunction()
    {
        PlayerPrefs.DeleteAll();
    }
}
#endif

