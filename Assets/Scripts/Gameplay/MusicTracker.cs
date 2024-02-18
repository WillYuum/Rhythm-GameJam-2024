/*
* I got this most of this code from https://qa.fmod.com/t/perfect-beat-tracking-in-unity/18788 discusion where the poster
* bloo_regard_q_kazoo asked how to perfect beat tracking in Unity.
* bl0o later posted the code which was on google drive https://drive.google.com/file/d/1r8ROjgsMh-mwKqGTZT7IWMCsJcs3GuU9/view
* 
* I
*/

using UnityEngine;
using System;
using System.Runtime.InteropServices;
using FMODUnity;

public class MusicTracker : MonoBehaviour
{
    private FMOD.ChannelGroup masterChannelGroup;

    [Header("OPTIONS:")]
    public float upBeatDivisor = 2f; // This value changes the offset of the up beats. Changing this value will "swing" the up beats.

    private int masterSampleRate;
    private double currentSamples = 0;
    private double currentTime = 0f;

    private ulong dspClock;
    private ulong parentDSP;

    public double BeatInterval = 0f; // This is the time between each beat;
    private double lastBeatInterval = 0f; // This is the previous time between each beat. It's what the "beatInterval" was before a tempo change.

    static bool justHitBeat = false;

    private double tempoTrackDSPStartTime;

    static string markerString = "";
    static bool justHitMarker = false;
    static int markerTime;

    public delegate void BeatEventDelegate();
    public event BeatEventDelegate fixedBeatUpdate; // Subscribe any function you wan't to happen on the down beat to this event! DON'T FORGET TO UNSUBSCRIBE BEFORE DESTROYING YOU GAMEOBJECTS!

    private double lastFixedBeatTime = -2;
    private double lastFixedBeatDSPTime = -2;

    public event BeatEventDelegate upBeatUpdate; // Subscribe any function you wan't to happen on the up beat to this event.

    private double lastUpBeatTime = -2;
    private double lastUpBeatDSPTime = -2;

    private bool hasDoneEnemyBeat = false;


    public event System.Action<float> TempoChanged;
    public event System.Action MarkerUpdated;


    [SerializeField]
    private EventReference _music;

    private FMOD.Studio.PLAYBACK_STATE _musicPlayState;
    private FMOD.Studio.PLAYBACK_STATE _lastMusicPlayState;

    [StructLayout(LayoutKind.Sequential)]
    public class TimelineInfo
    {
        public int currentBeat = 0;
        public int currentBar = 0;
        public int beatPosition = 0;
        public float currentTempo = 0;
        public float lastTempo = 0;
        public int currentPosition = 0;
        public double songLength = 0;
        public FMOD.StringWrapper lastMarker = new FMOD.StringWrapper();
    }

    public TimelineInfo _timelineInfo = null;

    private GCHandle _timelineHandle;

    private FMOD.Studio.EVENT_CALLBACK _beatCallback;
    private FMOD.Studio.EventDescription _descriptionCallback;

    public FMOD.Studio.EventInstance _musicPlayEvent;



    private void Update()
    {
        if (!_isPlaying)
        {
            return;
        }

        _musicPlayEvent.getPlaybackState(out _musicPlayState);

        if (_lastMusicPlayState != FMOD.Studio.PLAYBACK_STATE.PLAYING && _musicPlayState == FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            SetTrackStartInfo();
        }


        _lastMusicPlayState = _musicPlayState;

        if (_musicPlayState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            return;
        }

        _musicPlayEvent.getTimelinePosition(out _timelineInfo.currentPosition);

        UpdateDSPClock();

        CheckTempoMarkers();

        if (BeatInterval == 0f)
        {
            return;
        }

        if (justHitMarker)
        {
            justHitMarker = false;

            if (lastFixedBeatDSPTime < currentTime - (BeatInterval / 2f))
            {
                DoFixedBeat(); // We trigger the beat immediately if we're far enough past the last beat. This will help correct the timing when we hit a marker;
            }

            _musicPlayEvent.getTimelinePosition(out int currentTimelinePos);

            float offset = (currentTimelinePos - markerTime) / 1000f;

            tempoTrackDSPStartTime = currentTime - offset;
            lastFixedBeatTime = 0f;
            lastFixedBeatDSPTime = tempoTrackDSPStartTime;

            lastUpBeatTime = 0f;
            lastUpBeatDSPTime = tempoTrackDSPStartTime;

            if (MarkerUpdated != null)
            {
                MarkerUpdated.Invoke();
            }
        }

        CheckNextBeat();
    }


    void OnDestroy()
    {
        print("|BeatTracker| OnDestroy");

        StopMusic(false);
        _musicPlayEvent.setUserData(IntPtr.Zero);

        _musicPlayEvent.release();
        _timelineHandle.Free();
    }

    private void AssignMusicCallbacks()
    {
        _timelineInfo = new TimelineInfo();
        _beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);

        _timelineHandle = GCHandle.Alloc(_timelineInfo, GCHandleType.Pinned);
        _musicPlayEvent.setUserData(GCHandle.ToIntPtr(_timelineHandle));
        _musicPlayEvent.setCallback(_beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);

        _musicPlayEvent.getDescription(out _descriptionCallback);
        _descriptionCallback.getLength(out int length);

        _timelineInfo.songLength = length;


        FMODUnity.RuntimeManager.CoreSystem.getMasterChannelGroup(out masterChannelGroup);

        FMODUnity.RuntimeManager.CoreSystem.getSoftwareFormat(out masterSampleRate, out FMOD.SPEAKERMODE speakerMode, out int numRawSpeakers);
    }


    public void Load()
    {
        FMOD.Studio.EventDescription des;

        _musicPlayEvent.getDescription(out des);

        des.loadSampleData();

        _musicPlayEvent = RuntimeManager.CreateInstance(_music);
    }

    private bool _isPlaying = false;
    public void StartMusic()
    {
        _musicPlayEvent.start();
        _isPlaying = true;

        AssignMusicCallbacks();
    }


    public void StopMusic(bool AllowFadeout = false)
    {
        _isPlaying = false;
        _musicPlayEvent.stop(AllowFadeout ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
    }


    private void SetTrackStartInfo()
    {
        UpdateDSPClock();

        tempoTrackDSPStartTime = currentTime;
        lastFixedBeatTime = 0f;
        lastFixedBeatDSPTime = currentTime;
    }

    private void UpdateDSPClock()
    {
        masterChannelGroup.getDSPClock(out dspClock, out parentDSP);

        currentSamples = dspClock;
        currentTime = currentSamples / masterSampleRate;
    }


    public bool CheckIfInBeatWindow(float beforeBeatRatio, float afterBeatRatio)
    {
        if (lastFixedBeatTime == 0f)
        {
            return false;
        }

        float fixedSongPosition = (float)(currentTime - tempoTrackDSPStartTime);
        float timeSinceLastBeat = fixedSongPosition - (float)lastFixedBeatTime;

        if (timeSinceLastBeat < (float)BeatInterval * beforeBeatRatio || timeSinceLastBeat > (float)BeatInterval * afterBeatRatio)
        {
            return false;
        }

        return true;
    }

    private float UpBeatPosition()
    {
        return (float)BeatInterval / upBeatDivisor;
    }

    private void CheckNextBeat()
    {

        float fixedSongPosition = (float)(currentTime - tempoTrackDSPStartTime);
        float upBeatSongPosition = fixedSongPosition + UpBeatPosition();

        // FIXED BEAT (down beat)
        if (fixedSongPosition >= lastFixedBeatTime + BeatInterval)
        {

            float correctionAmount = Mathf.Repeat(fixedSongPosition, (float)BeatInterval); // This is the amount of time that we're off from the beat...

            DoFixedBeat();

            lastFixedBeatTime = (fixedSongPosition - correctionAmount); // ... we subtract that time from the current time to correct the timing off the next beat.
            lastFixedBeatDSPTime = (currentTime - correctionAmount); // So if this beat is late by 0.1 seconds, the next beat will happen 0.1 seconds sooner.

        }

        // UP BEAT
        if (upBeatSongPosition >= lastUpBeatTime + BeatInterval)
        {
            float correctionAmount = Mathf.Repeat(upBeatSongPosition, (float)BeatInterval);

            DoUpBeat();

            lastUpBeatTime = (upBeatSongPosition - correctionAmount);
            lastUpBeatDSPTime = ((currentTime + UpBeatPosition()) - correctionAmount);
        }
    }

    private void DoFixedBeat() // This is called when the "Down Beat" happens.
    {
        if (fixedBeatUpdate != null)
        {
            fixedBeatUpdate();
        }
    }

    private void DoUpBeat() // This is called when the "Up Beat" happens
    {

        if (upBeatUpdate != null)
        {
            upBeatUpdate();
        }
    }

    private bool CheckTempoMarkers()
    {
        if (_timelineInfo.currentTempo != _timelineInfo.lastTempo)
        {

            SetTrackTempo();

            return true;
        }

        return false;
    }

    private void SetTrackTempo()
    {
        print("|BeatTracker| Tempo Changed: " + _timelineInfo.currentTempo);
        _musicPlayEvent.getTimelinePosition(out int currentTimelinePos);

        float offset = (currentTimelinePos - _timelineInfo.beatPosition) / 1000f;


        tempoTrackDSPStartTime = currentTime - offset;

        lastFixedBeatTime = 0f;
        lastFixedBeatDSPTime = tempoTrackDSPStartTime;

        lastUpBeatTime = 0f;
        lastUpBeatDSPTime = tempoTrackDSPStartTime;

        lastBeatInterval = BeatInterval;

        _timelineInfo.lastTempo = _timelineInfo.currentTempo;

        BeatInterval = 60f / _timelineInfo.currentTempo;

        TempoChanged?.Invoke((float)BeatInterval);
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

        // Retrieve the user data
        FMOD.RESULT result = instance.getUserData(out IntPtr timelineInfoPtr);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Timeline Callback error: " + result);
        }
        else if (timelineInfoPtr != IntPtr.Zero)
        {
            // Get the object to store beat and marker details
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

            switch (type)
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        // There's more info about the callback in the "parameter" variable.

                        var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                        timelineInfo.currentBar = parameter.bar;
                        timelineInfo.currentBeat = parameter.beat;
                        timelineInfo.beatPosition = parameter.position;
                        timelineInfo.currentTempo = parameter.tempo;

                        justHitBeat = true;
                    }
                    break;
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        // Same here.

                        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));

                        timelineInfo.lastMarker = parameter.name;
                        markerString = parameter.name;
                        markerTime = parameter.position;
                        justHitMarker = true;
                    }
                    break;
            }
        }
        return FMOD.RESULT.OK;
    }
}
