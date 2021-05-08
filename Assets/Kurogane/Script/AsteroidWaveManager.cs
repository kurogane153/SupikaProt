using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AsteroidWaveManager : MonoBehaviour
{

    [Header("Set Asteroid Prefab")]
    //敵プレハブ
    public GameObject _enemyPrefab;

    [Header("Set Divide Into Two Asteroid Prefab")]
    //二つに分ける隕石
    public GameObject _selectPrefab;

    [Header("Set Spika Prefab")]
    //隕石が向かう場所の指定
    public GameObject _spika;

    [Header("Set Earth Prefab")]
    //隕石が向かう場所の指定
    public GameObject _earth;

    [Header("隕石をスピカに向かわせるならONに")]
    public bool _spikaflg;

    [Header("隕石の速さ")]
    //隕石の速さ
    public float _spawnedAsteroidSpeed = 50f;

    [Header("テキストの表示秒数（ウェーブ事の遅延秒数）")]
    public int drawingTime = 3;

    [Header("ウェーブ事の隕石の個数")]
    [SerializeField]
    private int[] _waveAsteroidcount = new int[4] { 7, 10, 15, 25 };

    [Header("表示するテキストナンバー")]
    [SerializeField]
    public int[] _textNo;

    [Header("TextWindowManager")]
    [SerializeField]
    public TextWindowManager _textmanager;

    [Header("LetterBox")]
    //隕石が向かう場所の指定
    public GameObject _letterbox;

    private Vector3 _spikaPos;
    private Vector3 _earthPos;

    public static Vector3 _instantiatePosition;

    public static bool _instansflg = false;
    public static int _asteroidnum = 5;

    //一つのスポーンに対してのスポーン数
    private int _asteroidwavecount;

    //ウェーブ事の隕石
    private int _waveAsteroid;

    //生成した隕石の数
    private int _waveAsteroidInstansCount;

    //生成するかどうかのフラグ
    private bool _asteroidinstansflg = false;

    //private GameObject _textmessage;

    public GameClearOver_Process _gameCOProcess;

    private int _textno;
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
        _textno = 1;
        _spikaPos = new Vector3(_spika.transform.position.x, 0, 0);
        _earthPos = new Vector3(_earth.transform.position.x, 0, 0);
        _wave = _wavecount.FAST;
        _asteroidwavecount = 2;
        _waveAsteroid = _waveAsteroidcount[0];
        _asteroidinstansflg = true;
        _gameCOProcess.SetGameClearCount(0);
        FastText(0);
    }

    void Update()
    {
        WaveCount();
        DoubleAsteroid();
        Dbg();
    }

    void DoubleAsteroid()
    {
        if (_instansflg)
        {
            GameObject asteroid = Instantiate(_enemyPrefab);
            GameObject asteroid2 = Instantiate(_enemyPrefab);
            asteroid.transform.position = _instantiatePosition;
            asteroid2.transform.position = new Vector3(_instantiatePosition.x + 300, _instantiatePosition.y + 200, _instantiatePosition.z);
            if (_spikaflg)
            {
                asteroid.GetComponent<AsteroidScript>().ChangeParam(_spawnedAsteroidSpeed, _spikaPos);
                asteroid2.GetComponent<AsteroidScript>().ChangeParam(_spawnedAsteroidSpeed, _spikaPos);
            }
            else
            {
                asteroid.GetComponent<AsteroidScript>().ChangeParam(_spawnedAsteroidSpeed, _earthPos);
                asteroid2.GetComponent<AsteroidScript>().ChangeParam(_spawnedAsteroidSpeed, _earthPos);
            }
            SetWaveAsteroidCount(2);
        }
        _instansflg = false;
        _instantiatePosition = new Vector3(0, 0, 0);
    }

    void WaveCount()
    {
        switch (_wave)
        {
            case _wavecount.FAST:
                if (_waveAsteroidInstansCount >= _waveAsteroid)  _asteroidinstansflg = true;
                
                if (_gameCOProcess.GetGameClearCount() >= _waveAsteroid)
                {
                    _asteroidwavecount++;
                    _wave = _wavecount.SECOND;
                    _waveAsteroid = _waveAsteroidcount[1];
                    _textmanager.TextWindowOn((int)_wavecount.FAST + 1);
                    Invoke("DelayChangeWave", drawingTime);
                }

                break;
            case _wavecount.SECOND:
                if (_waveAsteroidInstansCount >= _waveAsteroid) _asteroidinstansflg = true;

                if (_gameCOProcess.GetGameClearCount() >= _waveAsteroid)
                {
                    _asteroidwavecount++;
                    _wave = _wavecount.THIRD;
                    _waveAsteroid = _waveAsteroidcount[2];
                    _textmanager.TextWindowOn((int)_wavecount.SECOND + 1);
                    Invoke("DelayChangeWave", drawingTime);
                }
                
                break;
            case _wavecount.THIRD:
                if (_waveAsteroidInstansCount >= _waveAsteroid) _asteroidinstansflg = true;

                if (_gameCOProcess.GetGameClearCount() >= _waveAsteroid)
                {
                    _asteroidwavecount++;
                    _wave = _wavecount.FORTH;
                    _waveAsteroid = _waveAsteroidcount[3];
                    _textmanager.TextWindowOn((int)_wavecount.THIRD + 1);
                    Invoke("DelayChangeWave", drawingTime);
                }
                
                break;
            case _wavecount.FORTH:
                if (_waveAsteroidInstansCount >= _waveAsteroid) _asteroidinstansflg = true;  
                if (_gameCOProcess.GetGameClearCount() >= _waveAsteroid)
                {
                    if (!_textflg) {
                        LastText();
                    }
                    _asteroidinstansflg = true;
                }

                break;
        }

    }

    void FastText(int textno)
    {
        _textmanager.TextWindowOn(textno);
        _letterbox.GetComponent<Animator>().SetBool("LetterKey", true);
        Invoke("TextClose", drawingTime);
    }

    void TextClose()
    {
        _textmanager.TextWindowOn(_textno);
        Invoke("DelayChangeWave", drawingTime);
    }

    void LastText()
    {
        _letterbox.GetComponent<Animator>().SetTrigger("LetterTriger");
        _textflg = true;
        _textmanager.TextWindowOn((int)_wavecount.FORTH + 1);
        _letterbox.GetComponent<Animator>().SetBool("LetterKey", true);
        Invoke("LastTextMes", drawingTime);
    }

    void LastTextMes()
    {
        _textmanager.TextWindowOn((int)_wavecount.FORTH + 2);
        Invoke("LastTextClose", drawingTime);
    }

    void LastTextClose()
    {
        _letterbox.GetComponent<Animator>().SetBool("LetterKey", false);
        _waveAsteroidInstansCount = 0;
        _gameCOProcess.SetGameClearCount(0);
        _textmanager.TextWindowOff();
    }

    private void DelayChangeWave()
    {
        _letterbox.GetComponent<Animator>().SetBool("LetterKey", false);
        _asteroidinstansflg = false;
        _waveAsteroidInstansCount = 0;
        _gameCOProcess.SetGameClearCount(0);
        _textmanager.TextWindowOff();
    }

    public int GetWaveAsteroidInstansCount()
    {
        return _waveAsteroidInstansCount;
    }

    public int GetAsteroidWaveCount()
    {
        return _asteroidwavecount;
    }

    public int GetWaveCount()
    {
        return (int)_wave;
    }

    public int GetWaveAsteroid()
    {
        return _waveAsteroid;
    }

    public void SetWaveAsteroidCount(int asteroidcount)
    {
        _waveAsteroidInstansCount += asteroidcount;
    }

    public bool GetAsteroidInstansFlg()
    {
        return _asteroidinstansflg;
    }

    void Dbg()
    {
        _waveText.GetComponent<Text>().text = "ウエーブ：" + (int)_wave + "\n" 
            + "ウエーブの隕石：" +_waveAsteroid + "\n" +"隕石破壊数：" + _gameCOProcess.GetGameClearCount() + "\n"
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
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _asteroidwavecount++;
            _wave = _wavecount.FORTH;
            _textflg = false;
            _waveAsteroid = _waveAsteroidcount[3];
        }
        //Debug.Log("ウエーブ：" + _wave);
        //Debug.Log("ウエーブの隕石：" + _waveAsteroid);
        //Debug.Log("隕石破壊数：" + _gameCOProcess.GetGameClearCount());
        //Debug.Log("生成した隕石の数：" + _waveAsteroidInstansCount);
        //Debug.Log("隕石生成のフラグ：" + _asteroidinstansflg);

    }
}
