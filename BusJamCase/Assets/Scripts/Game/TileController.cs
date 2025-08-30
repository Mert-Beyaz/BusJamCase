using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileController : MonoBehaviour
{
    public static TileController Instance;

    [SerializeField] private TileAndMovementCost[] tiles;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public TileAndMovementCost[] GetTiles()
    {
        return tiles;
    }

    public Tile GetLastTile()
    {
        return tiles[2].tile;
    }
}

[System.Serializable]
public struct TileAndMovementCost
{
    public Tile tile;
    public ColorEnums color;
    public bool movable;
    public float movementCost;
}
