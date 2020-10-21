using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Reload : MonoBehaviour
{
    [SerializeField]
    private Image slider;
    [SerializeField]
    private GameObject reloadBar;
    [SerializeField]
    private AnimationCurve curve;
    [SerializeField]
    private Vector2 perfectRange = new Vector2(100, 115);
    [SerializeField]
    private Vector2 activeRange = new Vector2(116, 155);

    private Weapon weapon;
    private Coroutine reload;

    public void BeginReload(Weapon weapon)
    {
        slider.color = Color.white;
        this.weapon = weapon;
        reloadBar.SetActive(true);
        reload = StartCoroutine(Reloading());
    }

    public void ManualReload()
    {
        float value = slider.rectTransform.anchoredPosition.x;
        if (value >= perfectRange.x && value <= perfectRange.y)
            PerfectReload(value);
        else if (value >= activeRange.x && value <= activeRange.y)
            ActiveReload(value);
        else
            FailedReload(value);

        if (reload != null)
            StopCoroutine(reload);
            
    }

    private IEnumerator Reloading()
    {
        yield return new WaitForEndOfFrame();
        weapon.state = Weapon.State.RELOADING;
        for (float t = 0; t < 1; t += Time.deltaTime / weapon.standardReload)
        {
            float value = Mathf.Lerp(0, 300, curve.Evaluate(t));
            slider.rectTransform.anchoredPosition = new Vector2(value, 0);
            yield return null;
        }
        StartCoroutine(FinishReload(0.0f, false));
    }

    private IEnumerator FinishReload(float duration, bool perfect)
    {
        yield return new WaitForSeconds(duration);
        weapon.state = Weapon.State.READY;
        slider.rectTransform.anchoredPosition = new Vector2(0, 0);
        if (perfect)
            weapon.PerfectReload();
        else
            weapon.StandardReload();
        reloadBar.SetActive(false);
    }

    private void PerfectReload(float value)
    {
        slider.color = Color.green;
        float t = Mathf.InverseLerp(0, 300, value);
        float remaining = weapon.perfectReload - (t * weapon.standardReload);
        StartCoroutine(FinishReload(remaining, true));
    }

    private void ActiveReload(float value)
    {
        float t = Mathf.InverseLerp(0, 300, value);
        float remaining = weapon.activeReload - (t * weapon.standardReload);
        StartCoroutine(FinishReload(remaining, false));
    }

    private void FailedReload(float value)
    {
        slider.color = Color.red;
        float t = Mathf.InverseLerp(0, 300, value);
        float remaining = weapon.failedReload - (t * weapon.standardReload);
        StartCoroutine(FinishReload(remaining, false));
    }
}
