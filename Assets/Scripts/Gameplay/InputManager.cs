using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public System.Action<Vector2Int> OnMoveInput;

    private void Update()
    {
        HandleClickedMove();
    }


    public void Toggle(bool value)
    {
        enabled = value;
    }


    private void HandleClickedMove()
    {
        if (Input.GetKeyDown(KeyCode.W))
            OnMoveInput?.Invoke(Vector2Int.up);

        else if (Input.GetKeyDown(KeyCode.S))
            OnMoveInput?.Invoke(Vector2Int.down);

        else if (Input.GetKeyDown(KeyCode.A))
            OnMoveInput?.Invoke(Vector2Int.left);

        else if (Input.GetKeyDown(KeyCode.D))
            OnMoveInput?.Invoke(Vector2Int.right);
    }

}
