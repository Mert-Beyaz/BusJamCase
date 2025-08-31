using UnityEditor;

#if UNITY_EDITOR
public class DeleteData : EditorWindow
{
    [MenuItem("Data/Delete Save")]
    public static void DeleteSaveFunction()
    {
       LevelManager.Instance.ResetData();
    }
}
#endif

