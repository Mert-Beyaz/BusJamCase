using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> levelList = new();
    private int levelCount = 0;

    void Start()
    {
        StartCoroutine(LevelLoad());
    }

    private IEnumerator LevelLoad()
    {
        yield return null;
        levelList[0].SetActive(true);
        yield return null;
    }
}
