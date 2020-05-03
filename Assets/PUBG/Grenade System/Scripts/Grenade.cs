using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ExpandingTrigger), typeof(Rigidbody))]
public class Grenade : MonoBehaviour
{
    private ExpandingTrigger trigger;
    private Rigidbody rb; 

    public float cookTime = 5.0f;

    private bool cooking;

    private void Awake()
    {
        trigger = GetComponent<ExpandingTrigger>();
        rb = GetComponent<Rigidbody>();
    }

    public void Throw(Parabola.PathData data)
    {
        rb.isKinematic = false;
        transform.parent = null;
        rb.velocity = data.initialVelocity;

        if (!cooking)
            StartCook();
    }

    public void StartCook()
    {
        StartCoroutine(Explode());
    }

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(cookTime);
        StartExpand();
    }

    public void StartExpand()
    {
        StartCoroutine(trigger.Expand());
    }
}
