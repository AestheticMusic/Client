using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Transform parentTransform;

    public float shakeAmount;
    public float shakeTime;
    public float shakeLerpTime;

    float shakePercent;
    float shakeDuration;

    Vector3 standardPostion;

    void Awake()
    {
        standardPostion = parentTransform.position;
    }

    public void ShakeCam()
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);


        shakeDuration = shakeTime;
        shakeCoroutine = StartCoroutine(Shake());
    }

    Coroutine shakeCoroutine = null;

    IEnumerator Shake()
    {
        while (shakeDuration > 0f)
        {
            Vector3 amountPositionVec = Random.insideUnitSphere * shakeAmount;
            Vector3 amountRotationVec = Random.insideUnitSphere * shakeAmount;
            amountPositionVec.z = 0;
            amountRotationVec.x = amountRotationVec.y = 0;

            shakePercent = shakeAmount * shakePercent;
            shakeDuration -= Time.deltaTime;

            parentTransform.position = Vector3.Lerp(parentTransform.position, amountPositionVec, Time.deltaTime * shakeLerpTime);
            parentTransform.localRotation = Quaternion.Lerp(parentTransform.localRotation, Quaternion.Euler(amountRotationVec), Time.deltaTime * shakeLerpTime);

            yield return null;
        }

        parentTransform.position = standardPostion;
        parentTransform.localRotation = Quaternion.identity;
        shakeCoroutine = null;
    }

}