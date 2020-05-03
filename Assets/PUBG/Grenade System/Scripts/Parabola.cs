using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parabola : MonoBehaviour
{
    public struct PathData
    {
        public readonly Vector3 initialVelocity;
        public readonly float timeToTarget;

        public PathData(Vector3 initialVelocity, float timeToTarget)
        {
            this.initialVelocity = initialVelocity;
            this.timeToTarget = timeToTarget;
        }
    }

    public static PathData CalculateParabola(Vector3 start, Vector3 end, float height, float gravity)
    {
        // float displacementY = ? - start.y
        float displacementY = end.y - start.y;
        // float displacementXZ = new Vector3(? - start.x, 0, ? - start.z);  
        Vector3 displacementXZ = new Vector3(end.x - start.x, 0, end.z - start.z);
        // float time = Mathf.Sqrt (-2 * ? / gravity) + Mathf.Sqrt(2 * (displacementY - ?) / gravity);
        float time = Mathf.Sqrt(-2 * height / gravity) + Mathf.Sqrt(2 * (displacementY - height) / gravity);
        // Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * ?);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * height);
        Vector3 velocityXZ = displacementXZ / time;
        return new PathData(velocityXZ + velocityY * -Mathf.Sign(gravity), time);
    }
}
