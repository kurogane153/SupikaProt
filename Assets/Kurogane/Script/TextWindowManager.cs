using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextWindowManager : MonoBehaviour
{

    //TextMesh Proのテキスト、Inspectorで設定
    [Header("テキストウィンドウ")]
    [SerializeField]
    private GameObject _textwindow;

    //表示するテキスト
    [SerializeField]
    [TextArea(1, 3)]
    public string[] _text;

    private GameObject _textmessage;

    void Start()
    {
        _textmessage = _textwindow.transform.GetChild(0).gameObject;
        _textwindow.SetActive(false);
    }

    //テキストウィンドウの文字表示・アニメーションのON
    public void TextWindowOn(int textNo)
    {
        _textmessage.GetComponent<TextMeshProUGUI>().text = _text[textNo];
        _textwindow.SetActive(true);
        _textwindow.GetComponent<Animator>().SetBool("_textwindowflg", true);
    }

    //テキストウィンドウのアニメーションOFF
    public void TextWindowOff()
    {
        _textwindow.GetComponent<Animator>().SetBool("_textwindowflg", false);
        Invoke("DelayAnimation", 0.5f);
    }

    private void DelayAnimation()
    {
        _textwindow.SetActive(false);
    }
}
