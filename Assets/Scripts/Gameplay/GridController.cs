using UnityEngine;
using UnityEngine.Tilemaps;
using GameplayUtils.TileMapExtension;

public class GridController : MonoBehaviour
{
    [SerializeField] private Tilemap _activeTilemap;




    public bool CheckIfCanMoveToPosition(Vector2Int newCellPos)
    {
        return _activeTilemap.IsCellPosOutOfBounds(newCellPos);
    }

    public Vector2 GetWorldPosFromCellPos(Vector2Int cellPos)
    {
        return _activeTilemap.GetWorldPosFromCellPos(cellPos);
    }
}
