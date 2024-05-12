
using DG.Tweening;
using GameplayUtils;
using UnityEngine;

public class GameloopController : MonoBehaviour
{
    public Vector2Int PlayerCellPosition;
    private InputManager _inputManager;
    private RhythmController _rhythmController;
    private MainGameUI _mainGameUI;

    private RoomsData _roomsData;

    public RoomsData.Config PrevRoomData { get; private set; }


    private GridController _currentActiveRoom;

    private Player _player;


    private bool _gameIsActive = false;

    private void Awake()
    {
        _inputManager = FindObjectOfType<InputManager>();
        _currentActiveRoom = FindObjectOfType<GridController>();
        _player = FindObjectOfType<Player>();
        _rhythmController = FindObjectOfType<RhythmController>();
        _mainGameUI = FindObjectOfType<MainGameUI>();
    }


    private void Start()
    {
        PlayerCellPosition = new Vector2Int(0, 0);


        // Tilemap currentTileMap = _mapSwitcher.GetActiveMap();
        Vector2 newWorldPos = _currentActiveRoom.GetWorldPosFromCellPos(PlayerCellPosition);
        _player.UpdatePosition(newWorldPos);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.U))
        {
            TransitionToRoom(_currentActiveRoom.Data.RoomNumber + 1);
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            TransitionToRoom(_currentActiveRoom.Data.RoomNumber - 1);
        }
#endif
    }

    public void StartLoop()
    {
        if (_gameIsActive)
        {
            Debug.LogWarning("|GameLoopController| Game is already active!");
            return;
        }

        print("|GameLoopController| Starting game loop");

        _roomsData = new RoomsData();

        _gameIsActive = true;
        _inputManager.OnMoveInput += MovePlayer;
        _inputManager.OnClickTransition += HandlePlayerClickTransition;
        _inputManager.Toggle(true);

        _rhythmController.LoadMusic();
        _rhythmController.ToggleMusic(true);

        TransitionToRoom(1);
    }



    private void MovePlayer(Vector2Int direction)
    {
        if (!_gameIsActive)
            return;


        MusicTracker beatTracker_V2 = FindObjectOfType<MusicTracker>();

        bool clickedAroundBeat = beatTracker_V2.IsWithinBeatWindow(0.85f, 0.5f);
        if (!clickedAroundBeat)
        {
            Debug.Log("Not near a beat");
            return;
        }

        Vector2Int newCellPos = PlayerCellPosition + direction;


        Debug.Log("Moving to " + newCellPos);

        if (_currentActiveRoom.CheckIfCanMoveToPosition(newCellPos))
        {
            Debug.Log("Can't move to " + newCellPos);
            return;
        }


        PlayerCellPosition = newCellPos;

        Vector2 newWorldPos = _currentActiveRoom.GetWorldPosFromCellPos(newCellPos);
        _player.UpdatePosition(newWorldPos);
    }


    private void HandlePlayerClickTransition()
    {
        if (!_gameIsActive)
            return;

        TransitionRoomDetector transitionRoomDetector = _rhythmController.GetComponent<TransitionRoomDetector>();

        if (transitionRoomDetector.CheckIfOnTransitionBeat() && CheckIfPlayerIsOnExitDoor(out int roomNumber))
        {
            //For now room 12 is the end game room 
            bool nextRoomIsEndGame = _currentActiveRoom.Data.RoomNumber == 12;
            if (nextRoomIsEndGame)
            {
                Debug.Log("Last room");
                WinGame();
            }
            else
            {
                TransitionToRoom(roomNumber);
            }

        }
    }

    private void TransitionToRoom(int roomNumber)
    {
        Debug.Log("Transitioning to room " + roomNumber);

        int roomIndex = roomNumber - 1;
        RoomsData.Config roomData = _roomsData.GetRoomData(roomIndex);

        PrevRoomData = _currentActiveRoom.Data;

        var newRoom = Instantiate(_currentActiveRoom, Vector2.zero, Quaternion.identity).GetComponent<GridController>();

        Destroy(_currentActiveRoom.gameObject);

        newRoom.ConstructRoom(roomData);
        _rhythmController.SetCurrentLayer(roomNumber);


        _currentActiveRoom = newRoom;
    }



    private bool CheckIfPlayerIsOnExitDoor(out int roomNumber)
    {
        roomNumber = -1;
        if (!_gameIsActive)
            return false;

        Vector2Int playerCellPos = PlayerCellPosition;
        RoomsData.Config currentRoomData = _currentActiveRoom.Data;

        for (int i = 0; i < currentRoomData.ExitDoors.Length; i++)
        {
            Vector2Int exitDoorPos = currentRoomData.ExitDoors[i].Position;
            if (playerCellPos == exitDoorPos)
            {
                roomNumber = currentRoomData.ExitDoors[i].RoomNumber;
                return true;
            }
        }


        return false;
    }


    public void AfterEnemyMove(EnemyFollowAndDance enemy)
    {
        if (!_gameIsActive)
            return;

        Vector2Int enemyCellPos = _currentActiveRoom.GetCellPosFromWorldPos(enemy.transform.position);
        if (enemyCellPos == PlayerCellPosition)
        {
            Debug.Log("Player got caught by enemy");

            TransitionToRoom(PrevRoomData.RoomNumber);
        }
    }



    private void WinGame()
    {
        if (!_gameIsActive)
            return;

        _gameIsActive = false;
        _inputManager.Toggle(false);
        _rhythmController.ToggleMusic(false);

        _mainGameUI.ShowEndGameScreen();
        _rhythmController.PlayEndGameMusic();
    }
}
