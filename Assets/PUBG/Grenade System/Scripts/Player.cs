using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamage, IFlashed
{
    public Camera mainCamera; 
    public float health;
    public Material standardMat;
    public Material flashedMat; 

    public void Damage(float value)
    {
        Debug.Log("Took " + value, transform);
        health -= value;
        if (health <= 0)
            Death();
    }
    public void Flashed(float value, Vector3 position)
    {
        Vector2 screenPoint = mainCamera.WorldToScreenPoint(position);
        Rect rect = new Rect(0, 0, Screen.width, Screen.height);
        if (rect.Contains(screenPoint))
            StartCoroutine(Flash(value));
    }

    IEnumerator Flash(float duration)
    {
        GetComponent<MeshRenderer>().sharedMaterial = flashedMat;
        yield return new WaitForSeconds(duration);
        GetComponent<MeshRenderer>().sharedMaterial = standardMat;
    }

    public void Death()
    {
        gameObject.SetActive(false);
    }

}
