using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Animator _loadingAnimator;
    [SerializeField] private Image _loadingBackground;

    public struct LoadUI
    {
        public System.Action Open;
        public System.Action Close;
    }


    public LoadUI Load()
    {
        return new LoadUI
        {
            Open = HandleOpen,
            Close = HandleClose
        };
    }


    private void HandleOpen()
    {
        gameObject.SetActive(true);
        string stateName = "Hour_Glass_Animation";
        _loadingAnimator.Play(stateName);
    }


    private void HandleClose()
    {
        _loadingAnimator.StopPlayback();
        _loadingAnimator.gameObject.SetActive(false);

        _loadingBackground.DOFade(0, 0.5f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
