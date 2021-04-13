using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private int[] _waveAsteroidcount = new int[4] { 7, 10, 10, 20};

    private Vector3 _spikaPos;
    private Vector3 _earthPos;

    public static Vector3 _instantiatePosition;

    public static bool _instansflg = false;
    public static int _asteroidnum = 5;

    private int _waveAsteroid;
    private int _waveAsteroidInstansCount;
    private bool _asteroidinstansflg = false;

    public GameClearOver_Process _gameCOProcess;

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
        _spikaPos = new Vector3(_spika.transform.position.x, 0, 0);
        _earthPos = new Vector3(_earth.transform.position.x, 0, 0);
        _wave = _wavecount.FAST;

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
        }
        _instansflg = false;
        _instantiatePosition = new Vector3(0, 0, 0);
    }

    void WaveCount()
    {
        switch (_wave)
        {
            case _wavecount.FAST:
                _waveAsteroid = _waveAsteroidcount[0];
                if(_waveAsteroidInstansCount >= _waveAsteroid)  _asteroidinstansflg = true;
                
                if (_gameCOProcess.GetGameClearCount() >= _waveAsteroid)
                {
                    _waveAsteroid = _waveAsteroidcount[1];
                    _waveAsteroidInstansCount = 0;
                    _wave = _wavecount.SECOND;
                    _asteroidinstansflg = false;
                    _gameCOProcess.SetGameClearCount(0);
                }
                
                break;
            case _wavecount.SECOND:
                _waveAsteroid = _waveAsteroidcount[1];
                if (_waveAsteroidInstansCount >= _waveAsteroid) _asteroidinstansflg = true;

                if (_gameCOProcess.GetGameClearCount() >= _waveAsteroid)
                {
                    _waveAsteroid = _waveAsteroidcount[2];
                    _waveAsteroidInstansCount = 0;
                    _wave = _wavecount.THIRD;
                    _asteroidinstansflg = false;
                    _gameCOProcess.SetGameClearCount(0);

                }
                
                break;
            case _wavecount.THIRD:
                _waveAsteroid = _waveAsteroidcount[2];
                if (_waveAsteroidInstansCount >= _waveAsteroid) _asteroidinstansflg = true;

                if (_gameCOProcess.GetGameClearCount() >= _waveAsteroid)
                {
                    _waveAsteroid = _waveAsteroidcount[3];
                    _waveAsteroidInstansCount = 0;
                    _wave = _wavecount.FORTH;
                    _asteroidinstansflg = false;
                    _gameCOProcess.SetGameClearCount(0);
                }
                
                break;
            case _wavecount.FORTH:
                _waveAsteroid = _waveAsteroidcount[3];
                if (_waveAsteroidInstansCount >= _waveAsteroid) _asteroidinstansflg = true;

                if (_gameCOProcess.GetGameClearCount() >= _waveAsteroid)
                {
                    _waveAsteroidInstansCount = 0;
                    _asteroidinstansflg = false;
                }

                break;
        }
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
            + "生成した隕石の数：" + _waveAsteroidInstansCount + "\n" + "隕石生成のフラグ：" + _asteroidinstansflg;
        //Debug.Log("ウエーブ：" + _wave);
        //Debug.Log("ウエーブの隕石：" + _waveAsteroid);
        //Debug.Log("隕石破壊数：" + _gameCOProcess.GetGameClearCount());
        //Debug.Log("生成した隕石の数：" + _waveAsteroidInstansCount);
        //Debug.Log("隕石生成のフラグ：" + _asteroidinstansflg);

    }
}
