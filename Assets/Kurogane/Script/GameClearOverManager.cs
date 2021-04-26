using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameClearOverManager : MonoBehaviour
{
    public GameObject GameOverCountText = null; // Textオブジェクト
    public GameObject GameOverCountTextSpika = null; // Textオブジェクト
    public GameObject GameClearCountText = null; // Textオブジェクト

    [Header("ゲームオーバーまでの隕石の個数")]
    public int GameOverAsteroid = 3;

    [Header("ゲームクリアまでの隕石の個数")]
    public int GameClearAsteroid = 25;

    [Header("ゲームオーバーのリロード時間")]
    public float GameOverReloadTime = 3.5f;

    [Header("Set Spika Prefab")]
    public GameObject Spika;

    [Header("Set Earth Prefab")]
    public GameObject Earth;

    void Start()
    {
        
    }

    void Update()
    {
        GameOverCountText.GetComponent<Text>().text = "地球滅亡まで残り:" + (GameOverAsteroid - Earth.GetComponent<GameClearOver_Process>().GetGameOverCount());
        GameOverCountTextSpika.GetComponent<Text>().text = "コロニー滅亡まで残り:" + (GameOverAsteroid - Spika.GetComponent<GameClearOver_Process>().GetGameOverCount());
        GameClearCountText.GetComponent<Text>().text = "防衛成功まで残り:" + (GameClearAsteroid - Earth.GetComponent<GameClearOver_Process>().GetGameClearCount());
    }
}
