using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatDetector : MonoBehaviour
{
    private FMODUnity.StudioEventEmitter _songEmitter;

    public event System.Action OnBeat;

    public float SongBPM;
    public float SongPosition;
    public float BeatInterval;
    private float _beatTimer;

    private void Awake()
    {
        _songEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
        SongBPM = 90;// predefined when installed this audio file from https://ccmixter.org/files/Apoxode/67769|

        BeatInterval = 60f / SongBPM;
    }

    private void Update()
    {

        FMOD.RESULT result = _songEmitter.EventInstance.getTimelinePosition(out int songPosition);

        _beatTimer += songPosition / 1000f - SongPosition;

        SongPosition = songPosition / 1000f;


        CheckForBeat();
    }

    private void CheckForBeat()
    {
        if (_beatTimer >= BeatInterval)
        {
            OnBeat.Invoke();

            print("_beatTimer: " + _beatTimer);
            _beatTimer = 0;
            Debug.Log("Beat");

        }
    }

    /// <summary>
    /// Check if the current time is around a beat
    /// <para>
    /// If the value is 0.5, it will check half before a beat duration.
    /// If the value is 1, it will check the exact beat duration.
    /// </para>
    /// </summary>
    /// <param name="beforeBeatRatio"></param>
    /// <param name="afterBeatRatio"></param>
    /// <returns></returns>
    public bool CheckIfAroundABeat(float beforeBeatRatio, float afterBeatRatio)
    {
        if (_beatTimer < BeatInterval * beforeBeatRatio || _beatTimer > BeatInterval * afterBeatRatio)
        {
            return false;
        }

        return true;
    }

}
