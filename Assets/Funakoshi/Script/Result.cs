using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Result : MonoBehaviour
{

    GameObject ClearCameraObj;
    GameObject OverCameraObj;
    Camera cam;
    Camera cam2;
    public GameObject PlayerOff;

    public GameObject GameCleartext = null;
    public GameObject GameOvertext = null;
    private Text GameClear_text;
    private Text GameOver_text;


    // Start is called before the first frame update
    void Start()
    {
        ClearCameraObj = GameObject.Find("Main Camera");
        OverCameraObj = GameObject.Find("GameOverCamera");
        cam = ClearCameraObj.GetComponent<Camera>();
        cam2 = OverCameraObj.GetComponent<Camera>();

        GameClear_text = GameCleartext.GetComponent<Text>();
        GameOver_text = GameOvertext.GetComponent<Text>();
        GameCleartext.SetActive(false);
        GameOvertext.SetActive(false);

        ClearCameraObj.SetActive(false);
        OverCameraObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameClearOverManager.isclear == false)
        {
            PlayerOff.SetActive(false);
            OverCameraObj.SetActive(true);
            GameOvertext.SetActive(true);
        }

        if(GameClearOverManager.isclear == true)
        {
            ClearCameraObj.SetActive(true);
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
        SceneManager.LoadScene("Title_Supica");
    }
}
