using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;


    ///<summary> Wrapper to use Unity build-in Pool classes </summary>
    public static class ObjectPoolManager
    {
        ///<summary> Main pools </summary>
        public static Dictionary<int, ObjectPool<GameObject>> ObjectPools = new Dictionary<int, ObjectPool<GameObject>>();
        ///<summary> Root for pool objects </summary>
        private static GameObject PoolRoot;

        ///<summary> Create pool for prefab </summary>
        public static ObjectPool<GameObject> AddToPool(this GameObject prefab, int capacity)
        {
            if (PoolRoot == null)
            {
                PoolRoot = new GameObject("[PoolRoot]");
            }

            var hashCode = prefab.GetHashCode();
            if (ObjectPools.ContainsKey(hashCode))
            {
                return ObjectPools[hashCode];
            }

            ObjectPool<GameObject> newPool = new ObjectPool<GameObject>(() =>
                {
                    GameObject obj = Object.Instantiate(prefab, PoolRoot.transform, true);
                    return obj;
                },
                (o =>
                {
                    if (o != null)
                    {
                        o.SetActive(true);
                    }
                }),
                (o =>
                {
                    if (o != null)
                    {
                        o.SetActive(false);
                    }
                }),
                o =>
                {
                    if (o != null)
                    {
                        Object.Destroy(o);
                    }
                }, true, capacity);
            ObjectPools.Add(hashCode, newPool);
            return newPool;
        }

        public static ObjectPool<GameObject> GetManagedPool(this GameObject gameObject)
        {
            var hashCode = gameObject.GetHashCode();
            if (ObjectPools.ContainsKey(hashCode))
            {
                return ObjectPools[hashCode];
            }
            return null;
        }

        public static void CleanPoolAsync()
        {
            foreach (ObjectPool<GameObject> objectPool in ObjectPools.Values)
            {
                objectPool.Dispose();
            }
            
        }

        public static GameObject GetFromPool(this GameObject gameObject, bool createIfNotExistPool = true,
            int capacity = 10)
        {
            var hashCode = gameObject.GetHashCode();
            if (ObjectPools.ContainsKey(hashCode))
            {
                var instance = ObjectPools[hashCode].Get();
                ObjectPoolHashCode poolHashCode = instance.GetComponent<ObjectPoolHashCode>();
                if (poolHashCode == null)
                {
                    poolHashCode = instance.AddComponent<ObjectPoolHashCode>();
                }

                poolHashCode.PoolHashCode = hashCode;

                return instance;
            }

            if (createIfNotExistPool)
            {
                AddToPool(gameObject, capacity);
                return GetFromPool(gameObject);
            }

            // FDebug.LogError($"Pool for prefab [{gameObject.name}] is not available", gameObject);
            return null;
        }
        

        public static void ReturnToPool(this GameObject instance)
        {
            ObjectPoolHashCode poolHashCode = instance.GetComponent<ObjectPoolHashCode>();
            if (poolHashCode != null)
            {
                int hashCode = poolHashCode.PoolHashCode;

                if (ObjectPools.ContainsKey(hashCode))
                {
                    ObjectPools[hashCode].Release(instance);
                    instance.transform.SetParent(PoolRoot.transform);
                }
                else
                {
                    // FDebug.LogError($"This instance [{instance.name}] is not available to manager via pool: ObjectPools doesn't have this hashcode", instance);
                }
            }
            else
            {
                // FDebug.LogError($"This instance [{instance.name}] is not available to manager via pool", instance);
            }
        }

        public static GameObject GetPoolRoot()
        {
            return PoolRoot;
        }
    }
