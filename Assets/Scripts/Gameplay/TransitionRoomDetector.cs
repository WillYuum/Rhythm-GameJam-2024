using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionRoomDetector : MonoBehaviour
{


    private int _beatCount = 0;
    private int _beatCountToTransition = 4;

    void Awake()
    {

    }


    public bool CanTransition { get; private set; } = false;

    private void HandleBeat()
    {
        _beatCount++;
        if (_beatCount > _beatCountToTransition)
        {
            _beatCount = 0;
        }

        if (_beatCount == _beatCountToTransition)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/ping_me");
        }

        if (_beatCount == _beatCountToTransition - 1)
        {

        }
    }

    public bool CheckIfOnTransitionBeat()
    {
        return _beatCount == _beatCountToTransition;
    }
}
