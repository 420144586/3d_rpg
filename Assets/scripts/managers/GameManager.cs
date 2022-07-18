using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{

    private CinemachineFreeLook followCamera;
    
    public CharacterStats playerStats;
    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();
    public void RigisterPlayer(CharacterStats player) 
    {
        playerStats = player;
        followCamera = FindObjectOfType<CinemachineFreeLook>();
        if (followCamera != null)
        {
            followCamera.Follow = playerStats.transform.GetChild(2);
            followCamera.LookAt = playerStats.transform.GetChild(2);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneController.Instance.transitionToMainMenu();
        }
    }

    public void addObserver(IEndGameObserver observer)
    {
       
        endGameObservers.Add(observer);
    }

    public void removeObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }

    public void notifyObservers()
    {
        foreach (var observer in endGameObservers)
        {
            observer.endGameNotify();
            
        }
    }

    public Transform getEntrance()
    {
        foreach (var destination in FindObjectsOfType<TransitionDestination>())
        {
            if (destination.destinationTag == TransitionDestination.DestinationTag.ENTER)
            {
                return destination.transform;
            }
        }
        return null;
    }
}
