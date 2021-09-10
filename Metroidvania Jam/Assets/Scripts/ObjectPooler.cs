using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectPooler : MonoBehaviour
{
    GameObject[] poolHolders;

    [SerializeField] bool recycle = true;
    [SerializeField] List<Projectile> projectilePrefabs;
    public Dictionary<int, Queue<Projectile>> projectilPoolDictionary;

    public static ObjectPooler Instance;

    void Awake()
    {
        if (!Instance)
            Instance = this;
        CreatePools();
    }   

    void CreatePools()
    {
        poolHolders = new GameObject[1];

        projectilPoolDictionary = new Dictionary<int, Queue<Projectile>>();
        poolHolders[0] = new GameObject("Projectile Pool");
        foreach (Projectile prefab in projectilePrefabs)
        {
            Queue<Projectile> enemyPool = new Queue<Projectile>();
            projectilPoolDictionary.Add(prefab.id, enemyPool);
        }

        for (int i = 0; i < poolHolders.Length; i++)
            poolHolders[i].transform.SetParent(transform);
    }

    public void RecycleProjectile(Projectile projectile)
    {
        Recycle(projectile, projectilPoolDictionary[projectile.id], 0);
    }

    void Recycle<T>(T objectToRecycle, Queue<T> pool, int poolHolderIndex) where T : MonoBehaviour
    {
        if (!recycle)
        {
            Destroy(objectToRecycle);
            return;
        }
        objectToRecycle.gameObject.SetActive(false);
        pool.Enqueue(objectToRecycle);
        objectToRecycle.transform.SetParent(poolHolders[poolHolderIndex].transform);
    }

    public Projectile GetProjectile(int projectileId, Vector3 pos, Quaternion rot,
                                    ObjectData enemyData)
    {
        return Get(projectilePrefabs[projectileId], projectilPoolDictionary, projectileId, pos, rot,
                   enemyData);
    }

    T Get<T>(T prefab, Dictionary<int, Queue<T>> poolDictionary, int id, Vector3 pos, Quaternion rot,
             ObjectData objectData) where T : MonoBehaviour, IPooledObject
    {
        if (!poolDictionary.ContainsKey(id))
        {
            Debug.LogWarning("There is no existing " + prefab.name + " pool with id " + id);
            return null;
        }
        Queue<T> pool = poolDictionary[id];
        return Get(prefab, pool, pos, rot, objectData);
    }

    T Get<T>(T prefab, Queue<T> pool, Vector3 pos, Quaternion rot,
         ObjectData objectData) where T : MonoBehaviour, IPooledObject
    {
        T objectToGet = (pool.Count == 0 || !recycle)
                      ? Instantiate(prefab, pos, rot)
                      : pool.Dequeue();
        objectToGet.transform.rotation = rot;
        objectToGet.gameObject.SetActive(true);
        objectToGet.transform.position = pos;

        objectToGet.Initialise(objectData);

        return objectToGet;
    }
}