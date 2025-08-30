using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Aoiti.Pathfinding;
using DG.Tweening;
using static UnityEditor.PlayerSettings;

public class MoveOnTilemap : MonoBehaviour
{
    Vector3Int[] directions=new Vector3Int[4] {Vector3Int.left,Vector3Int.right,Vector3Int.up,Vector3Int.down };

    public Tilemap tilemap;
    private TileAndMovementCost[] tiles;
    Pathfinder<Vector3Int> pathfinder;
    private Vector3 _addAreaPos = new(0.6f, 0f, 0.6f);

    public List<Vector3Int> path;
    [Range(0.001f,1f)]
    public float stepTime;

    private Tile _lastTile;
    [SerializeField] private Transform lastPoint;

    public float DistanceFunc(Vector3Int a, Vector3Int b)
    {
        return (a-b).sqrMagnitude;
    }


    public Dictionary<Vector3Int,float> connectionsAndCosts(Vector3Int a)
    {
        Dictionary<Vector3Int, float> result= new Dictionary<Vector3Int, float>();
        foreach (Vector3Int dir in directions)
        {
            foreach (TileAndMovementCost tmc in tiles)
            {
                if (tilemap.GetTile(a+dir)==tmc.tile)
                {
                    if (tmc.movable && (tmc.color == ColorEnums.None)) result.Add(a + dir, tmc.movementCost);
                }
            }
        }
        return result;
    }

    void Start()
    {
        tiles = TileController.Instance.GetTiles();
        _lastTile = TileController.Instance.GetLastTile();
        pathfinder = new Pathfinder<Vector3Int>(DistanceFunc, connectionsAndCosts);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("Passanger"))
                {
                    var currentCellPos = tilemap.WorldToCell(hit.transform.position);
                    var target = tilemap.WorldToCell(lastPoint.position);
                    target.z = 0;
                    pathfinder.GenerateAstarPath(currentCellPos, target, out path);
                    StopAllCoroutines();
                    if (path.Count > 0)
                    {
                        hit.collider.enabled = false;
                        tilemap.SetTile(currentCellPos, tiles[0].tile);
                    }
                    StartCoroutine(Move(hit.transform));
                }
            }
        }
    }

    IEnumerator Move(Transform passengerTransform)
    {
        var passenger = passengerTransform.GetComponent<Passenger>();
        while (path.Count > 0)
        {
            if (tilemap.GetTile(path[0]) == _lastTile)
            {
                EventBroker.Publish(Events.SET_WAITING_AREA, passenger);
                yield break;
            }
            passengerTransform.DOMove(tilemap.CellToWorld(path[0]) + _addAreaPos, stepTime);
            passengerTransform.DOLookAt(tilemap.CellToWorld(path[0]) + _addAreaPos, 0.1f);
            passenger.SetWalkAnim(true);
            path.RemoveAt(0);
            yield return new WaitForSeconds(stepTime);
            
        }
        

    }



    
}

