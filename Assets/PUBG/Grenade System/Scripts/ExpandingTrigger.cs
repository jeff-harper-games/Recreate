using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ExpandingTrigger : MonoBehaviour
{
    private SphereCollider sphereCollider; 

    public float expandRate = 0.5f;
    public float range = 10.0f;
    public float value = 10;
    public AnimationCurve falloff;

    public enum GrenadeType { Frag, Flash, Smoke };
    public GrenadeType grenadeType = GrenadeType.Frag;

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }

    public IEnumerator Expand()
    {
        for (float t = 0; t < 1.0f; t += Time.deltaTime / expandRate)
        {
            float radius = Mathf.Lerp(0, range, t);
            sphereCollider.radius = radius;
            yield return null;
        }
        transform.localScale = new Vector3(range, range, range);

        gameObject.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (grenadeType == GrenadeType.Frag)
        {
            IDamage damage = other.GetComponent<IDamage>();
            if (damage != null)
            {
                RaycastHit hit;
                var heading = other.transform.position - transform.position;
                var dist = heading.magnitude;
                var direction = heading / dist;
                Ray ray = new Ray(transform.position, direction);
                if (Physics.Raycast(ray, out hit, dist))
                {
                    if (hit.collider == other)
                    {
                        float damageScale = falloff.Evaluate((1 - (hit.distance / range))) * value;
                        damage.Damage(damageScale);
                    }
                }
            }
        }
        else if (grenadeType == GrenadeType.Flash)
        {
            IFlashed flashed = other.GetComponent<IFlashed>();
            if (flashed != null)
            {
                RaycastHit hit;
                var heading = other.transform.position - transform.position;
                var dist = heading.magnitude;
                var direction = heading / dist;
                Ray ray = new Ray(transform.position, direction);
                if (Physics.Raycast(ray, out hit, dist))
                {
                    if (hit.collider == other)
                    {
                        float hitScale = falloff.Evaluate((1 - (hit.distance / range))) * value;
                        flashed.Flashed(hitScale, transform.position);
                    }
                }
            }
        }
    }
}
