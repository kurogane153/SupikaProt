using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameClearOver_Process : MonoBehaviour
{

    private int GameOverCount = 0;
    public static int GameClearCount = 0;

    public GameObject GameOverCountText = null; // Textオブジェクト
    public GameObject GameOverText = null; // Textオブジェクト
    public GameObject GameClearText = null; // Textオブジェクト
    public GameObject GameClearCountText = null; // Textオブジェクト
    private Text GameOverCcunt_text;
    private Text GameClearCcunt_text;

    [Header("ゲームオーバーまでの隕石の個数")]
    public int GameOverAsteroid = 3;

    [Header("ゲームクリアまでの隕石の個数")]
    public int GameClearAsteroid = 20;

    [Header("ゲームオーバーのリロード時間")]
    public float GameOverReloadTime = 3.5f;

    void Start()
    {
        GameOverCcunt_text = GameOverCountText.GetComponent<Text>();
        GameClearCcunt_text = GameClearCountText.GetComponent<Text>();
        GameClearText.SetActive(false);
        GameClearCount = 0;
    }

    void Update()
    {
        if(GameOverCount >= GameOverAsteroid)
        {
            GameOverText.SetActive(true);
            Invoke("DelayMethod", GameOverReloadTime);
        }

        if(GameClearCount >= GameClearAsteroid)
        {
            GameClearText.SetActive(true);
            Invoke("DelayMethod", GameOverReloadTime);
        }

        Debug.Log(GameClearCount);
        GameOverCcunt_text.text = "地球滅亡まで残り:" + (GameOverAsteroid - GameOverCount);
        GameClearCcunt_text.text = "防衛成功まで残り:" + (GameClearAsteroid - GameClearCount);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Asteroid")
        {
            GameOverCount++;
        }
    }

    void DelayMethod()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
