using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{

    public int defaultAmount = 10;
    public GameObject[] poolList;
    public int[] poolAmount;

    public Dictionary<string, ObjectPool> objectPoolList =  new Dictionary<string, ObjectPool>();

    void Awake() {
        InitObjectPool();
    }

    void InitObjectPool()
    {
        for (int i = 0; i < poolList.Length; ++i)
        {
            ObjectPool objectPool = new ObjectPool();
            objectPool.source = poolList[i];
            objectPoolList[poolList[i].name] = objectPool;

            GameObject folder = new GameObject();
            folder.name = poolList[i].name;
            folder.transform.parent = this.transform;
            objectPool.folder = folder;

            int amount = defaultAmount;

            if (poolAmount.Length > i && poolAmount[i] != 0)
            {
                amount = poolAmount[i];
            }

            for (int j = 0; j < amount; ++j)
            {
                GameObject inst = Instantiate(objectPool.source);
                inst.SetActive(false);
                inst.transform.parent = folder.transform;
                objectPool.unusedList.Add(inst);

                //yield return new WaitForEndOfFrame();
            }
            objectPool.maxAmount = amount;
        }
    }

    public GameObject Get(string _name)
    {
        if (!objectPoolList.ContainsKey(_name))
        {
            Debug.LogError("[ObjectPoolManager] Can't Find ObjectPool : " + _name);
            return null;
        }

        ObjectPool pool = objectPoolList[_name];
        if (pool.unusedList.Count > 0)
        {
            GameObject obj = pool.unusedList[0];
            pool.unusedList.RemoveAt(0);
            obj.SetActive(true);
            return obj;
        }
        else {
            GameObject obj = Instantiate(pool.source);
            obj.transform.parent = pool.folder.transform;

            ++pool.maxAmount;
            print(_name + " / Pool Size" + pool.maxAmount);
            return obj;
        }
    }

    public GameObject Get(string _name, Vector3 _position, Quaternion _rotation)
    {
        if (!objectPoolList.ContainsKey(_name))
        {
            Debug.LogError("[ObjectPoolManager] Can't Find ObjectPool : " + _name);
            return null;
        }

        ObjectPool pool = objectPoolList[_name];
        if (pool.unusedList.Count > 0)
        {
            GameObject obj = pool.unusedList[0];
            pool.unusedList.RemoveAt(0);

            obj.transform.position = _position;
            obj.transform.rotation = _rotation;
            obj.SetActive(true);
            return obj;
        }
        else {
            GameObject obj = Instantiate(pool.source);
            obj.transform.parent = pool.folder.transform;
            obj.transform.position = _position;
            obj.transform.rotation = _rotation;


            ++pool.maxAmount;
            print(_name + " / Pool Size" + pool.maxAmount);

            return obj;
        }
    }

    public void Free(GameObject obj)
    {
        string keyName = obj.transform.parent.name;
        if (!objectPoolList.ContainsKey(keyName))
        {
            Debug.LogError("[ObjectPoolManager] Can't Find ObjectPool : " + keyName);
            return;
        }

        ObjectPool pool = objectPoolList[keyName];
        obj.SetActive(false);
        pool.unusedList.Add(obj);
    }

}
