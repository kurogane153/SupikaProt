using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    public GameObject GameCleartext = null;
    public GameObject GameOvertext = null;
    private Text GameClear_text;
    private Text GameOver_text;


    // Start is called before the first frame update
    void Start()
    {
        GameClear_text = GameCleartext.GetComponent<Text>();
        GameOver_text = GameOvertext.GetComponent<Text>();
        GameCleartext.SetActive(false);
        GameOvertext.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameClearOver_Process.isclear == false && GameClearOverSpika_Process.isclears == false)
        {
            GameOvertext.SetActive(true);
        }

        if(GameClearOver_Process.isclear == true)
        {
            GameCleartext.SetActive(true);
        }

        
    }

    public void PushRestart()
    {
        //Restartのボタンを押すと、メイン画面に戻る
        SceneManager.LoadScene("S0_ProtoScene_Nagahama");
    }

    public void PushBackToTitle()
    {
        //BackToTitleのボタンを押すと、タイトルに戻る
        SceneManager.LoadScene("Title");
    }
}
