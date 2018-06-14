using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPool
{
    public GameObject source;

    public int maxAmount;

    public GameObject folder;

    public List<GameObject> unusedList = new List<GameObject>();
    
}
