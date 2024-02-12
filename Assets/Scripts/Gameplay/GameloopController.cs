using System.Collections;
using System.Collections.Generic;
using GameplayUtils;
using UnityEngine;
using GameplayUtils.TileMapExtension;
using UnityEngine.Tilemaps;

public class GameloopController : MonoBehaviour
{
    public Vector2Int PlayerCellPosition;
    [SerializeField] private InputManager _inputManager;

    private MapSwitcher _mapSwitcher;

    private Player _player;


    private bool _gameIsActive = false;


    private void Start()
    {
        PlayerCellPosition = new Vector2Int(0, 0);
        _player = FindObjectOfType<Player>();
        _mapSwitcher = FindObjectOfType<MapSwitcher>();
        _inputManager = FindObjectOfType<InputManager>();


        Tilemap currentTileMap = _mapSwitcher.GetActiveMap();
        Vector2 newWorldPos = currentTileMap.GetWorldPosFromCellPos(PlayerCellPosition);
        _player.UpdatePosition(newWorldPos);

        StartLoop();


    }

    public void StartLoop()
    {
        _gameIsActive = true;
        _inputManager.OnMoveInput += MovePlayer;
        _inputManager.Toggle(true);
    }



    private void MovePlayer(Vector2Int direction)
    {
        if (!_gameIsActive)
            return;

        Vector2Int newCellPos = PlayerCellPosition + direction;
        Tilemap currentTileMap = _mapSwitcher.GetActiveMap();

        Debug.Log("Moving to " + newCellPos);

        if (currentTileMap.IsCellPosOutOfBounds(newCellPos))
            return;


        PlayerCellPosition = newCellPos;

        Vector2 newWorldPos = currentTileMap.GetWorldPosFromCellPos(newCellPos);
        _player.UpdatePosition(newWorldPos);
    }


}
