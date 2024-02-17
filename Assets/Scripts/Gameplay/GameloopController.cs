using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameplayUtils;
using UnityEditorInternal;
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
        _inputManager.OnClickTransition += HandlePlayerClickTransition;
        _inputManager.Toggle(true);

        _rhythmController.ToggleMusic(true);
        _rhythmController.SetCurrentLayer(1);

    }



    private void MovePlayer(Vector2Int direction)
    {
        if (!_gameIsActive)
            return;


        BeatDetector beatDetector = _rhythmController.GetComponent<BeatDetector>();


        if (!beatDetector.CheckIfAroundABeat(0.25f, 1.65f))
        {
            Debug.Log("Not near a beat");
            return;
        }

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


    private void HandlePlayerClickTransition()
    {
        if (!_gameIsActive)
            return;


        TransitionRoomDetector transitionRoomDetector = _selectedGrid.GetComponent<TransitionRoomDetector>();

        if (transitionRoomDetector.InvokeTryingToTransition())
        {
            BeatDetector beatDetector = _rhythmController.GetComponent<BeatDetector>();
            bool clickedNearBeat = beatDetector.CheckIfAroundABeat(0.25f, 1.65f);

            if (clickedNearBeat)
            {
                Debug.Log("Transitioning to next room");
            }
        }

    }


}
