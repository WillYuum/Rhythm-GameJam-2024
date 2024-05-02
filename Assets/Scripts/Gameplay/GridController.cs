using UnityEngine;
using UnityEngine.Tilemaps;
using GameplayUtils.TileMapExtension;

[RequireComponent(typeof(RoomSpawnManager))]
public class GridController : MonoBehaviour
{
    [SerializeField] private Tilemap _activeTilemap;
    private RoomSpawnManager _roomSpawnManager;
    public RoomsData.Config CurrentRoomData { get; private set; }
    public RoomsData.Config PrevRoomData { get; private set; }

    [SerializeField] private Transform _spawnedObjectsParent;

    private RoomsData _roomsData;



    private void Awake()
    {
        _roomsData = new RoomsData();
        _roomSpawnManager = GetComponent<RoomSpawnManager>();
    }


    public bool CheckIfCanMoveToPosition(Vector2Int newCellPos)
    {
        return _activeTilemap.IsCellPosOutOfBounds(newCellPos);
    }

    public Vector2 GetWorldPosFromCellPos(Vector2Int cellPos)
    {
        return _activeTilemap.GetWorldPosFromCellPos(cellPos);
    }

    public Vector2Int GetCellPosFromWorldPos(Vector2 worldPos)
    {
        return _activeTilemap.GetCellPosFromWorldPos(worldPos);
    }


    public bool NextRoomIsEndGame()
    {
        //For now room 12 is the end game room 
        return CurrentRoomData.RoomNumber == 12;
    }


    public void BuildUpObjectsInRoom(int roomNumber)
    {
        int roomIndex = roomNumber - 1;
        if (roomIndex < 0 || roomIndex >= _roomsData.AllRooms.Length)
        {
            Debug.LogError("|GridController| Room index out of bounds");
            return;
        }

        ClearCreateObjectsOnGrid();


        var roomData = _roomsData.GetRoomData(roomIndex);

        var doorPrefab = _roomSpawnManager.DoorExitPrefab;
        var enemyPrefab = _roomSpawnManager.EnemyPrefab;
        var obstaclePrefab = _roomSpawnManager.ObstaclePrefab;


        MusicTracker musicTracker = FindObjectOfType<MusicTracker>();

        if (roomData.ExitDoors != null)
        {
            foreach (var doorData in roomData.ExitDoors)
            {
                Debug.Log($"Door at {doorData.Position}");
                doorPrefab.CreateGameObject(_activeTilemap.GetWorldPosFromCellPos(doorData.Position), Quaternion.identity, _spawnedObjectsParent);

            }
        }

        if (roomData.EnemyPositions != null)
        {
            foreach (var enemyPos in roomData.EnemyPositions)
            {
                Debug.Log($"Enemy at {enemyPos}");
                GameObject enemy = enemyPrefab.CreateGameObject(_activeTilemap.GetWorldPosFromCellPos(enemyPos), Quaternion.identity, _spawnedObjectsParent);
                enemy.GetComponent<EnemyFollowAndDance>().Prepare(musicTracker, this);
            }
        }

        if (roomData.ObstaclePositions != null)
        {
            foreach (var obstaclePos in roomData.ObstaclePositions)
            {
                Debug.Log($"Obstacle at {obstaclePos}");
                obstaclePrefab.CreateGameObject(_activeTilemap.GetWorldPosFromCellPos(obstaclePos), Quaternion.identity, _spawnedObjectsParent);
            }
        }

        PrevRoomData = CurrentRoomData;
        CurrentRoomData = roomData;
    }



    private void ClearCreateObjectsOnGrid()
    {
        if (_spawnedObjectsParent.childCount == 0)
            return;

        foreach (Transform child in _spawnedObjectsParent)
        {
            Destroy(child.gameObject);
        }
    }
}


public class RoomsData
{
    public class Config
    {
        public int RoomNumber;
        public int MusicLayer = -1;
        public ExitDoor[] ExitDoors;
        public Vector2Int[] EnemyPositions;
        public Vector2Int[] ObstaclePositions;
    }

    public class ExitDoor
    {
        public Vector2Int Position;
        public int RoomNumber;

        public ExitDoor(Vector2Int position, int roomNumber)
        {
            Position = position;
            RoomNumber = roomNumber;
        }
    }

    public Config[] AllRooms { get; private set; }

    public RoomsData()
    {
        AllRooms = new Config[]{
            // Room 1
            new () {
                RoomNumber = 1,
                MusicLayer = 1,
                ExitDoors = new ExitDoor[]{
                    new(new Vector2Int(-2, 0), 2),

                },
                EnemyPositions = null,
                ObstaclePositions = null,
            },
            // Room 2
            new()
            {
                RoomNumber = 2,
                MusicLayer = 2,
                ExitDoors = new ExitDoor[]{
                   new( new Vector2Int(-2, -2), 3)
                },
                EnemyPositions = null,
                ObstaclePositions = null,
            },
            // Room 3
            new()
            {
                RoomNumber = 3,
                MusicLayer = 3,
                ExitDoors = new ExitDoor[]{
                    new(new Vector2Int(-2, -2), 4)
                },
                EnemyPositions = new Vector2Int[]
                {
                    new Vector2Int(0, 2)
                },
                ObstaclePositions = null,
            },
            //Room 4
            new()
            {
                RoomNumber = 4,
                MusicLayer = 4,
                ExitDoors = new ExitDoor[]{
                   new( new Vector2Int(0, 2), 6),
                   new( new Vector2Int(-2, 2), 5),
                },
                EnemyPositions = null,
                ObstaclePositions = null,
            },
            //Room 5
            new()
            {
                RoomNumber = 5,
                ExitDoors = null,
                EnemyPositions = new Vector2Int[]
                {
                    new Vector2Int(-2, -2)
                },
                ObstaclePositions = null,
            },
            //Room 6
            new()
            {
                RoomNumber = 6,
                MusicLayer = 5,
                ExitDoors = new ExitDoor[]{
                   new( new Vector2Int(2, 0), 8),
                   new( new Vector2Int(0, 2), 7),
                },
                EnemyPositions = new Vector2Int[]
                {
                    new Vector2Int(-2, -2)
                },
                ObstaclePositions = null,
            },
            //Room 7
            new()
            {
                RoomNumber = 7,
                ExitDoors = new ExitDoor[]{
                   new( new Vector2Int(-2, -2), 3)
                },
                EnemyPositions = new Vector2Int[]
                {
                    new Vector2Int(-2, -2)
                },
                ObstaclePositions = null,
            },
            //Room 8
            new()
            {
                RoomNumber = 8,
                MusicLayer = 6,
                ExitDoors = new ExitDoor[]{
                   new( new Vector2Int(0, 2), 9)
                },
                EnemyPositions = new Vector2Int[]
                {
                    new Vector2Int(-2, -2)
                },
                ObstaclePositions = new Vector2Int[]
                {
                    new Vector2Int(0, 1),
                }
            },
            //Room 9
            new()
            {
                RoomNumber = 9,
                MusicLayer = 7,
                ExitDoors = new ExitDoor[]{
                   new( new Vector2Int(2, 0), 12),
                   new( new Vector2Int(0, 2), 10),
                   new( new Vector2Int(0, -2),11),
                },
                EnemyPositions = new Vector2Int[]
                {
                    new Vector2Int(-2, -2),
                    new Vector2Int(-1, -1),
                },
                ObstaclePositions = new Vector2Int[]
                {
                    new Vector2Int(0, 1),
                }
            },
            //Room 10
            new()
            {
                RoomNumber = 10,
                ExitDoors = null,
                EnemyPositions = new Vector2Int[]
                {
                    new Vector2Int(-2, -2),
                },
                ObstaclePositions = new Vector2Int[]
                {
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 0),
                }
            },
            //Room 11
            new()
            {
                RoomNumber = 11,
                ExitDoors = null,
                EnemyPositions = new Vector2Int[]
                {
                    new Vector2Int(-2, -2),
                    new Vector2Int(2, 2),
                },
                ObstaclePositions = new Vector2Int[]
                {
                    new Vector2Int(0, 1),
                }
            },
            //Room 12
            new()
            {
                RoomNumber = 12,
                MusicLayer = 8,
                ExitDoors = new ExitDoor[]{
                   new( new Vector2Int(0, -2), -420)
                },
                EnemyPositions = new Vector2Int[]
                {
                    new Vector2Int(-2, 2),
                    new Vector2Int(2, 2),
                },
                ObstaclePositions = new Vector2Int[]
                {
                    new Vector2Int(1, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 1),
                }
            },
        };
    }

    public Config GetRoomData(int index)
    {
        return AllRooms[index];
    }

}

