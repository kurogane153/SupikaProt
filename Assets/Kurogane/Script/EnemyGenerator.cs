﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyGenerator : MonoBehaviour
{

    [Header("Set Asteroid Prefab")]
    //敵プレハブ
    public GameObject enemyPrefab;

    [Header("Set Spika Prefab")]
    //隕石が向かう場所の指定
    public GameObject Spika;

    [Header("Set Earth Prefab")]
    //隕石が向かう場所の指定
    public GameObject Earth;

    [Header("隕石をスピカに向かわせるならONに")]
    public bool Spikaflg;

    [Header("Set Interval Min and Max")]
    //時間間隔の最小値
    [Range(1f, 10f)]
    public float minTime = 2f;
    //時間間隔の最大値
    [Range(10f, 30f)]
    public float maxTime = 5f;
    
    [Header("隕石の速さ")]
    //隕石の速さ
    public float _spawnedAsteroidSpeed = 50f;
    
    [Header("X座標の最大値と最小値")]
    //X座標の最小値
    [Range(-500f, 0f)]
    public float xMinPosition = -10f;
    //X座標の最大値
    [Range(0f, 500f)]
    public float xMaxPosition = 10f;

    [Header("Y座標の最大値と最小値")]
    //Y座標の最小値
    [Range(-500f, 0f)]
    public float yMinPosition = 0f;
    //Y座標の最大値
    [Range(0f, 500f)]
    public float yMaxPosition = 10f;

    [Header("Z座標の最大値と最小値")]
    //Z座標の最小値
    [Range(-500f, 0f)]
    public float zMinPosition = 0f;
    //Z座標の最大値
    [Range(0f, 500f)]
    public float zMaxPosition = 0f;

    public AsteroidWaveManager _asteroidwavemanager;

    private int _asteroidInstansCount;

    //敵生成時間間隔
    private float interval;
    //経過時間
    private float time = 0f;

    //カメラに表示されているか
    private bool isRendered = false;

    private Vector3 SpikaPos;

    private Vector3 EarthPos;

    //メインカメラに付いているタグ名
    private const string MAIN_CAMERA_TAG_NAME = "MainCamera";

    void Start()
    {
        //時間間隔を決定する
        interval = GetRandomTime();
        SpikaPos = new Vector3(Spika.transform.position.x, 0, 0);
        EarthPos = new Vector3(Earth.transform.position.x, 0, 0);
    }

    void Update()
    {
        //時間計測
        time += Time.deltaTime;

        AsteroidInstens();
    }

    void AsteroidInstens()
    {
        //経過時間が生成時間になったとき(生成時間より大きくなったとき)
        if (time > interval)
        {
            if (isRendered)
            {
                if (!_asteroidwavemanager.GetAsteroidInstansFlg())
                {
                    //enemyをインスタンス化する(生成する)
                    GameObject asteroid = Instantiate(enemyPrefab);
                    //生成した敵の位置をランダムに設定する
                    asteroid.transform.position = GetRandomPosition();

                    if (Spikaflg)
                    {
                        asteroid.GetComponent<AsteroidScript>().ChangeParam(_spawnedAsteroidSpeed, SpikaPos);
                    }
                    else
                    {
                        asteroid.GetComponent<AsteroidScript>().ChangeParam(_spawnedAsteroidSpeed, EarthPos);
                    }
                    //経過時間を初期化して再度時間計測を始める
                    time = 0f;
                    //次に発生する時間間隔を決定する
                    interval = GetRandomTime();
                    isRendered = false;
                    _asteroidwavemanager.SetWaveAsteroidCount(1);
                }
                else
                {
                    interval = GetRandomTime();
                    isRendered = false;
                    time = 0f;
                }
            }
        }
    }

    //ランダムな時間を生成する関数
    private float GetRandomTime()
    {
        return Random.Range(minTime, maxTime);
    }

    //ランダムな位置を生成する関数
    private Vector3 GetRandomPosition()
    {
        //それぞれの座標をランダムに生成する
        float x = Random.Range(xMinPosition + this.transform.position.x, xMaxPosition + this.transform.position.x);
        float y = Random.Range(yMinPosition + this.transform.position.y, yMaxPosition + this.transform.position.y);
        float z = Random.Range(zMinPosition + this.transform.position.z, zMaxPosition + this.transform.position.z);

        //Vector3型のPositionを返す
        return new Vector3(x, y, z);
    }

    private void OnWillRenderObject()
    {
        if (Camera.current.tag == MAIN_CAMERA_TAG_NAME)
        {
            isRendered = true;
        }
    }



}
