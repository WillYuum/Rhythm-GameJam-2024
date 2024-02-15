using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmController : MonoBehaviour
{
    public int CurrentLayer { get; private set; } = 1;
    private FMODUnity.StudioBankLoader _bankLoader;
    private string _bankName = "Master";
    private string _eventName = "event:/main_song";

    private void Awake()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/ping_me");
        }
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
        FMODUnity.RuntimeManager.GetBus("bus:/main_song").setPaused(value);
    }

    public void Play()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/main_song");
    }
}
