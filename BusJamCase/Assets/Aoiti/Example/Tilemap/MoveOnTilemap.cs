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
    private List<Vector3Int> _pathForOutline;
    [Range(0.001f,1f)]
    public float stepTime;

    private Tile _lastTile;
    [SerializeField] private Transform lastPoint;

    private int _clickCounter = 0;

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

    void OnEnable()
    {
        Subscribe();
        tiles = TileController.Instance.GetTiles();
        _lastTile = TileController.Instance.GetLastTile();
        pathfinder = new Pathfinder<Vector3Int>(DistanceFunc, connectionsAndCosts);
    }
    private void Subscribe()
    {
        EventBroker.Subscribe<int>(Events.RESET_CLICK_COUNTER, ResetClickCounter);
        EventBroker.Subscribe(Events.SET_CLICKABLE_PASSENGER, SetClickablePassenger);
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0) && LevelStarter.Instance.CanIClick && LevelStarter.Instance.CanITakeAPassenger())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("Passanger"))
                {
                    _clickCounter++;
                    var currentCellPos = tilemap.WorldToCell(hit.transform.position);
                    var target = tilemap.WorldToCell(lastPoint.position);
                    target.z = 0;
                    pathfinder.GenerateAstarPath(currentCellPos, target, out path);
                    if (path.Count > 0)
                    {
                        LevelStarter.Instance.ReadyPassenger++;
                        hit.collider.enabled = false;
                        tilemap.SetTile(currentCellPos, tiles[0].tile);
                        SetClickablePassenger();
                        StartCoroutine(Move(hit.transform));
                    }
                }
            }
        }
    }

    IEnumerator Move(Transform passangerTransform)
    {
        var passenger = passangerTransform.GetComponent<Passenger>();
        passenger.SetWalkAnim(true);
        passenger.SetOutline(false);
        passenger.DidMove = true;
        while (path.Count > 0)
        {
            if (tilemap.GetTile(path[0]) == _lastTile)
            {
                EventBroker.Publish(Events.SET_WAITING_AREA, passenger);
                yield break;
            }
            passangerTransform.DOMove(tilemap.CellToWorld(path[0]) + _addAreaPos, stepTime).SetEase(Ease.Linear);
            passangerTransform.DOLookAt(tilemap.CellToWorld(path[0]) + _addAreaPos, 0.1f);
            path.RemoveAt(0);
            yield return new WaitForSeconds(stepTime);
        }
    }

    private void ResetClickCounter(int _fullAreaAmount)
    {
        _clickCounter = _fullAreaAmount;
    }

    private void SetClickablePassenger()
    {
        foreach (var passenger in LevelStarter.Instance.Passengers)
        {
            if (!passenger.DidMove)
            {
                var currentCellPos = tilemap.WorldToCell(passenger.transform.position);
                var target = tilemap.WorldToCell(lastPoint.position);
                target.z = 0;
                pathfinder.GenerateAstarPath(currentCellPos, target, out _pathForOutline);
                passenger.SetOutline(_pathForOutline.Count > 0);
            }
        }
    }

    private void UnSubscribe()
    {
        EventBroker.UnSubscribe<int>(Events.RESET_CLICK_COUNTER, ResetClickCounter);
        EventBroker.Subscribe(Events.SET_CLICKABLE_PASSENGER, SetClickablePassenger);
    }

    private void OnDestroy()
    {
        UnSubscribe();
    }
}

