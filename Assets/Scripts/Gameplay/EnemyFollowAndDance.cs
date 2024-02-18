using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;

public class EnemyFollowAndDance : MonoBehaviour
{
    private List<Vector2Int> danceSteps; // Define dance steps or patterns
    private int currentDanceStepIndex = 0;

    private int beatsPassed = 0;
    private int beatsUntilDance = 5; // Perform a dance every 3 beats

    private bool _isDancing = false;

    private Transform _target;

    private void Start()
    {
        // Initialize dance steps
        danceSteps = new List<Vector2Int>
        {
            new Vector2Int(1, 0),  // Move right
            new Vector2Int(0, 1),  // Move up
            // Add more dance steps as needed
        };

        _target = FindObjectOfType<Player>().transform;

    }



    private void OnGUI()
    {
        // GUILayoutOption
        // GUI.Label(new Rect(10, 10, 100, 20), "isDancing: " + _isDancing);
        GUILayout.Box("isDancing: " + _isDancing, GUILayout.Width(100), GUILayout.Height(20));

    }


    private MusicTracker _musicTracker;
    private GridController _gridController;
    public void Prepare(MusicTracker musicTracker, GridController gridController)
    {
        _musicTracker = musicTracker;
        _gridController = gridController;


        _musicTracker.fixedBeatUpdate += HandleBeat;
    }


    private void HandleBeat()
    {
        print("|EnemyFollowAndDance| HandleBeat() called!");
        if (_isDancing)
        {
            if (currentDanceStepIndex >= danceSteps.Count)
            {
                currentDanceStepIndex = 0;
                _isDancing = false;
            }
            else
            {
                Vector2Int danceStepDir = danceSteps[currentDanceStepIndex];
                Vector2Int nextPos = _gridController.GetCellPosFromWorldPos(transform.position) + danceStepDir;
                currentDanceStepIndex++;

                //The check is useless since the data generated took care of the checks
                if (_gridController.CheckIfCanMoveToPosition(nextPos))
                {
                    HandleMove(nextPos);
                }

            }



            return;
        }


        beatsPassed++;
        if (beatsPassed >= beatsUntilDance)
        {
            beatsPassed = 0;
            EnterDanceMode();
        }
        else
        {
            // Chase the player on every beat when not dancing
            Vector2Int currentPosition = _gridController.GetCellPosFromWorldPos(transform.position);

            Vector2Int playerPosition = _gridController.GetCellPosFromWorldPos(_target.position);

            // Calculate the direction to move towards the player
            Vector2Int directionToPlayer = CalculateDirectionToPlayer(currentPosition, playerPosition);

            // Calculate the next position based on the direction
            Vector2Int nextPos = currentPosition + directionToPlayer;

            HandleMove(nextPos);
        }
    }

    private void EnterDanceMode()
    {
        _isDancing = true;

        Vector2Int currnetPosition = _gridController.GetCellPosFromWorldPos(transform.position);
        List<Vector2Int> possibleDanceSteps = new List<Vector2Int>();

        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
        };

        foreach (Vector2Int direction in directions)
        {
            Vector2Int nextPos = currnetPosition + direction;
            if (!_gridController.CheckIfCanMoveToPosition(nextPos))
            {
                possibleDanceSteps.Add(direction);
            }

            if (possibleDanceSteps.Count >= 2)
            {
                break;
            }
        }

        danceSteps = possibleDanceSteps;
    }


    private void HandleMove(Vector2Int nextPos)
    {
        Debug.Log("|Enemy| Moving to " + nextPos);
        transform.position = _gridController.GetWorldPosFromCellPos(nextPos);
    }


    private Vector2Int CalculateDirectionToPlayer(Vector2Int currentPosition, Vector2Int playerPosition)
    {
        // Calculate the direction to move towards the player
        Vector2Int directionToPlayer = Vector2Int.zero;

        //Calculate direction without moving diagonally
        if (currentPosition.x < playerPosition.x)
        {
            directionToPlayer = Vector2Int.right;
        }
        else if (currentPosition.x > playerPosition.x)
        {
            directionToPlayer = Vector2Int.left;
        }
        else if (currentPosition.y < playerPosition.y)
        {
            directionToPlayer = Vector2Int.up;
        }
        else if (currentPosition.y > playerPosition.y)
        {
            directionToPlayer = Vector2Int.down;
        }

        if (directionToPlayer == Vector2Int.zero)
        {
            Debug.Log("Player is on the same cell as the enemy");
        }

        return directionToPlayer;
    }
}
