using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolTime : MonoBehaviour {

    public float maxTime = 2f;

    void OnEnable() {
        StartCoroutine(DisableTimer());
    }

    IEnumerator DisableTimer() {
        yield return new WaitForSeconds(maxTime);
        ObjectPoolManager.Instance.Free(gameObject);
    }

}
