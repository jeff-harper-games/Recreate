using System.Collections;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField]
    private Transform standardPrefab;
    [SerializeField]
    private Transform perfectPrefab;

    private Weapon weapon;

    public void BeginFire(Weapon weapon)
    {
        this.weapon = weapon;
        bool isPerfect = weapon.Fire();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Transform bullet = null;

            if (isPerfect)
                bullet = Instantiate(perfectPrefab, hit.point, Quaternion.identity);
            else
                bullet = Instantiate(standardPrefab, hit.point, Quaternion.identity);

            Destroy(bullet.gameObject, 3.0f);
        }
        StartCoroutine(Firing());
    }

    private IEnumerator Firing()
    {
        weapon.state = Weapon.State.FIRING;
        yield return new WaitForSeconds(weapon.fireRate);
        weapon.state = Weapon.State.READY;
    }
}
