using UnityEngine;
using UnityEngine.Tilemaps;
using GameplayUtils.TileMapExtension;

[RequireComponent(typeof(RoomSpawnManager))]
public class GridController : MonoBehaviour
{
    [SerializeField] private Tilemap _activeTilemap;
    private RoomSpawnManager _roomSpawnManager;
    [SerializeField] private Transform _spawnedObjectsParent;

    public RoomsData.Config Data { get; private set; }

    private void Awake()
    {
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


    public void ConstructRoom(RoomsData.Config roomDataToLoad)
    {
        ClearCreateObjectsOnGrid();

        Data = roomDataToLoad;

        var doorPrefab = _roomSpawnManager.DoorExitPrefab;
        var enemyPrefab = _roomSpawnManager.EnemyPrefab;
        var obstaclePrefab = _roomSpawnManager.ObstaclePrefab;


        MusicTracker musicTracker = FindObjectOfType<MusicTracker>();

        if (roomDataToLoad.ExitDoors != null)
        {
            foreach (var doorData in roomDataToLoad.ExitDoors)
            {
                Debug.Log($"Door at {doorData.Position}");
                doorPrefab.CreateGameObject(_activeTilemap.GetWorldPosFromCellPos(doorData.Position), Quaternion.identity, _spawnedObjectsParent);

            }
        }

        if (roomDataToLoad.EnemyPositions != null)
        {
            foreach (var enemyPos in roomDataToLoad.EnemyPositions)
            {
                Debug.Log($"Enemy at {enemyPos}");
                GameObject enemy = enemyPrefab.CreateGameObject(_activeTilemap.GetWorldPosFromCellPos(enemyPos), Quaternion.identity, _spawnedObjectsParent);
                enemy.GetComponent<EnemyFollowAndDance>().Prepare(musicTracker, this);
            }
        }

        if (roomDataToLoad.ObstaclePositions != null)
        {
            foreach (var obstaclePos in roomDataToLoad.ObstaclePositions)
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

