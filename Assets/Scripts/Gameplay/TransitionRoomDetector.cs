using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionRoomDetector : MonoBehaviour
{
    private BeatDetector _beatDetector;

    private int _beatCount = 0;
    private int _beatCountToTransition = 4;

    void Awake()
    {
        _beatDetector = GetComponent<BeatDetector>();

        _beatDetector.OnBeat += HandleBeat;
    }


    private void HandleBeat()
    {
        _beatCount++;
        if (_beatCount == _beatCountToTransition)
        {
            _beatCount = 0;
            FMODUnity.RuntimeManager.PlayOneShot("event:/ping_me");
        }
    }

    public bool InvokeTryingToTransition()
    {
        return _beatCount == _beatCountToTransition;
    }
}
