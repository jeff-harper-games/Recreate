using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Gears of War/Weapon")]
public class Weapon : ScriptableObject
{
    [Header("Weapon Info")]
    public string weaponName;
    public Sprite weaponGraphic;

    [Header("Ammo Info")]
    public int maxAmmo = 100;
    public int clipCapacity = 10;

    [Header("Reload Info")]
    public float standardReload = 3.0f;
    public float activeReload = 2.25f;
    public float perfectReload = 1.8f;
    public float failedReload = 4.1f;

    [Header("Fire Info")]
    public float fireRate = 0.25f;

    [Header("Current")]
    public int currentAmmo = 0;
    public int currentClip = 0;
    public List<bool> bulletState = new List<bool>();

    [Header("States")]
    public State state = State.READY;
    public enum State { READY, FIRING, RELOADING };

    public delegate void UpdateUI();
    public UpdateUI updateUI;

    public bool CanReload()
    {
        if (state != State.READY)
            return false;
        if (currentClip == clipCapacity)
            return false;
        if (currentAmmo == 0)
            return false;

        return true;
    }

    public void PerfectReload()
    {
        int needed = clipCapacity - currentClip;
        int amount = currentAmmo > needed ? needed : currentAmmo;
        for (int i = 0; i < amount; i++)
        {
            bulletState.Add(true);
        }
        currentClip += amount;
        currentAmmo -= amount;

        state = State.READY;

        if (updateUI != null)
            updateUI.Invoke();
    }

    public void StandardReload()
    {
        int needed = clipCapacity - currentClip;
        int amount = currentAmmo > needed ? needed : currentAmmo;
        for (int i = 0; i < amount; i++)
        {
            bulletState.Add(false);
        }
        currentClip += amount;
        currentAmmo -= amount;

        state = State.READY;

        if (updateUI != null)
            updateUI.Invoke();
    }

    public bool CanFire()
    {
        if (state != State.READY)
            return false;
        if (currentClip == 0)
            return false;

        return true; 
    }

    public bool Fire()
    {
        bool perfect = bulletState[0];
        bulletState.RemoveAt(0);
        currentClip--;

        if (updateUI != null)
            updateUI.Invoke();

        return perfect;
    }
}
