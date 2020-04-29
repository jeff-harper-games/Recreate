using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RoomManager : MonoBehaviour
{
    private Room currentRoom; 
    //private Transform mainCamera;
    private Transform player;
    private Vector2 mapImageSize;
    private Vector3 playerStartPos;
    private Coroutine pausing; 

    public RectTransform mapArea;
    public RectTransform mapContainer;
    public RectTransform mapImagePrefab;
    public CinemachineVirtualCamera roomVirtCam;
    public float pauseDuration = 5.0f;
    public float panDuration = 1.0f;
    public CanvasGroup minimapFade;
    public float minimapAlpha = 0.75f;
    public bool canMove { get; set; }

    private void Awake()
    {
        //mainCamera = Camera.main.transform;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerStartPos = player.position;
        mapImageSize = mapImagePrefab.sizeDelta;
    }

    public void ResetPlayer()
    {
        canMove = false;
        roomVirtCam.gameObject.SetActive(false);
        player.position = playerStartPos;
        //if (pausing != null)
        //    StopCoroutine(pausing);
        //pausing = StartCoroutine(PauseToZoom());
    }

    private IEnumerator PauseToZoom()
    {
        //yield return new WaitForSeconds(pauseDuration);
        roomVirtCam.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        canMove = true;
        minimapFade.alpha = minimapAlpha;
        pausing = null;
        currentRoom.Enter();
        player.gameObject.SetActive(true);
    }

    public void SetCurrentRoom(Room room, bool moveCamera = true)
    {
        if(currentRoom)
            currentRoom.Exit();
        
        currentRoom = room;

        float xPos = currentRoom.transform.position.x;
        float zPos = currentRoom.transform.position.z;

        if (moveCamera)
            StartCoroutine(MoveCamera(new Vector3(xPos, roomVirtCam.transform.position.y, zPos)));
        else
            roomVirtCam.transform.position = new Vector3(xPos, roomVirtCam.transform.position.y, zPos);
    }

    private IEnumerator MoveCamera(Vector3 endPos)
    {
        Debug.Log("Move Camera");
        Vector3 startPos = roomVirtCam.transform.position;
        canMove = false;
        for (float t = 0; t < 1.0f; t += Time.deltaTime / panDuration)
        {
            Vector3 position = Vector3.Lerp(startPos, endPos, t);
            roomVirtCam.transform.position = position;
            yield return null;
        }
        canMove = true; 
    }

    public void CheckBoundary(Vector2 position)
    {
        if (mapContainer.sizeDelta.x < mapArea.sizeDelta.x && mapContainer.sizeDelta.y < mapArea.sizeDelta.y)
            return;

        Vector2 offset = mapArea.sizeDelta - mapContainer.sizeDelta;
        offset = new Vector2(Mathf.Abs(offset.x), offset.y) / 2;
        float xOffset = (mapContainer.sizeDelta.x / 2) - offset.x + mapContainer.anchoredPosition.x;

        float yOffset = (mapContainer.sizeDelta.y - mapArea.sizeDelta.y) / 2;
        yOffset = (mapContainer.sizeDelta.y / 2) - yOffset - mapContainer.anchoredPosition.y;

        Vector2 max = position + (mapImageSize / 2);
        Vector2 min = position - (mapImageSize / 2);

        if (max.x + xOffset > mapArea.offsetMax.x)
        {
            float x = max.x + xOffset - mapArea.offsetMax.x;
            mapContainer.anchoredPosition -= new Vector2(x, 0);
        }

        if (max.y > yOffset)
        {
            float y = max.y - yOffset;
            mapContainer.anchoredPosition -= new Vector2(0, y);
        }

        if (min.x + xOffset < mapArea.offsetMin.x)
        {
            float x = min.x + xOffset;
            mapContainer.anchoredPosition -= new Vector2(x, 0);
        }

        if (min.y - yOffset < mapArea.offsetMin.y)
        {
            float y = Mathf.Abs(min.y - yOffset) - Mathf.Abs(mapArea.offsetMin.y);
            mapContainer.anchoredPosition += new Vector2(0, y);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (pausing != null)
                StopCoroutine(pausing);
            pausing = StartCoroutine(PauseToZoom());
        }

    }
}
