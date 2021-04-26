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
    public GameObject _spawnchildPredab;

    [Header("Set Asteroid Prefab")]
    //敵プレハブ
    public GameObject _pointchildPredab;

    [Header("隕石の速さ")]
    //隕石の速さ
    public float _spawnedAsteroidSpeed = 50f;

    [Header("Set Earth Prefab")]
    //隕石が向かう場所の指定
    public GameObject Earth;

    public static bool _taegethpflg = false;

    void Update()
    {
        if (this.gameObject.GetComponent<TargetCollider>().IsHp)
        {
            //enemyをインスタンス化する(生成する)
            GameObject asteroid = Instantiate(enemyPrefab);
            asteroid.transform.position = _spawnchildPredab.transform.position;
            asteroid.GetComponent<AsteroidScript>().ChangeParam(400, _pointchildPredab.transform.position);
            asteroid.GetComponent<AsteroidScript>().ChangeRotation(true);
            this.gameObject.GetComponent<TargetCollider>().IsHp = false;
        }
    }
}
