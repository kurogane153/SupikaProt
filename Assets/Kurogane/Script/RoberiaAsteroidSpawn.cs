using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoberiaAsteroidSpawn : MonoBehaviour
{
    [Header("Set Asteroid Prefab")]
    //敵プレハブ
    public GameObject enemyPrefab;

    [Header("Set Asteroid Prefab")]
    //敵プレハブ
    public GameObject childPredab;

    [Header("隕石の速さ")]
    //隕石の速さ
    public float _spawnedAsteroidSpeed = 50f;

    [Header("Set Earth Prefab")]
    //隕石が向かう場所の指定
    public GameObject Earth;

    private Vector3 EarthPos;

    public static bool _taegethpflg = false;

    void Start()
    {
        EarthPos = new Vector3(Earth.transform.position.x, 0, 0);
    }

    void Update()
    {
        if (_taegethpflg)
        {
            //enemyをインスタンス化する(生成する)
            GameObject asteroid = Instantiate(enemyPrefab);
            //生成した敵の位置をランダムに設定する
            asteroid.transform.position = childPredab.transform.position;
            _taegethpflg = false;
            asteroid.GetComponent<AsteroidScript>().ChangeParam(_spawnedAsteroidSpeed, EarthPos);
        }
    }
}
