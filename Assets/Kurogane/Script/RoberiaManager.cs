﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class RoberiaManager : MonoBehaviour
{
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

    [Header("ロベリアの速さ")]
    //隕石の速さ
    public float _spawnedAsteroidSpeed = 20f;

    [Header("最後のテキストの表示秒数")]
    public int drawingTime = 3;

    [Header("ロベリアのラストコライダーを表示させるHP")]
    public int _roberiahp = 180;

    [Header("ロベリア破壊シーンに飛ばすHP最低値")]
    public int _roberialasthp = 30;

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

    private bool _lastshotflg;
    private Vector3 _earthPos;

    void Start()
    {
        _lastshotflg = false;
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
        if (_roberiaPrefab.gameObject.GetComponent<AsteroidScript>().GetAsteroidHp() <= _roberiahp)
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
            }
            
        }

        if (_roberiaPrefab.gameObject.GetComponent<AsteroidScript>().GetAsteroidHp() <= _roberialasthp)
        {
            //_lastshotflg = true;
        }
    }
}
