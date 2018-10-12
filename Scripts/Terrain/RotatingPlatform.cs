using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    public float maxRotationAngle = 90;
    public float rotationInterval = 2;
    public float rotationTime = .5f;

    float timeToNextRotation;
    float rotationZ;

    // Start is called before the first frame update
    void Start()
    {
        timeToNextRotation = rotationInterval;
        rotationZ = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(timeToNextRotation <= 0)
        {
            float angle = maxRotationAngle / rotationTime * Time.deltaTime;

            if (rotationZ + angle > maxRotationAngle)
            {
                angle = maxRotationAngle - rotationZ;
            }

            rotationZ += angle;
            transform.Rotate(Vector3.forward, angle);

            if(rotationZ == maxRotationAngle)
            {
                timeToNextRotation = rotationInterval;
                rotationZ = 0;
            }

            
        } else
        {
            timeToNextRotation -= Time.deltaTime;
        }
    }
}
