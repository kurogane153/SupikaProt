using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class RoberiaClearManager : MonoBehaviour
{
    [Header("Set RoberiaAsteroid Prefab")]
    //隕石プレハブ
    public GameObject _roberiaPrefab;

    //TextMesh Proのテキスト、Inspectorで設定
    [Header("テキストウィンドウ")]
    [SerializeField]
    public GameObject _textwindow;

    [Header("最後に表示するテキスト")]
    [SerializeField]
    [TextArea(1, 3)]
    public string _lasttext;

    public GameObject _blackout;
    
    private GameObject _textmessage;

    void Start()
    {
        _textmessage = _textwindow.transform.GetChild(0).gameObject;
    }

    void Update()
    {
        if (RoberiaAsteroidLasCollider._lastshotflg)
        {
            _textwindow.SetActive(true);
            _textwindow.GetComponent<Animator>().SetBool("_textwindowflg", true);
            _textmessage.GetComponent<TextMeshProUGUI>().text = _lasttext;
            Invoke("DelayChangeTextWindow", 3f);
        }

    }

    void DelayChangeTextWindow()
    {
        _textwindow.GetComponent<Animator>().SetBool("_textwindowflg",false);
        Invoke("DelayChangeWave", 0.5f);
    }
    void DelayChangeWave()
    {
        _textwindow.SetActive(false);
        SceneManager.LoadScene("S3_EndingScene_Nagahama");
        //_blackout.SetActive(true);
    }
}
