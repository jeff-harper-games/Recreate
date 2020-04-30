using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cinemachine;

public class RoomGenerator : MonoBehaviour
{
    private RoomManager manager;
    private List<MapImage> mapImages = new List<MapImage>();
    private List<Room> spawnedRooms = new List<Room>();
    private List<Room> deadends = new List<Room>();
    private Coroutine generating; 

    public CinemachineVirtualCamera worldVirtCam;
    public GameObject roomVirtCam;
    public GameObject player; 
    public Room roomPrefab;
    public int numOfRooms = 10;
    public MapImage mapImagePrefab;
    public RectTransform mapContainer;
    public CanvasGroup minimapFade;
    public float cameraScaleBuffer = 25.0f;
    public Icons icons;
    public List<Vector2Int> grid { get; set; }

    [System.Serializable]
    public class Icons
    {
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
    }

    private void Awake()
    {
        manager = FindObjectOfType<RoomManager>();
    }

    private void Start()
    {
        //Generate();
        generating = StartCoroutine(Generate());
    }

    private IEnumerator Generate()
    {
        player.SetActive(false);
        roomVirtCam.SetActive(false);
        minimapFade.alpha = 0.0f;
        if (manager.canMove)
        {
            manager.canMove = false; 
            yield return new WaitForSeconds(2.0f);
        }
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

        spawnedRooms = new List<Room>();
        grid = new List<Vector2Int>();
        mapImages = new List<MapImage>(); 
        manager.ResetPlayer();

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

            float xPos = roomPrefab.transform.localScale.x * gridPos.x;
            float zPos = roomPrefab.transform.localScale.z * gridPos.y;
            Room newRoom = Instantiate(roomPrefab, new Vector3(xPos, 0, zPos), Quaternion.identity);
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

        float xCamScale = ((Mathf.Abs(xMax) + Mathf.Abs(xMin)) * roomPrefab.transform.localScale.x) + roomPrefab.transform.localScale.x;
        float yCamScale = ((Mathf.Abs(yMax) + Mathf.Abs(yMin)) * roomPrefab.transform.localScale.z) + roomPrefab.transform.localScale.z;

        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = xCamScale / yCamScale;

        if (screenRatio >= targetRatio)
        {
            worldVirtCam.m_Lens.OrthographicSize = yCamScale / 2;
            worldVirtCam.m_Lens.OrthographicSize += cameraScaleBuffer;
        }
        else
        {
            float difference = targetRatio / screenRatio;
            worldVirtCam.m_Lens.OrthographicSize = yCamScale / 2 * difference;
            worldVirtCam.m_Lens.OrthographicSize += cameraScaleBuffer;
        }

        Vector2 midpoint = (new Vector2(xMax * roomPrefab.transform.localScale.x, yMax * roomPrefab.transform.localScale.z) 
            + new Vector2(xMin * roomPrefab.transform.localScale.x, yMin * roomPrefab.transform.localScale.z)) / 2;
        worldVirtCam.transform.position = new Vector3(midpoint.x, 50, midpoint.y);

        Vector2 mapImageSize = mapImagePrefab.GetComponent<RectTransform>().sizeDelta;
        Vector2 mapContainerSize = new Vector2((Mathf.Abs(xMax) + Mathf.Abs(xMin)) * mapImageSize.x, (Mathf.Abs(yMax) + Mathf.Abs(yMin)) * mapImageSize.y) + mapImageSize;
        mapContainer.sizeDelta = mapContainerSize;

        float mapOffsetX = (xMax * mapImageSize.x) + (mapImageSize.x / 2) + (xMin * mapImageSize.x) - (mapImageSize.x / 2);
        float mapOffsetY = (yMax * mapImageSize.y) + (mapImageSize.y / 2) + (yMin * mapImageSize.y) - (mapImageSize.y / 2);
        Vector2 mapOffset = new Vector2(mapOffsetX, mapOffsetY) / 2;

        for (int i = 0; i < spawnedRooms.Count; i++)
        {
            MapImage mapImage = Instantiate(mapImagePrefab, mapContainer);
            spawnedRooms[i].SetMapImage(mapImage);
            mapImages.Add(mapImage);
            mapImage.Hide();
            if (i != 0)
                mapImage.Setup(manager, spawnedRooms[i].gridPos, mapOffset, null, false, false, false);
            else
            {
                mapImage.Setup(manager, spawnedRooms[i].gridPos, mapOffset, icons.rootIcon, true, true, true);
                mapImage.SetIcon(icons.rootIcon, icons.rootColor);
            }

            if (spawnedRooms[i].connections.Count == 1)
                deadends.Add(spawnedRooms[i]);
        }

        deadends = deadends.OrderByDescending(ctx => ctx.step).ToList();

        if (deadends.Count > 0)
        {
            deadends[0].roomType = RoomType.Boss;
            deadends[0].mapImage.SetIcon(icons.bossIcon, icons.bossColor);
        }


        int chance = Random.Range(0, 100);
        if (chance > 33)
        {
            if (deadends.Count > 1)
            {
                deadends[1].roomType = RoomType.Miniboss;
                deadends[1].mapImage.SetIcon(icons.minibossIcon, icons.minibossColor);
            }
            if (deadends.Count > 2)
            {
                deadends[2].roomType = RoomType.Treasure;
                deadends[2].mapImage.SetIcon(icons.treasureIcon, icons.treasureColor);
            }
        }
        else
        {
            if (deadends.Count > 1)
            {
                deadends[1].roomType = RoomType.Treasure;
                deadends[1].mapImage.SetIcon(icons.treasureIcon, icons.treasureColor);
            }
        }

        if (deadends.Count > 3)
        {
            deadends[3].roomType = RoomType.Store;
            deadends[3].mapImage.SetIcon(icons.storeIcon, icons.storeColor);
        }

        manager.SetCurrentRoom(root, false);
        //root.Enter();

        generating = null; 
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Generate();
            if (generating != null)
                StopCoroutine(generating);
            generating = StartCoroutine(Generate());
        }
    }
}
