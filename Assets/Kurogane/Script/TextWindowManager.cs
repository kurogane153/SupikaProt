using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextWindowManager : MonoBehaviour
{

    [SerializeField] private SoundPlayer _audioSource;
    [SerializeField] private AudioClip _se_WindowOpen;

    //TextMesh Proのテキスト、Inspectorで設定
    [Header("テキストウィンドウ")]
    [SerializeField]
    private GameObject _textwindow;

    [SerializeField] private TextMeshProUGUI _messageText;

    //表示するテキスト
    [SerializeField]
    [TextArea(1, 6)]
    public string[] _text;

    private Animator textWindowAnimator;

    void Start()
    {
        _textwindow.SetActive(false);
        textWindowAnimator = _textwindow.GetComponent<Animator>();
    }

    //テキストウィンドウの文字表示・アニメーションのON
    public void TextWindowOn(int textNo)
    {
        _messageText.text = _text[textNo];
        _textwindow.SetActive(true);
        textWindowAnimator.SetBool("_textwindowflg", true);
        _audioSource.PlaySE(_se_WindowOpen);
    }

    //テキストウィンドウのアニメーションOFF
    public void TextWindowOff()
    {
        textWindowAnimator.SetBool("_textwindowflg", false);
        Invoke("DelayAnimation", 0.5f);
    }

    private void DelayAnimation()
    {
        _textwindow.SetActive(false);
    }
}
