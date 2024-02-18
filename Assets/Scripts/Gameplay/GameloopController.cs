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
    private int _currentRoomLevel;
    public int CurrentRoomLevel
    {
        get => _currentRoomLevel;
        set
        {
            _currentRoomLevel = value;
            _rhythmController.SetCurrentLayer(value);
        }
    }

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

    }

    public void StartLoop()
    {
        if (_gameIsActive)
        {
            Debug.LogWarning("|GameLoopController| Game is already active!");
            return;
        }

        print("|GameLoopController| Starting game loop");

        _gameIsActive = true;
        _inputManager.OnMoveInput += MovePlayer;
        _inputManager.OnClickTransition += HandlePlayerClickTransition;
        _inputManager.Toggle(true);

        _rhythmController.LoadMusic();
        _rhythmController.ToggleMusic(true);
        CurrentRoomLevel = 1;

        _selectedGrid.BuildUpObjectsInRoom(CurrentRoomLevel);
    }



    private void MovePlayer(Vector2Int direction)
    {
        if (!_gameIsActive)
            return;


        // BeatDetector beatDetector = _rhythmController.GetComponent<BeatDetector>();
        MusicTracker beatTracker_V2 = FindObjectOfType<MusicTracker>();


        if (!beatTracker_V2.CheckIfInBeatWindow(0.25f, 1.65f))
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

    private void OnGUI()
    {
        //draw the player pos
        // GUI.Label(new Rect(10, 10, 100, 20), "Player pos: " + PlayerCellPosition.x + " " + PlayerCellPosition.y);

        string playerPos = "Player pos: " + PlayerCellPosition.x + " " + PlayerCellPosition.y;
        // GUI.Box(new Rect(10, 30, 100, 20), "Room level: " + CurrentRoomLevel);
        GUILayout.Box(playerPos, GUILayout.Width(100), GUILayout.Height(20));
    }


    private void HandlePlayerClickTransition()
    {
        if (!_gameIsActive)
            return;


        TransitionRoomDetector transitionRoomDetector = _rhythmController.GetComponent<TransitionRoomDetector>();

        if (transitionRoomDetector.CheckIfOnTransitionBeat())
        {
            //Handle logic for checking if player clicked near beat when transition queue is played
            // Debug.Break();
            CurrentRoomLevel++;
            _selectedGrid.BuildUpObjectsInRoom(CurrentRoomLevel);


            bool currentRoomIsLast = _selectedGrid.CurrentRoomIsLastRoom(CurrentRoomLevel);
            if (currentRoomIsLast)
            {
                Debug.Log("Last room");
                WinGame();
            }
        }
    }



    private void WinGame()
    {
        _gameIsActive = false;
        _inputManager.Toggle(false);
        _rhythmController.ToggleMusic(false);
    }
}
