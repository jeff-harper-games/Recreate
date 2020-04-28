using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapImage : MonoBehaviour
{
    public GameObject container; 
    public Image completeImage;
    public Image iconImage;

    public Color incompletedColor = Color.grey;
    public Color completeColor = Color.grey;
    public Color activeColor = Color.white;

    private bool complete;
    private bool active; 
    private bool discovered;
    private RectTransform rectTransform;
    private RoomManager manager;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public Vector2 GetMaxOffset()
    {
        return rectTransform.offsetMax;
    }

    public Vector2 GetMinOffset()
    {
        return rectTransform.offsetMin;
    }

    public void Setup(RoomManager manager, Vector2Int gridPos, Vector2 offset, Sprite mapIcon = null, bool complete = false, bool active = false, bool discovered = false)
    {
        this.manager = manager;
        Vector2 position = new Vector2(gridPos.x * rectTransform.sizeDelta.x, gridPos.y * rectTransform.sizeDelta.y);
        rectTransform.anchoredPosition = position - offset;
        if (mapIcon)
            iconImage.sprite = mapIcon;
        else
            iconImage.enabled = false;
        this.complete = complete;
        if (active)
            Enter();
        if (discovered)
            Discover();
    }

    public void Display()
    {
        container.SetActive(true);
        if (complete && !active)
            completeImage.color = completeColor;
        else if (!complete && !active)
            completeImage.color = incompletedColor;
        else if (active)
            completeImage.color = activeColor;
    }

    public void Hide()
    {
        container.SetActive(false);
    }

    public void Enter()
    {
        active = true;
        completeImage.color = activeColor;
        manager.CheckBoundary(rectTransform.anchoredPosition);
    }

    public void Exit()
    {
        active = false;
        if (complete && !active)
            completeImage.color = completeColor;
        else if (!complete && !active)
            completeImage.color = incompletedColor;
    }

    public void Complete()
    {
        complete = true;
    }

    public void Discover()
    {
        discovered = true;
        Display();
    }

    public void SetIcon(Sprite sprite, Color color)
    {
        iconImage.enabled = true;
        iconImage.sprite = sprite;
        iconImage.color = color;
    }
}
