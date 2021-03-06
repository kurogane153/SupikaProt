﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class RoberiaManager : MonoBehaviour
{

    #region Singleton
    private static RoberiaManager instance;

    public static RoberiaManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (RoberiaManager)FindObjectOfType(typeof(RoberiaManager));

                if (instance == null)
                {
                    Debug.LogError(typeof(RoberiaManager) + "is nothing");
                }
            }

            return instance;
        }
    }
    #endregion

    [Header("Set RoberiaAsteroid Prefab")]
    //隕石プレハブ
    public GameObject _roberiaPrefab;

    [Header("Set RoberiaCollider Prefab")]
    //隕石プレハブ
    public GameObject _roberiacolliderPrefab;

    [Header("Set Earth Prefab")]
    //隕石が向かう場所の指定
    public GameObject _earth;

    [Header("Set SpikaRendering Prefab")]
    public GameObject _spikarendering;

    [Header("Set EarthRendering Prefab")]
    public GameObject _earthrendering;

    [SerializeField, Header("ロベリアのエフェクト")] private ParticleSystem[] _lastBossParticles;

    [Header("ロベリアの速さ")]
    //隕石の速さ
    public float _spawnedAsteroidSpeed = 20f;

    [Header("最後のテキストの表示秒数")]
    public int drawingTime = 3;

    [Header("ロベリアのラストコライダーを表示させるHP")]
    public int _roberiahp = 180;

    [Header("ロベリアが生成する隕石の数（extralifeの数）")]
    public int _roberiainstansasteroidcount = 10;

    [Header("ゲームオーバー・ゲームクリアのリロード時間")]
    public float GameOverReloadTime = 3f;

    [Header("LetterBox")]
    public GameObject _letterbox;

    [Header("RoberiaHpba")]
    public GameObject _hpbar;

    [Header("Sppedbar")]
    public GameObject _speedbar;

    [Header("TextWindowManager")]
    [SerializeField]
    public TextWindowManager _textmanager;

    [Header("最後に表示するテキストナンバー")]
    [SerializeField]
    private int _textNo = 4;

    //デバッグ用
    public GameObject _waveText = null; // Textオブジェクト

    private bool _lastshotflg;
    private bool _notasteroidcountflg;
    private int _roberiaasteroidcount = 0;
    private Vector3 _earthPos;

    void Start()
    {
        _lastshotflg = false;
        _notasteroidcountflg = false;
        _earthPos = new Vector3(_earth.transform.position.x, 0, 0);
        _roberiaPrefab.GetComponent<AsteroidScript>().ChangeParam(_spawnedAsteroidSpeed, _earthPos);
        _roberiacolliderPrefab.SetActive(false);
    }

    void Update()
    {
        GameOverSceneChange();
        if (_roberiaPrefab) 
        {
            HpColliderOn();
        }
        Dbg();

    }

    void DelayMethod()
    {
        GameClearOverManager._gameoverCount = 0;
        GameClearOverManager._gameoverCountColony = 0;
        SceneManager.LoadScene("Result");
    }

    void DelayChangeTextWindow()
    {
        _textmanager.TextWindowOff();
        //_letterbox.GetComponent<Animator>().SetBool("LetterKey", false);
        Invoke("DelayChangeWave", drawingTime - (drawingTime - 1));
    }
    void DelayChangeWave()
    {
        _letterbox.GetComponent<Animator>().SetTrigger("LetterTriger");
        GameClearOverManager._gameoverCount = 0;
        GameClearOverManager._gameoverCountColony = 0;
    }

    void GameOverSceneChange()
    {
        if (_earthrendering.GetComponent<GameClearOver_Process>().GetGameOverCount()
            >= _earthrendering.GetComponent<GameClearOver_Process>().GetGameOverAsteroidCount())
        {
            Invoke("DelayMethod", GameOverReloadTime);
        }
        if (_spikarendering.GetComponent<GameClearOver_Process>().GetGameOverCount()
            >= _spikarendering.GetComponent<GameClearOver_Process>().GetGameOverAsteroidCount())
        {
            Invoke("DelayMethod", GameOverReloadTime);
        }
    }

    void HpColliderOn()
    {
        if (_roberiaPrefab.gameObject.GetComponent<AsteroidScript>().GetAsteroidHp() <= _roberiahp
            && _roberiainstansasteroidcount == _roberiaasteroidcount)
        {
            if (!_lastshotflg) {
                _lastshotflg = true;
                _roberiacolliderPrefab.SetActive(true);
                _roberiacolliderPrefab.GetComponent<TargetCollider>().IsFinalExcutionTgt = true;

                _roberiaPrefab.GetComponent<AsteroidScript>().ChangeParam(_spawnedAsteroidSpeed * 2, _earthPos);

                _textmanager.TextWindowOn(_textNo);
                _letterbox.GetComponent<Animator>().SetBool("LetterKey", true);
                _hpbar.SetActive(false);
                _speedbar.SetActive(false);
                Invoke("DelayChangeTextWindow", drawingTime);
                // ロベリアのエフェクトをストップ
                foreach (var par in _lastBossParticles) {
                    var main = par.main;
                    main.loop = false;
                    par.Stop();
                }
            }
            
        }

        if (_roberiaPrefab.gameObject.GetComponent<AsteroidScript>().GetAsteroidHp() <= _roberiahp
            && _roberiainstansasteroidcount > _roberiaasteroidcount)
        {
            if (!_notasteroidcountflg)
            {
                _notasteroidcountflg = true;
                _textmanager.TextWindowOn(11);
                Invoke("TextClose", drawingTime);
            }

            // ロベリアのエフェクトをストップ
            foreach (var par in _lastBossParticles) {
                var main = par.main;
                main.loop = false;
                par.Stop();
            }
        }
    }

    void TextClose()
    {
        _textmanager.TextWindowOff();
    }

    public void RoberiaAsteroidDestroyCount(int count)
    {
        _roberiaasteroidcount += count;
    }

    void Dbg()
    {
        _waveText.GetComponent<Text>().text = "破壊数：" + _roberiaasteroidcount + "\n";

    }
}
