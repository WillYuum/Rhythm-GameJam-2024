using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameUI : MonoBehaviour
{
    [SerializeField] private GameObject _endGameScreen;


    void Awake()
    {
        _endGameScreen.SetActive(false);
    }

    public void ShowEndGameScreen()
    {
        _endGameScreen.SetActive(true);
    }
}
