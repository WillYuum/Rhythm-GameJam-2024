using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.GenericSingletons;
public class GameManager : MonoBehaviourSingleton<MonoBehaviour>
{
    public event System.Action OnCloseGame;


    private GameStarter _gameStarter;

    private void Start()
    {

        _gameStarter = new GameStarter();

        MainGameUI mainGameUI = FindObjectOfType<MainGameUI>();
        LoadingScreen.LoadUI loadingScreen = mainGameUI.LoadLoadingScreen();

        loadingScreen.Open.Invoke();

        StartCoroutine(CheckIfBanksLoaded(() =>
        {
            loadingScreen.Close.Invoke();
            _gameStarter.StartGame();
        }));
    }



    private IEnumerator CheckIfBanksLoaded(System.Action callback)
    {
        string bankName = "Master";
        yield return new WaitUntil(() => FMODUnity.RuntimeManager.HasBankLoaded(bankName));
        Debug.Log("|GameManager|: Bank loaded: " + bankName);

        //add minor delay:
        yield return new WaitForSeconds(2f);

        callback.Invoke();
    }

    private void OnDestroy()
    {
        OnCloseGame?.Invoke();
        OnCloseGame = null;
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
