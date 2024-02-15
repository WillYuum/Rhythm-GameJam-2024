using System.Collections;
using System.Collections.Generic;
using GameplayUtils;
using UnityEngine;

public class GameloopController : MonoBehaviour
{
    public Vector2Int PlayerCellPosition;
    private InputManager _inputManager;
    private RhythmController _rhythmController;

    private GridController _selectedGrid;

    private Player _player;


    private bool _gameIsActive = false;

    private void Awake()
    {
        _inputManager = FindObjectOfType<InputManager>();
        _selectedGrid = FindObjectOfType<GridController>();
        _player = FindObjectOfType<Player>();
        _rhythmController = FindObjectOfType<RhythmController>();
    }


    private void Start()
    {
        PlayerCellPosition = new Vector2Int(0, 0);


        // Tilemap currentTileMap = _mapSwitcher.GetActiveMap();
        Vector2 newWorldPos = _selectedGrid.GetWorldPosFromCellPos(PlayerCellPosition);
        _player.UpdatePosition(newWorldPos);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            _rhythmController.ToggleMusic(!_gameIsActive);
            _gameIsActive = !_gameIsActive;
        }


        if (Input.GetKeyDown(KeyCode.L))
        {
            _rhythmController.SetCurrentLayer(_rhythmController.CurrentLayer + 1);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            _rhythmController.SetCurrentLayer(_rhythmController.CurrentLayer - 1);
        }
    }

    public void StartLoop()
    {
        if (_gameIsActive)
        {
            Debug.LogWarning("|GameLoopController| Game is already active!");
            return;
        }

        _gameIsActive = true;
        _inputManager.OnMoveInput += MovePlayer;
        _inputManager.Toggle(true);

        _rhythmController.Play();
        _rhythmController.SetCurrentLayer(1);

    }



    private void MovePlayer(Vector2Int direction)
    {
        if (!_gameIsActive)
            return;

        Vector2Int newCellPos = PlayerCellPosition + direction;


        Debug.Log("Moving to " + newCellPos);

        if (_selectedGrid.CheckIfCanMoveToPosition(newCellPos))
        {
            Debug.Log("Can't move to " + newCellPos);
            return;
        }


        PlayerCellPosition = newCellPos;

        Vector2 newWorldPos = _selectedGrid.GetWorldPosFromCellPos(newCellPos);
        _player.UpdatePosition(newWorldPos);
    }


}
