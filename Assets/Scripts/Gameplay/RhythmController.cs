using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmController : MonoBehaviour
{
    public int CurrentLayer { get; private set; } = 1;

    private MusicTracker _musicTracker;

    private void Awake()
    {
        _musicTracker = GetComponent<MusicTracker>();
    }


    public void SetCurrentLayer(int layer)
    {
        if (layer <= 1)
        {
            layer = 1;
        }

        CurrentLayer = layer;
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("CurrentNumLayer", CurrentLayer);
    }

    public void LoadMusic()
    {
        _musicTracker.Load();
    }

    public void ToggleMusic(bool value)
    {
        switch (value)
        {
            case true:
                _musicTracker.StartMusic();
                break;
            case false:
                _musicTracker.StopMusic();
                break;
        }
    }

    public void PlayEndGameMusic()
    {
        string endGameEvent = "event:/end_game";
        FMODUnity.RuntimeManager.PlayOneShot(endGameEvent);
    }

}
