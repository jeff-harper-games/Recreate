using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Connection { Left, Top, Right, Bottom, None }
public enum RoomType { Root, Boss, Standard, Treasure, Store, Miniboss }

public class Room : MonoBehaviour
{
    /*
     * int step
     * vector2 gridPos
     * gameobjects 4 doors
     * list of connections
     * ref to generator
     * room type
    */

    public RoomGenerator generator;
    public RoomManager manager;
    public int step;
    public Vector2Int gridPos;
    public List<Connection> connections = new List<Connection>();
    public List<Room> adjacentRooms = new List<Room>();
    public RoomType roomType = RoomType.Standard;
    public MapImage mapImage; 

    public GameObject leftDoor; 
    public GameObject topDoor; 
    public GameObject rightDoor; 
    public GameObject bottomDoor; 

    // pass in the generator, gridPos, connection, and step
    // add connection to list of connections
    public void Setup(RoomGenerator generator, RoomManager manager, Vector2Int gridPos, Connection connection = Connection.None, int step = 0)
    {
        this.generator = generator;
        this.manager = manager;
        this.gridPos = gridPos;
        this.step = step;
        AddConnection(connection);
    }

    public void SetMapImage(MapImage mapImage)
    {
        this.mapImage = mapImage;
    }

    public void AddConnection(Connection connection)
    {
        switch (connection)
        {
            case Connection.Left:
                connections.Add(connection);
                leftDoor.SetActive(false);
                break;
            case Connection.Top:
                connections.Add(connection);
                topDoor.SetActive(false);
                break;
            case Connection.Right:
                connections.Add(connection);
                rightDoor.SetActive(false);
                break;
            case Connection.Bottom:
                connections.Add(connection);
                bottomDoor.SetActive(false);
                break;
            case Connection.None:
                break;
            default:
                break;
        }
    }

    public List<Connection> GetPossibleConnections()
    {
        // check all existing connections
        // if a connection does not exist
        // check if there is anything blocking that space
        // if both == true, add to connections

        List<Connection> openConections = new List<Connection>();

        List<Vector2Int> grid = generator.grid;

        if (!connections.Contains(Connection.Left))
        {
            if (!grid.Contains(gridPos + new Vector2Int(-1, 1)) && !grid.Contains(gridPos + new Vector2Int(-2, 0)) && !grid.Contains(gridPos + new Vector2Int(-1, -1)))
                openConections.Add(Connection.Left);
        }

        if (!connections.Contains(Connection.Top))
        {
            if (!grid.Contains(gridPos + new Vector2Int(-1, 1)) && !grid.Contains(gridPos + new Vector2Int(0, 2)) && !grid.Contains(gridPos + new Vector2Int(1, 1)))
                openConections.Add(Connection.Top);
        }

        if (!connections.Contains(Connection.Right))
        {
            if (!grid.Contains(gridPos + new Vector2Int(1, 1)) && !grid.Contains(gridPos + new Vector2Int(2, 0)) && !grid.Contains(gridPos + new Vector2Int(1, -1)))
                openConections.Add(Connection.Right);
        }

        if (!connections.Contains(Connection.Bottom))
        {
            if (!grid.Contains(gridPos + new Vector2Int(-1, -1)) && !grid.Contains(gridPos + new Vector2Int(0, -2)) && !grid.Contains(gridPos + new Vector2Int(1, -1)))
                openConections.Add(Connection.Bottom);
        }

        return openConections;
    }

    public void AddAdjacentRoom(Room room)
    {
        adjacentRooms.Add(room);
    }

    private void OnDrawGizmos()
    {
        if (roomType == RoomType.Root)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.25f);
        }
        else if (roomType == RoomType.Boss)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.25f);
        }
        else if (roomType == RoomType.Miniboss)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, 0.25f);
        }
        else if (roomType == RoomType.Store)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(transform.position, 0.25f);
        }
        else if (roomType == RoomType.Treasure)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.25f);
        }


    }

    private void OnMouseDown()
    {
        if (manager.currentRoom == this)
            mapImage.Complete();

        manager.SetCurrentRoom(this);
    }

    public void Enter()
    {
        mapImage.Enter();

        for (int i = 0; i < adjacentRooms.Count; i++)
        {
            adjacentRooms[i].mapImage.Discover();
        }
    }

    public void Exit()
    {
        mapImage.Exit();
    }
}