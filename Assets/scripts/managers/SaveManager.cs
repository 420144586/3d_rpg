using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager>
{
    public string sceneName;

    public string SceneName
    {
        get
        {
            return PlayerPrefs.GetString(sceneName); 
        }
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public void saveData(object data, string key)
    {
        var jsonData = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString(key, jsonData);
        PlayerPrefs.SetString(sceneName, SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
    }

    public void loadData(Object data, string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            var jsonData = PlayerPrefs.GetString(key);
            JsonUtility.FromJsonOverwrite(jsonData, data);
        }
    }

    private void Update()
    {

       

        if (Input.GetKeyDown(KeyCode.S))
        {
            savePlayerData();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            loadplayerData();
        }
    }

    public void savePlayerData()
    {
        var cData = GameManager.Instance.playerStats.characterData;
        saveData(cData, cData.name);
    }

    public void loadplayerData()
    {
        var cData = GameManager.Instance.playerStats.characterData;
        loadData(cData, cData.name);
    }
}
