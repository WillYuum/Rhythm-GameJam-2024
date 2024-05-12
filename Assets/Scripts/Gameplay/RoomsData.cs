
using UnityEngine;

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
                    new(new Vector2Int(0, -2), 2),

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
                   new( new Vector2Int(0, -2), 3)
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
                    new(new Vector2Int(0, -2), 4)
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
                   new( new Vector2Int(2, 0), 6),
                   new( new Vector2Int(0, -2), 5),
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
                   new( Vector2Int.right, 8),
                   new( Vector2Int.up, 7),
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
                   new(  Vector2Int.left, 3)
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
                   new( Vector2Int.right, 9)
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
                   new( Vector2Int.right, 12),
                   new( Vector2Int.up, 10),
                   new( Vector2Int.down,11),
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

    public Config GetRoomData(int roomIndex)
    {
        return AllRooms[roomIndex];
    }

}

