using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionRoomDetector : MonoBehaviour
{


    private int _beatCount = 0;
    private int _beatCountToTransition = 5;
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
            CanTransition = true;
            FMODUnity.RuntimeManager.PlayOneShot("event:/cue");
        }
        else
        {
            CanTransition = false;
        }

        if (_beatCount == _beatCountToTransition - 1)
        {

        }
    }

    public bool CheckIfOnTransitionBeat()
    {
        bool isOnBeat = CanTransition;

        return isOnBeat && _musicTracker.IsWithinBeatWindow(0.85f, 0.85f);
    }
}
