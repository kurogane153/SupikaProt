using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameClearOverManager : MonoBehaviour
{
    public static bool isclear = false;
    private int _gameoverCount = 0;
    private int _gameoverCountColony = 0;

    [Header("ゲームオーバーまでの隕石の個数")]
    public int GameOverAsteroid = 3;

    [Header("ゲームクリアまでの隕石の個数")]
    public int GameClearAsteroid = 17;

    [Header("ゲームオーバー・ゲームクリアのリロード時間")]
    public float GameOverReloadTime = 3.5f;

    [Header("Set Spika Prefab")]
    public GameObject Spika;

    [Header("Set Earth Prefab")]
    public GameObject Earth;

    void Start()
    {
        Earth.GetComponent<GameClearOver_Process>().SetGameOverCount(_gameoverCount);
        Spika.GetComponent<GameClearOver_Process>().SetGameOverCount(_gameoverCountColony);
    }

    void Update()
    {

        if (Earth.GetComponent<GameClearOver_Process>().GetGameOverCount() >= GameOverAsteroid)
        {
            Invoke("DelayMethod", GameOverReloadTime);
        }

        if (Spika.GetComponent<GameClearOver_Process>().GetGameOverCount() >= GameOverAsteroid)
        {
            Invoke("DelayMethod", GameOverReloadTime);
        }

        if (Earth.GetComponent<GameClearOver_Process>().GetGameClearCount() >= GameClearAsteroid)
        {
            isclear = true;
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
        _gameoverCount = Earth.GetComponent<GameClearOver_Process>().GetGameOverCount();
        _gameoverCountColony = Spika.GetComponent<GameClearOver_Process>().GetGameOverCount();
        SceneManager.LoadScene("KuroganeScene");
    }
}
