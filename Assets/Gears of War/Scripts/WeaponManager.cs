using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponManager : MonoBehaviour
{
    [Header("Key Codes")]
    [SerializeField]
    private KeyCode reloadKey = KeyCode.R;
    [SerializeField]
    private KeyCode fireKey = KeyCode.Mouse0;

    [Header("Managers")]
    [SerializeField]
    private Reload reload;
    [SerializeField]
    private Fire fire;

    [SerializeField] // for testing
    private Weapon weapon;

    [Header("UI")]
    [SerializeField]
    private Image weaponImage;
    [SerializeField]
    private TextMeshProUGUI bulletCounter;
    [SerializeField]
    private Image[] bulletGraphics;

    // Start is called before the first frame update
    void Start()
    {
        weapon.updateUI += UpdateUI;
        weapon.StandardReload();
        weapon.state = Weapon.State.READY;
    }

    // Update is called once per frame
    void Update()
    {
        if (weapon.CanReload() && Input.GetKeyDown(reloadKey))
            reload.BeginReload(weapon);
        else if (Input.GetKeyDown(reloadKey) && weapon.state == Weapon.State.RELOADING)
            reload.ManualReload();

        if (weapon.CanFire() && Input.GetKey(fireKey))
            fire.BeginFire(weapon);
        else if (weapon.CanReload() && Input.GetKey(fireKey))
            reload.BeginReload(weapon);
    }

    // for testing
    private void OnGUI()
    {
        GUI.Label(new Rect(5, 0, 500, 30), "State: " + weapon.state);
    }

    private void UpdateUI()
    {
        weaponImage.sprite = weapon.weaponGraphic;
        bulletCounter.text = "" + weapon.currentAmmo;

        for (int i = 0; i < bulletGraphics.Length; i++)
        {
            bulletGraphics[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < weapon.currentClip; i++)
        {
            bulletGraphics[i].gameObject.SetActive(true);
            if (weapon.bulletState[i])
                bulletGraphics[i].color = Color.yellow;
            else
                bulletGraphics[i].color = Color.white;
        }
    }
}
