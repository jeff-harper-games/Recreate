using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public Room currentRoom;
    public Vector2 mapImageSize = new Vector2(50, 25);
    public RectTransform mapArea;
    public RectTransform mapContainer; 

    public void SetCurrentRoom(Room room)
    {
        if(currentRoom)
            currentRoom.mapImage.Exit();
        
        currentRoom = room;
        currentRoom.Enter();
    }

    public void CheckBoundary(Vector2 position)
    {
        Vector2 offset = mapArea.sizeDelta - mapContainer.sizeDelta;
        offset = new Vector2(Mathf.Abs(offset.x), offset.y) / 2;
        Debug.Log("offset:" + offset);
        float xOffset = (mapContainer.sizeDelta.x / 2) - offset.x + mapContainer.anchoredPosition.x;
        float yOffset = (mapContainer.sizeDelta.y - mapArea.sizeDelta.y) / 2;
        yOffset = (mapContainer.sizeDelta.y / 2) - yOffset - mapContainer.anchoredPosition.y;

        Vector2 max = position + (mapImageSize / 2);
        Debug.Log(yOffset + " / " + max.y);
        //Debug.Log("max:" + max);
        Vector2 min = position - (mapImageSize / 2);

        if (max.x + xOffset > mapArea.offsetMax.x)
        {
            float x = max.x + xOffset - mapArea.offsetMax.x;
            mapContainer.anchoredPosition -= new Vector2(x, 0);
        }

        if (max.y > yOffset)
        {
            mapContainer.anchoredPosition -= new Vector2(0, max.y - yOffset);
        }

        if (min.x + xOffset < mapArea.offsetMin.x)
        {
            float x = min.x + xOffset;
            mapContainer.anchoredPosition -= new Vector2(x, 0);
        }

        if (min.y - yOffset < mapArea.offsetMin.y)
        {
            mapContainer.anchoredPosition += new Vector2(0, max.y - mapArea.offsetMin.y);
        }

        /*
        if ((max + centerOffset).y > mapContainer.anchoredPosition.y + centerOffset.y)
        {
            float y = (max + centerOffset).y - mapContainer.anchoredPosition.y + centerOffset.y;

            Debug.Log("" + (max + centerOffset).y);
            mapContainer.anchoredPosition -= new Vector2(0, y);
        }
        */




        /*
        if (max.x > mapArea.offsetMax.x)
            mapContainer.anchoredPosition -= new Vector2(mapImageSize.x, 0);
        if(max.y > mapArea.offsetMax.y)
            mapContainer.anchoredPosition -= new Vector2(0, mapImageSize.y);

        if (min.x < mapArea.offsetMin.x)
            mapContainer.anchoredPosition += new Vector2(mapImageSize.x, 0);
        if (min.y < mapArea.offsetMin.y)
            mapContainer.anchoredPosition += new Vector2(0, mapImageSize.y);
            */
    }
}
