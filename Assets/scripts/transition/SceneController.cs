using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class SceneController : Singleton<SceneController>, IEndGameObserver
{
    public GameObject playerPrefab;
    private GameObject player;
    private NavMeshAgent playerAgent;
    public SceneFader sceneFaderPrefab;
    private bool isFadeFinished;


    private void Start()
    {
        isFadeFinished = true;
        GameManager.Instance.addObserver(this);
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public void transitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }

    IEnumerator Transition(string sceneName, TransitionDestination.DestinationTag destinationTag)
    {
        //TODO: 保存数据
        SaveManager.Instance.savePlayerData();
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            //异步加载
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return Instantiate(
                playerPrefab, 
                getDestination(destinationTag).transform.position,
                getDestination(destinationTag).transform.rotation);
            SaveManager.Instance.loadplayerData();
            yield break;
        }
        else
        {
            player = GameManager.Instance.playerStats.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();
            playerAgent.enabled = false;
            player.transform.SetPositionAndRotation(getDestination(destinationTag).transform.position, getDestination(destinationTag).transform.rotation);
            playerAgent.enabled = true;
            yield return null;
        }




    }

    private TransitionDestination getDestination(TransitionDestination.DestinationTag destinationTag)
    {
        var entrance = FindObjectsOfType<TransitionDestination>();
        foreach (var destination in entrance)
        {
            if (destination.destinationTag == destinationTag)
            {
                return destination;
            }
        }
        
        return null;
    }

    public void transitionToFirstLevel()
    {
        StartCoroutine(LoadLevel("Forest"));
    }

    public void transitionToLoadGame()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }

    public void transitionToMainMenu()
    {
        StartCoroutine(LoadMainMenu());
    }

    IEnumerator LoadLevel(string scene)
    {
        SceneFader fader = Instantiate(sceneFaderPrefab);

        if (scene != null)
        {
            yield return StartCoroutine(fader.fadeOut(2.5f));
            yield return SceneManager.LoadSceneAsync(scene);
            yield return player = Instantiate(playerPrefab, GameManager.Instance.getEntrance().position, GameManager.Instance.getEntrance().rotation);
            //保存游戏
            SaveManager.Instance.savePlayerData();
            yield return StartCoroutine(fader.fadeIn(2.5f));
            yield break;
        }

        
    }

    IEnumerator LoadMainMenu()  
    {
        SceneFader fader = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fader.fadeOut(2.5f));
        yield return SceneManager.LoadSceneAsync("MainMenu");
        yield return StartCoroutine(fader.fadeIn(2.5f));
        yield break;
    }

    public void endGameNotify()
    {
        if (isFadeFinished)
        {
            isFadeFinished = false;
            StartCoroutine(LoadMainMenu());
        }

        
    }
}
