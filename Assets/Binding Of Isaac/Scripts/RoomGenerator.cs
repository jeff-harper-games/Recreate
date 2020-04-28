﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class RoomGenerator : MonoBehaviour
{
    /*
     * room prefab
     * list of rooms spawned
     * list of vector2 for the grid
     */

    public RoomManager manager;
    public Room roomPrefab;
    public List<Room> spawnedRooms = new List<Room>();
    public List<Vector2Int> grid = new List<Vector2Int>();
    public List<Room> deadends = new List<Room>();

    public int numOfRooms = 10;

    public MapImage mapImagePrefab;
    public List<MapImage> mapImages = new List<MapImage>(); 
    public RectTransform mapContainer;
    public float mapImageBuffer = 10.0f;

    public Sprite rootIcon;
    public Color rootColor = Color.green;
    public Sprite bossIcon;
    public Color bossColor = Color.red; 
    public Sprite treasureIcon;
    public Color treasureColor = Color.yellow;
    public Sprite storeIcon;
    public Color storeColor = Color.blue;
    public Sprite minibossIcon;
    public Color minibossColor = Color.black;

    public Vector2 totalUIGridSize; 


    /*
    public int stageID = 1;
    public RoomAmountCalculation calculations;


    [System.Serializable]
    public class RoomAmountCalculation
    {
        public int maxRooms = 20;
        public int randMin = 0;
        public int randMax = 2;
        public int controlAmount = 5;
        public int multiplier = 10;
        public int divider = 3;
    }    
    */

    private void Start()
    {
        Generate();
    }

    private void Generate()
    {
        if (spawnedRooms.Count > 0)
        {
            // clear rooms 
            for (int i = 0; i < spawnedRooms.Count; i++)
            {
                Destroy(spawnedRooms[i].gameObject);
            }

            for (int i = 0; i < mapImages.Count; i++)
            {
                Destroy(mapImages[i].gameObject);
            }

            spawnedRooms.Clear();
            grid.Clear();
            mapImages.Clear();
        }

        // spawn root room
        Room root = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);
        root.Setup(this, manager, Vector2Int.zero);
        root.roomType = RoomType.Root;
        // add it to the rooms list
        spawnedRooms.Add(root);
        // add to grid
        grid.Add(Vector2Int.zero);

        // spawn rooms

        // while (spawned < rooms.Count)
        while (spawnedRooms.Count < numOfRooms)
        {
            // get a random room from spawned list
            int rand = Random.Range(0, spawnedRooms.Count);
            // check to make sure that is a valid room with an avaliable connection
            List<Connection> openConnections = spawnedRooms[rand].GetPossibleConnections();
            while (openConnections.Count == 0)
            {
                rand = Random.Range(0, spawnedRooms.Count);
                openConnections = spawnedRooms[rand].GetPossibleConnections();
            }
            // get a random connection of the remaining connections
            int randConn = Random.Range(0, openConnections.Count);
            // add connection to existing room
            Room room = spawnedRooms[rand];
            Connection connection = openConnections[randConn];
            room.AddConnection(connection);

            Connection newConn = Connection.None;
            Vector2Int gridPos = Vector2Int.zero;
            // create new room based on the random connection
            switch (connection)
            {
                case Connection.Left:
                    newConn = Connection.Right;
                    gridPos = room.gridPos + new Vector2Int(-1, 0);
                    break;
                case Connection.Top:
                    newConn = Connection.Bottom;
                    gridPos = room.gridPos + new Vector2Int(0, 1);
                    break;
                case Connection.Right:
                    newConn = Connection.Left;
                    gridPos = room.gridPos + new Vector2Int(1, 0);
                    break;
                case Connection.Bottom:
                    newConn = Connection.Top;
                    gridPos = room.gridPos + new Vector2Int(0, -1);
                    break;
                case Connection.None:
                    break;
                default:
                    break;
            }

            Room newRoom = Instantiate(roomPrefab, new Vector3(gridPos.x, 0, gridPos.y), Quaternion.identity);
            /*
            mapImage.rectTransform.anchoredPosition = new Vector2(gridPos.x * mapImage.rectTransform.sizeDelta.x,
                gridPos.y * mapImage.rectTransform.sizeDelta.y);
            */
            newRoom.Setup(this, manager, gridPos, newConn, room.step + 1);

            room.AddAdjacentRoom(newRoom);
            newRoom.AddAdjacentRoom(room);

            // add it to the rooms list
            spawnedRooms.Add(newRoom);
            // add to grid
            grid.Add(gridPos);
        }

        deadends = new List<Room>();


        float xMin = 0;
        float xMax = 0;
        float yMin = 0;
        float yMax = 0;

        for (int i = 0; i < grid.Count; i++)
        {
            if (grid[i].x < xMin)
                xMin = grid[i].x;
            if (grid[i].y < yMin)
                yMin = grid[i].y;
            if (grid[i].x > xMax)
                xMax = grid[i].x;
            if (grid[i].y > yMax)
                yMax = grid[i].y;
        }

        Vector2 mapImageSize = mapImagePrefab.GetComponent<RectTransform>().sizeDelta;
        Vector2 mapContainerSize = new Vector2((Mathf.Abs(xMax) + Mathf.Abs(xMin)) * mapImageSize.x, (Mathf.Abs(yMax) + Mathf.Abs(yMin)) * mapImageSize.y) + mapImageSize;
        mapContainer.sizeDelta = mapContainerSize;
        Vector2 mapOffset = ((new Vector2((xMax * mapImageSize.x) + (mapImageSize.x / 2), (yMax * mapImageSize.y) + (mapImageSize.y / 2))) 
            + new Vector2((xMin * mapImageSize.x) - (mapImageSize.x / 2), (yMin * mapImageSize.y) - (mapImageSize.y / 2))) / 2;


        for (int i = 0; i < spawnedRooms.Count; i++)
        {
            MapImage mapImage = Instantiate(mapImagePrefab, mapContainer);
            spawnedRooms[i].SetMapImage(mapImage);
            mapImages.Add(mapImage);
            //mapImage.Hide();
            mapImage.Discover();
            if (i != 0)
                mapImage.Setup(manager, spawnedRooms[i].gridPos, mapOffset, null, false, false, false);
            else
            {
                mapImage.Setup(manager, spawnedRooms[i].gridPos, mapOffset, rootIcon, true, true, true);
                mapImage.SetIcon(rootIcon, rootColor);
            }

            if (spawnedRooms[i].connections.Count == 1)
            {
                deadends.Add(spawnedRooms[i]);
            }
        }

        deadends = deadends.OrderByDescending(ctx => ctx.step).ToList();

        if (deadends.Count > 0)
        {
            deadends[0].roomType = RoomType.Boss;
            deadends[0].mapImage.SetIcon(bossIcon, bossColor);
        }


        int chance = Random.Range(0, 100);
        if (chance > 33)
        {
            if (deadends.Count > 1)
            {
                deadends[1].roomType = RoomType.Miniboss;
                deadends[1].mapImage.SetIcon(minibossIcon, minibossColor);
            }
            if (deadends.Count > 2)
            {
                deadends[2].roomType = RoomType.Treasure;
                deadends[2].mapImage.SetIcon(treasureIcon, treasureColor);
            }
        }
        else
        {
            if (deadends.Count > 1)
            {
                deadends[1].roomType = RoomType.Treasure;
                deadends[1].mapImage.SetIcon(treasureIcon, treasureColor);
            }
        }

        if (deadends.Count > 3)
        {
            deadends[3].roomType = RoomType.Store;
            deadends[3].mapImage.SetIcon(storeIcon, storeColor);
        }

        manager.SetCurrentRoom(root);
        root.Enter();


        /*
        float xMin = 0; 
        float xMax = 0; 
        float yMin = 0; 
        float yMax = 0; 

        for (int i = 0; i < mapImages.Count; i++)
        {
            Vector2 max = mapImages[i].GetMaxOffset();
            Vector2 min = mapImages[i].GetMinOffset();
            if (max.x > xMax)
                xMax = max.x;
            if (max.y > yMax)
                yMax = max.y;
            if (min.x < xMin)
                xMin = min.x;
            if (min.y < yMin)
                yMin = min.y;
        }

        Debug.Log("" + (Mathf.Abs(xMax) + Mathf.Abs(xMin)) + " x " + (Mathf.Abs(yMax) + Mathf.Abs(yMin)));

        mapContainer.sizeDelta = new Vector2(Mathf.Abs(xMax) + Mathf.Abs(xMin), Mathf.Abs(yMax) + Mathf.Abs(yMin));
        mapContainer.anchoredPosition = (new Vector2(xMax, yMax) + new Vector2(xMin, yMin)) / 2;

        for (int i = 0; i < mapImages.Count; i++)
        {
            mapImages[i].transform.SetParent(mapContainer);
        }
        */
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Generate();
    }
}
