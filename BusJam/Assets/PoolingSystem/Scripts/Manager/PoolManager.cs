using System.Collections.Generic;
using UnityEngine;
public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    [SerializeField] private List<PoolItemSO> poolItems;

    private readonly Dictionary<PoolType, Queue<GameObject>> _poolDic = new();
    private readonly Dictionary<PoolType, GameObject> _prefabLookup = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePools()
    {
        foreach (var item in poolItems)
        {
            var queue = new Queue<GameObject>();
            for (int i = 0; i < item.InitialSize; i++)
            {
                var obj = Instantiate(item.Prefab, transform);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }

            _poolDic[item.PoolType] = queue;
            _prefabLookup[item.PoolType] = item.Prefab;
        }
    }

    public GameObject GetObject(PoolType type)
    {
        if (!_poolDic.ContainsKey(type))
        {
            Debug.LogError($"No pool found for type: {type}");
            return null;
        }
        GameObject obj;

        if (_poolDic[type].Count > 0) obj = _poolDic[type].Dequeue();
        else obj = Instantiate(_prefabLookup[type], transform);

        if (!obj.TryGetComponent(out Poolable poolable))
            poolable = obj.AddComponent<Poolable>();
            poolable.poolType = type;

        obj.SetActive(true);
        return obj;

    }

    public void ReturnObject(GameObject obj)
    {
        var poolable = obj.GetComponent<Poolable>();
        if (poolable == null)
        {
            Destroy(obj);
            return;
        }

        PoolType type = poolable.poolType;

        if (!_poolDic.ContainsKey(type))
        {
            Destroy(obj);
            return;
        }

        if (!_poolDic[type].Contains(obj))
        {
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            _poolDic[type].Enqueue(obj);
        }
    }

}

