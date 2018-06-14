using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolParticle : MonoBehaviour
{
    void OnDisable()
    {
        ObjectPoolManager.Instance.Free(gameObject);
    }

}
