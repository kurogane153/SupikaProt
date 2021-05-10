﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameClearOverManager : MonoBehaviour
{
    public static bool isclear = false;
    public static int _gameoverCount = 0;
    public static int _gameoverCountColony = 0;

    [Header("ゲームクリアまでの隕石の個数")]
    public int GameClearAsteroid = 17;

    [Header("ゲームオーバー・ゲームクリアのリロード時間")]
    public float GameOverReloadTime = 6f;

    [Header("Set Spika Prefab")]
    public GameObject Spika;

    [Header("Set Earth Prefab")]
    public GameObject Earth;

    void Start()
    {
        isclear = false;
    }

    void Update()
    {

        if (Earth.GetComponent<GameClearOver_Process>().GetGameOverCount() >= Earth.GetComponent<GameClearOver_Process>().GetGameOverAsteroidCount())
        {
            Invoke("DelayMethod", GameOverReloadTime);
        }

        if (Spika.GetComponent<GameClearOver_Process>().GetGameOverCount() >= Spika.GetComponent<GameClearOver_Process>().GetGameOverAsteroidCount())
        {
            Invoke("DelayMethod", GameOverReloadTime);
        }

        if (Earth.GetComponent<GameClearOver_Process>().GetGameClearCount() >= GameClearAsteroid)
        {
            
            _gameoverCount = Earth.GetComponent<GameClearOver_Process>().GetGameOverCount();
            _gameoverCountColony = Spika.GetComponent<GameClearOver_Process>().GetGameOverCount();
            Invoke("DelayLastScene", GameOverReloadTime);
        }
    }

    void DelayMethod()
    {
        _gameoverCount = 0;
        _gameoverCountColony = 0;
        SceneManager.LoadScene("Result");
    }

    void DelayLastScene()
    {
        SceneManager.LoadScene("LastBossAppearrScene");
    }
}
