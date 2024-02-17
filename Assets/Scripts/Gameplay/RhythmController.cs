using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmController : MonoBehaviour
{
    public int CurrentLayer { get; private set; } = 1;

    private FMODUnity.StudioEventEmitter _songEmitter;

    private void Awake()
    {
        _songEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
    }

    private void Start()
    {
        GetComponent<BeatDetector>().enabled = true;
    }

    private void Update()
    {

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

    public void ToggleMusic(bool value)
    {
        switch (value)
        {
            case true:
                _songEmitter.Play();
                break;
            case false:
                _songEmitter.Stop();
                break;
        }


        GetComponent<BeatDetector>().enabled = value;
    }

}
