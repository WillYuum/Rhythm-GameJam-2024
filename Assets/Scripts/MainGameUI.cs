using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameUI : MonoBehaviour
{
    [SerializeField] private GameObject _endGameScreen;
    [SerializeField] private Material roomTransitionMaterial;
    [SerializeField] private Image _screenTransitionImage;


    [SerializeField] private LoadingScreen _loadingScreen;

    public Color colorA = Color.white;
    public Color colorB = Color.black;
    public float lerpSpeed = 0.5f;

    void Start()
    {
        _endGameScreen.SetActive(false);
    }



    public void ShowEndGameScreen()
    {
        _endGameScreen.SetActive(true);
    }


    public LoadingScreen.LoadUI LoadLoadingScreen()
    {
        return _loadingScreen.Load();
    }
}
