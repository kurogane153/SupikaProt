using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameClearOver_Process : MonoBehaviour
{

    private int GameOverCount = 0;

    public GameObject GameOverCountText = null; // Textオブジェクト
    public GameObject GameOverText = null; // Textオブジェクト
    private Text GameOverCcunt_text;

    [Header("ゲームオーバーまでの隕石の個数")]
    //隕石の速さ
    public int GameOverAsteroid = 3;

    [Header("ゲームオーバーのリロード時間")]
    //隕石の速さ
    public float GameOverReloadTime = 3.5f;

    void Start()
    {
        GameOverCcunt_text = GameOverCountText.GetComponent<Text>();
    }

    void Update()
    {
        if(GameOverCount >= GameOverAsteroid)
        {
            GameOverText.SetActive(true);
            Invoke("DelayMethod", GameOverReloadTime);
        }
        GameOverCcunt_text.text = "地球滅亡まで残り:" + (GameOverAsteroid - GameOverCount);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Asteroid")
        {
            GameOverCount++;
            Debug.Log("当たった");
            Debug.Log(GameOverCount);
        }
    }

    void DelayMethod()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
