using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{

    [Header("TextWindowManager")]
    [SerializeField]
    public TextWindowManager _textmanager;

    [Header("テキストの表示秒数（ウェーブ事の遅延秒数）")]
    public int drawingTime = 3;

    [Header("LetterBox")]
    public GameObject _letterbox;

    [Header("ウェーブ事の隕石の個数")]
    [SerializeField]
    private int _waveAsteroidtutorialcount = 5;

    [Header("ウェーブ事の隕石の個数")]
    [SerializeField]
    private int[] _waveAsteroidcount = new int[3] { 3, 5, 6};

    public GameClearOver_Process _gameCOProcess;

    //一つのスポーンに対してのスポーン数
    private int _asteroidwavecount;

    //ウェーブ事の隕石
    private int _waveAsteroid;

    //生成した隕石の数
    private int _waveAsteroidInstansCount;

    //生成するかどうかのフラグ
    private bool _asteroidinstansflg;

    //ラグランジュポイント接近時のテキスト用
    public GameObject _orbitshift;
    public GameObject _textwindow;
    private bool _fastorbitshifttextflg = false;

    private bool _textflg = false;

    //デバッグ用
    public GameObject _waveText = null; // Textオブジェクト

    enum _wavecount
    {
        FAST = 1,
        SECOND,
        THIRD,
        FORTH
    };

    private _wavecount _wave;

    void Start()
    {
        _asteroidinstansflg = true;
        _wave = _wavecount.FAST;
        _asteroidwavecount = 1;
        _waveAsteroid = _waveAsteroidcount[0];
        FastText(12);
    }

    void Update()
    {
        WaveCount();
        OrbitShiftText();
        Dbg();
    }

    void WaveCount()
    {
        switch (_wave)
        {
            case _wavecount.FAST:
                if (_waveAsteroidInstansCount >= _waveAsteroid) _asteroidinstansflg = true;

                if (_gameCOProcess.GetGameClearCount() >= _waveAsteroid)
                {
                    _wave = _wavecount.SECOND;
                    _textmanager.TextWindowOn(14);
                    _waveAsteroid = _waveAsteroidcount[(int)_wavecount.FAST];
                    Invoke("DelayChangeWave", drawingTime);
                }

                break;
            case _wavecount.SECOND:
                if (_waveAsteroidInstansCount >= _waveAsteroid) _asteroidinstansflg = true;

                if (_gameCOProcess.GetGameClearCount() >= _waveAsteroid)
                {
                    _wave = _wavecount.THIRD;
                    _textmanager.TextWindowOn(16);
                    _waveAsteroid = _waveAsteroidcount[(int)_wavecount.SECOND];
                    Invoke("DelayChangeWave", drawingTime);
                }

                break;

            case _wavecount.THIRD:
                if (_waveAsteroidInstansCount >= _waveAsteroid) _asteroidinstansflg = true;

                if (_gameCOProcess.GetGameClearCount() >= _waveAsteroid)
                {

                    if (!_textflg)
                    {
                        lastText();
                    }
                    _asteroidinstansflg = true;
                }

                break;
        }
    }

    void OrbitShiftText()
    {
        if (_orbitshift.activeSelf)
        {
            if (!_fastorbitshifttextflg)
            {
                if (!_textwindow.activeSelf)
                {
                    _textmanager.TextWindowOn(9);
                    _fastorbitshifttextflg = true;
                    Invoke("TextClose", drawingTime);
                }
            }
        }
    }  

    private void DelayChangeWave()
    {
        _letterbox.GetComponent<Animator>().SetBool("LetterKey", false);
        _asteroidinstansflg = false;
        _waveAsteroidInstansCount = 0;
        _gameCOProcess.SetGameClearCount(0);
        _textmanager.TextWindowOff();
    }

    void TextClose()
    {
        _textmanager.TextWindowOff();
    }

    void FastText(int textno)
    {
        _textmanager.TextWindowOn(textno);
        _letterbox.GetComponent<Animator>().SetBool("LetterKey", true);
        Invoke("SecondText", drawingTime);
    }

    void SecondText()
    {
        _textmanager.TextWindowOn(13);
        Invoke("DelayChangeWave", drawingTime);
    }

    void lastText()
    {
        _textflg = true;
        _textmanager.TextWindowOn(17);
        _letterbox.GetComponent<Animator>().SetBool("LetterKey", true);
        Invoke("LastSecondText", drawingTime);
    }

    void LastSecondText()
    {
        _textmanager.TextWindowOn(18);
        Invoke("LastThirdText", drawingTime);
    }

    void LastThirdText()
    {
        _textmanager.TextWindowOn(19);
        Invoke("SceneChange", drawingTime);
        //OpeningScene
    }

    void SceneChange()
    {
        SceneManager.LoadScene("OpeningScene");
    }

    public int GetAsteroidWaveCount()
    {
        return _asteroidwavecount;
    }

    public bool GetAsteroidInstansFlg()
    {
        return _asteroidinstansflg;
    }

    public void SetWaveAsteroidCount(int asteroidcount)
    {
        _waveAsteroidInstansCount += asteroidcount;
    }

    public int GetWaveCount()
    {
        return (int)_wave;
    }

    public int GetWaveAsteroid()
    {
        return _waveAsteroid;
    }

    public int GetWaveAsteroidInstansCount()
    {
        return _waveAsteroidInstansCount;
    }

    #region デバッグ
    void Dbg()
    {
        _waveText.GetComponent<Text>().text = "ウエーブ：" + (int)_wave + "\n"
            + "ウエーブの隕石：" + _waveAsteroid + "\n" + "隕石破壊数：" + _gameCOProcess.GetGameClearCount() + "\n"
            + "生成した隕石の数：" + _waveAsteroidInstansCount + "\n" + "隕石生成のフラグ：" + _asteroidinstansflg +
            "\n" + "１スポナー事の生成数：" + _asteroidwavecount;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _wave = _wavecount.FAST;
            _asteroidwavecount = 2;
            _waveAsteroid = _waveAsteroidcount[0];
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _asteroidwavecount++;
            _wave = _wavecount.SECOND;
            _waveAsteroid = _waveAsteroidcount[1];
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _asteroidwavecount++;
            _wave = _wavecount.THIRD;
            _waveAsteroid = _waveAsteroidcount[2];
        }
        //Debug.Log("ウエーブ：" + _wave);
        //Debug.Log("ウエーブの隕石：" + _waveAsteroid);
        //Debug.Log("隕石破壊数：" + _gameCOProcess.GetGameClearCount());
        //Debug.Log("生成した隕石の数：" + _waveAsteroidInstansCount);
        //Debug.Log("隕石生成のフラグ：" + _asteroidinstansflg);

    }
    #endregion

}
