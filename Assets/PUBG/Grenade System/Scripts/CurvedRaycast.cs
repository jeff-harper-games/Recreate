using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedRaycast : MonoBehaviour
{
    public float power = 10.0f;
    public float gravity = -9.8f;
    public Transform aimStart;
    public Transform fpsCamera;
    public Transform endPoint;
    public int iterations = 10;
    public LineRenderer line;

    public List<Vector3> positions = new List<Vector3>();
    //public Rigidbody rb;

    public Grenade grenadePrefab;

    private Grenade currentGrenade;
    private bool endPointHit = false; 

    // Update is called once per frame
    void Update()
    {
        CalculatePath(iterations, aimStart.position, new Ray(aimStart.position, aimStart.forward), power);

        float height = 0;
        for (int i = 0; i < positions.Count; i++)
        {
            if (i == 0)
                height = positions[i].y;

            if (height < positions[i].y)
                height = positions[i].y;
        }
        Parabola.PathData data = Parabola.CalculateParabola(aimStart.position, endPoint.position, height, gravity);
        
        //DrawPath(data);

        if (Input.GetMouseButtonDown(0))
        {
            currentGrenade = Instantiate(grenadePrefab, aimStart.position, aimStart.rotation, aimStart);
        }

        if(Input.GetMouseButtonUp(0))
        {
            currentGrenade.Throw(data);
        }
    }

    private void CalculatePath(int iterations, Vector3 startPos, Ray ray, float velocity)
    {
        RaycastHit hit;
        Vector3 pos = startPos;
        float slicedGravity = gravity / iterations / velocity;
        positions = new List<Vector3>();
        positions.Add(startPos);

        for (int i = 0; i < iterations; i++)
        {
            if (Physics.Raycast(pos, ray.direction * velocity, out hit, velocity))
            {
                Debug.DrawRay(pos, ray.direction * velocity, Color.green);
                pos += ray.direction * velocity;
                ray = new Ray(ray.origin, ray.direction + new Vector3(0, slicedGravity, 0));
                endPoint.position = hit.point;
                endPointHit = true; 
                positions.Add(hit.point);
                for (int j = 0; j < iterations; j++)
                {
                    Debug.DrawRay(pos, ray.direction * velocity, Color.yellow);
                    pos += ray.direction * velocity;
                    ray = new Ray(ray.origin, ray.direction + new Vector3(0, slicedGravity, 0));
                }
            }
            Debug.DrawRay(pos, ray.direction * velocity, Color.magenta);
            pos += ray.direction * velocity;
            positions.Add(pos);
            ray = new Ray(ray.origin, ray.direction + new Vector3(0, slicedGravity, 0));
        }
    }

    public void DrawPath(Parabola.PathData data)
    {
        Vector3 start = aimStart.position;
        line.transform.position = start;
        Vector3 previousDrawPoint = start;
        line.positionCount = iterations + 1;
        for (int i = 0; i <= iterations; i++)
        {
            float simulationTime = i / (float)iterations * data.timeToTarget;
            Vector3 displacement = data.initialVelocity * simulationTime + Vector3.up * gravity * simulationTime * simulationTime / 2.0f;
            Vector3 drawPoint = start + displacement;
            line.SetPosition(i, displacement);
            previousDrawPoint = drawPoint;
        }
    }
}
