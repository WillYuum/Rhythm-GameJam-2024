using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmController : MonoBehaviour
{


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/ping_me");
        }
    }
}
