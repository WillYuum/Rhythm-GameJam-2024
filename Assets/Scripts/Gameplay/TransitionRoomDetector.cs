using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionRoomDetector : MonoBehaviour
{


    private int _beatCount = 0;
    private int _beatCountToTransition = 4;
    private MusicTracker _musicTracker;

    void Awake()
    {
        _musicTracker = GetComponent<MusicTracker>();
    }


    void Start()
    {
        _musicTracker.fixedBeatUpdate += HandleBeat;
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
            FMODUnity.RuntimeManager.PlayOneShot("event:/cue");
        }

        if (_beatCount == _beatCountToTransition - 1)
        {

        }
    }

    public bool CheckIfOnTransitionBeat()
    {
        bool isOnBeat = _beatCount == _beatCountToTransition;

        return isOnBeat && _musicTracker.CheckIfInBeatWindow(0.85f, 1.15f);
    }
}
