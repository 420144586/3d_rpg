using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Playables;
public class MainMenu : MonoBehaviour
{
    private Button newGameBtn;
    private Button continueBtn;
    private Button exitBtn;
    private PlayableDirector director;
    

    private void Awake()
    {
        newGameBtn = transform.GetChild(1).GetComponent<Button>();
        continueBtn = transform.GetChild(2).GetComponent<Button>();
        exitBtn = transform.GetChild(3).GetComponent<Button>();
        
        newGameBtn.onClick.AddListener(playTimeLine);
        continueBtn.onClick.AddListener(continueGame);
        exitBtn.onClick.AddListener(quitGame);

        director = FindObjectOfType<PlayableDirector>();
        director.stopped += newGame;
    }

    void playTimeLine()
    {
        director.Play();

    }

    void newGame(PlayableDirector obj)
    {
        PlayerPrefs.DeleteAll();
        //转换场景
        SceneController.Instance.transitionToFirstLevel();
    }

    void continueGame()
    {
        //转换场景读取记录
        if(SaveManager.Instance.SceneName == null) return;
        SceneController.Instance.transitionToLoadGame();
    }

    void quitGame()
    {
        Application.Quit();
    }
    
    
}
