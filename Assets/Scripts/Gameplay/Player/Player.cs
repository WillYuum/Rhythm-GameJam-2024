using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public void UpdatePosition(Vector2 position)
    {
        transform.position = position;
    }

}
