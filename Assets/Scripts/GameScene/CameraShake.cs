using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public void Shake(float xMagnitude,float yMagnitude, float zMagnitude, float duration)
    {
        StartCoroutine(ShakeRoutine( xMagnitude,  yMagnitude, zMagnitude, duration));
    }

    IEnumerator ShakeRoutine(float xMagnitude, float yMagnitude, float zMagnitude,float duration)
    {
        Vector3 startPosition = transform.position;
        float startTime = Time.time;

        while(Time.time < startTime + duration)
        {
            float randomX = Random.Range(-xMagnitude, xMagnitude);
            float randomY = Random.Range(-yMagnitude, yMagnitude);
            float randomZ = Random.Range(-zMagnitude, zMagnitude);

            transform.position = new Vector3(transform.position.x + randomX, transform.position.y+randomY, transform.position.z+randomZ);
            yield return new WaitForSeconds(0.05f);
        }

        transform.position = startPosition;
    }
}
