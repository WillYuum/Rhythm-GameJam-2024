using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScaleObjectsOnBeat : MonoBehaviour
{
    private Vector3 originalScale;
    private MusicTracker _currentMusicTracker;

    [SerializeField] public List<Transform> objectsToScale;
    [SerializeField] public float scaleMultiplier = 3f;


    void Start()
    {
        // Store the original scale of the objects
        if (objectsToScale.Count > 0)
        {
            originalScale = objectsToScale[0].localScale;
        }


        _currentMusicTracker = FindObjectOfType<MusicTracker>();
        _currentMusicTracker.fixedBeatUpdate += OnBeatHandler;

    }


    private void OnBeatHandler()
    {
        ScaleObjects();
    }

    private void ScaleObjects()
    {
        float scaleDownDuration = (float)_currentMusicTracker.BeatInterval;
        foreach (Transform obj in objectsToScale)
        {
            obj.DOKill();

            obj.localScale = originalScale * scaleMultiplier;
            obj.DOScale(originalScale, scaleDownDuration);
        }
    }
}
