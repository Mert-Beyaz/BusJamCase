using DG.Tweening;
using Helpers;
using Solo.MOST_IN_ONE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MoveOnTileMap : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Transform lastPoint;
    [Range(0.001f, 1f)]
    [SerializeField] private float stepTime;

    private TileAndMovementCost[] _tiles;
    private Tile _lastTile;
    private List<Vector3Int> _pathForOutline;
    private Vector3 _addAreaPos = new(0.5f, 0f, 0.4f);
    private Pathfinding _pathfinder;

    void OnEnable()
    {
        Subscribe();
        _tiles = TileController.Instance.GetTiles();
        _lastTile = TileController.Instance.GetLastTile();
        _pathfinder = GetComponent<Pathfinding>();
    }
    private void Subscribe()
    {
        EventBroker.Subscribe(Events.SET_CLICKABLE_PASSENGER, SetClickablePassenger);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && GameManager.Instance.CanIClick && LevelStarter.Instance.CanITakeAPassenger())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("Passanger"))
                {
                    CheckPath(hit);
                }
            }
        }
    }

    private void CheckPath(RaycastHit hit)
    {
        Vector3Int start = tilemap.WorldToCell(hit.transform.position); ;
        Vector3Int target = tilemap.WorldToCell(lastPoint.position);
        target.z = 0;
        List<Vector3Int> path = _pathfinder.FindPath(start, target);

        if (path.Count > 0)
        {
            Most_HapticFeedback.Generate(Most_HapticFeedback.HapticTypes.LightImpact);
            SoundManager.Instance.Play("Pick");
            LevelStarter.Instance.ReadyPassenger++;
            hit.collider.enabled = false;
            tilemap.SetTile(start, _tiles[0].tile);
            SetClickablePassenger();
            StartCoroutine(Move(hit.transform, path));
        }
    }

    IEnumerator Move(Transform passangerTransform, List<Vector3Int> path)
    {
        var passenger = passangerTransform.GetComponent<Passenger>();
        passenger.SetWalkAnim(true);
        passenger.SetOutline(false);
        passenger.DidMove = true;
        foreach (var step in path)
        {
            if (tilemap.GetTile(step) == _lastTile)
            {
                EventBroker.Publish(Events.SET_WAITING_AREA, passenger);
                yield break;
            }
            passangerTransform.DOMove(tilemap.CellToWorld(step) + _addAreaPos, stepTime).SetEase(Ease.Linear);
            if ((tilemap.CellToWorld(step) + _addAreaPos) - passangerTransform.transform.position != Vector3.zero)
                passangerTransform.transform.DOLookAt(tilemap.CellToWorld(step) + _addAreaPos, 1f);
            yield return Helper.GetWait(stepTime);
        }
    }

    private void SetClickablePassenger()
    {
        foreach (var passenger in LevelStarter.Instance.Passengers)
        {
            if (!passenger.DidMove && tilemap != null)
            {
                var currentCellPos = tilemap.WorldToCell(passenger.transform.position);
                var target = tilemap.WorldToCell(lastPoint.position);
                target.z = 0;
                _pathForOutline = _pathfinder.FindPath(currentCellPos, target);
                passenger.SetOutline(_pathForOutline.Count > 0);
            }
        }
    }

    private void UnSubscribe()
    {
        EventBroker.Subscribe(Events.SET_CLICKABLE_PASSENGER, SetClickablePassenger);
    }

    private void OnDestroy()
    {
        UnSubscribe();
    }
}
