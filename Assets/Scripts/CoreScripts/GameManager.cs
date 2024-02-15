using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.GenericSingletons;
public class GameManager : MonoBehaviourSingleton<MonoBehaviour>
{
    [SerializeField] private FMODUnity.StudioBankLoader _bankLoader;


    private GameStarter _gameStarter;
    private void Awake()
    {
        _gameStarter = new GameStarter();

        StartCoroutine(CheckIfBanksLoaded(_gameStarter.StartGame));
    }


    private IEnumerator CheckIfBanksLoaded(System.Action callback)
    {
        string bankName = "Master";
        yield return new WaitUntil(() => FMODUnity.RuntimeManager.HasBankLoaded(bankName));
        Debug.Log("|GameManager|: Bank loaded: " + bankName);

        callback.Invoke();

    }

}

public class GameStarter
{

    public void StartGame()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        switch (sceneName)
        {
            case "GameScene":
                GameObject.FindObjectOfType<GameloopController>().StartLoop();
                break;

            default:
                Debug.LogError("GameStarter: Scene not found: " + sceneName);
                break;
        }
    }
}
