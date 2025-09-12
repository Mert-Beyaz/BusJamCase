using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding : MonoBehaviour
{
    public Tilemap tilemap;
    private TileAndMovementCost[] _tiles;
    Vector3Int[] directions = new Vector3Int[4] { Vector3Int.left, Vector3Int.right, Vector3Int.up, Vector3Int.down };

    private void OnEnable()
    {
        _tiles = TileController.Instance.GetTiles();
    }

    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int target)
    {
        Dictionary<Vector3Int, Node> openSet = new Dictionary<Vector3Int, Node>();
        HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();

        Node startNode = new Node(start, null, 0f, Heuristic(start, target));
        openSet[start] = startNode;

        while (openSet.Count > 0)
        {
            Node current = null;
            foreach (var node in openSet.Values)
            {
                if (current == null || node.f < current.f)
                    current = node;
            }

            if (current.position == target)
                return RetracePath(current);

            openSet.Remove(current.position);
            closedSet.Add(current.position);

            foreach (Vector3Int dir in directions)
            {
                Vector3Int neighborPos = current.position + dir;
                if (closedSet.Contains(neighborPos))
                    continue;

                TileAndMovementCost? tmc = GetTileData(neighborPos);
                if (tmc == null || !tmc.Value.movable || tmc.Value.color != ColorEnums.None)
                    continue;

                float tentativeG = current.g + tmc.Value.movementCost;

                if (!openSet.TryGetValue(neighborPos, out Node neighborNode))
                {
                    neighborNode = new Node(neighborPos, current, tentativeG, Heuristic(neighborPos, target));
                    openSet[neighborPos] = neighborNode;
                }
                else if (tentativeG < neighborNode.g)
                {
                    neighborNode.g = tentativeG;
                    neighborNode.f = tentativeG + neighborNode.h;
                    neighborNode.parent = current;
                }
            }
        }

        return new List<Vector3Int>();
    }

    private List<Vector3Int> RetracePath(Node endNode)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Node current = endNode;
        while (current != null)
        {
            path.Add(current.position);
            current = current.parent;
        }
        path.Reverse();
        return path;
    }

    private float Heuristic(Vector3Int a, Vector3Int b)
    {
        return (a - b).sqrMagnitude;
    }

    private TileAndMovementCost? GetTileData(Vector3Int pos)
    {
        TileBase tileBase = tilemap.GetTile(pos);
        foreach (var t in _tiles)
        {
            if (t.tile == tileBase)
                return t;
        }
        return null;
    }

    private class Node
    {
        public Vector3Int position;
        public Node parent;
        public float g;
        public float h;
        public float f;

        public Node(Vector3Int pos, Node parent, float g, float h)
        {
            this.position = pos;
            this.parent = parent;
            this.g = g;
            this.h = h;
            this.f = g + h;
        }
    }
}
