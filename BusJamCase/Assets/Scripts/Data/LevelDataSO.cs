using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "LevelData/Level Item")]
public class LevelData : ScriptableObject
{
    public GameObject LevelPrefab;
    public float levelTime = 100;
}
