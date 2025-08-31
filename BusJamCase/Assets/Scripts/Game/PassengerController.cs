using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PassengerController : MonoBehaviour
{
    [SerializeField] private List<PassengerFeatures> passengersFeatures = new();

    [Header("Tilemap")]
    [SerializeField] private Tilemap tilemap;

    private Vector3 _addAreaPos = new(0.5f, 0f, 0.4f);
    private TileAndMovementCost[] tiles;

    private void OnEnable()
    {
        tiles = TileController.Instance.GetTiles();
        PutPassenger();
    }

    private void PutPassenger()
    {
        foreach (var item in passengersFeatures)
        {
            var obj = PoolManager.Instance.GetObject(PoolType.Passenger);
            var passanger = obj.GetComponent<Passenger>();
            passanger.SetFeatures(item.Color);
            LevelStarter.Instance.Passengers.Add(passanger);

            obj.transform.position = tilemap.CellToWorld(item.PassengerPos) + _addAreaPos;

            foreach (TileAndMovementCost tile in tiles)
            {
                if (tile.color == item.Color)
                {
                    SetTile(item.PassengerPos, tile.tile);
                }
            }
        }
        EventBroker.Publish(Events.SET_CLICKABLE_PASSENGER);
    }

    private void SetTile(Vector3Int pos, TileBase tile = null)
    {
        if (tile == null)
        {
            tilemap.SetTile(pos, tiles[0].tile);
            return;

        }
        tilemap.SetTile(pos, tile);
    }

}

[Serializable]
public class PassengerFeatures
{
    public Vector3Int PassengerPos;
    public ColorEnums Color;
}

