using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameplayUtils.TileMapExtension
{
    public static class TileMapExtensions
    {
        public static bool IsCellPosOutOfBounds(this Tilemap tileMap, Vector2Int cellPos)
        {
            return !tileMap.HasTile((Vector3Int)cellPos);
        }

        public static Vector2 GetWorldPosFromCellPos(this Tilemap tileMap, Vector2Int cellPos)
        {
            return tileMap.GetCellCenterWorld((Vector3Int)cellPos);
        }

        public static Vector2Int GetCellPosFromWorldPos(this Tilemap tileMap, Vector2 worldPos)
        {
            return (Vector2Int)tileMap.WorldToCell(worldPos);
        }
    }
}
