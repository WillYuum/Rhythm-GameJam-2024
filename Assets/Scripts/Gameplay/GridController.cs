using UnityEngine;
using UnityEngine.Tilemaps;
using GameplayUtils.TileMapExtension;

[RequireComponent(typeof(RoomSpawnManager))]
public class GridController : MonoBehaviour
{
    [SerializeField] private Tilemap _activeTilemap;
    private RoomSpawnManager _roomSpawnManager;

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


    public bool CurrentRoomIsLastRoom(int currentRoomNumber)
    {
        int roomIndex = currentRoomNumber - 1;
        return roomIndex == _roomsData.AllRooms.Length;
    }


    public void BuildUpObjectsInRoom(int roomNumber)
    {
        int roomIndex = roomNumber - 1;
        if (roomIndex < 0 || roomIndex >= _roomsData.AllRooms.Length)
        {
            Debug.LogError("|GridController| Room index out of bounds");
            return;
        }

        if (CurrentRoomIsLastRoom(roomNumber))
        {
            return;
        }


        ClearCreateObjectsOnGrid();


        var roomData = _roomsData.GetRoomData(roomIndex);

        var doorPrefab = _roomSpawnManager.DoorExitPrefab;
        var enemyPrefab = _roomSpawnManager.EnemyPrefab;
        var obstaclePrefab = _roomSpawnManager.ObstaclePrefab;

        if (roomData.DoorsToExitPositions != null)
        {
            foreach (Vector2Int doorPos in roomData.DoorsToExitPositions)
            {
                Debug.Log($"Door at {doorPos}");
                doorPrefab.CreateGameObject(_activeTilemap.GetWorldPosFromCellPos(doorPos), Quaternion.identity, _spawnedObjectsParent);

            }
        }

        if (roomData.EnemyPositions != null)
        {
            foreach (var enemyPos in roomData.EnemyPositions)
            {
                Debug.Log($"Enemy at {enemyPos}");
                enemyPrefab.CreateGameObject(_activeTilemap.GetWorldPosFromCellPos(enemyPos), Quaternion.identity, _spawnedObjectsParent);
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
    public struct RoomData
    {
        public Vector2Int[] DoorsToExitPositions;
        public Vector2Int[] EnemyPositions;
        public Vector2Int[] ObstaclePositions;
    }

    public RoomData[] AllRooms { get; private set; }

    public RoomsData()
    {
        AllRooms = new RoomData[]{
            new () {
                DoorsToExitPositions = new Vector2Int[]{
                    new Vector2Int(0, 0)
                },
                EnemyPositions = null,
                ObstaclePositions = null,
            },
            new()
            {
                DoorsToExitPositions = new Vector2Int[]{
                    new Vector2Int(0, 0)
                },
                EnemyPositions = new Vector2Int[]
                {
                    new Vector2Int(2, 2)
                },
                ObstaclePositions = null,
            },
        };
    }

    public RoomData GetRoomData(int index)
    {
        return AllRooms[index];
    }

}

