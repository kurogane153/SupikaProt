using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoberiaSpawnManager : MonoBehaviour
{
    [Header("Set RoberiaAsteroid Prefab")]
    //隕石プレハブ
    public GameObject _roberiaPrefab;

    [Header("Set Earth Prefab")]
    //隕石が向かう場所の指定
    public GameObject Earth;

    [Header("隕石の速さ")]
    //隕石の速さ
    public float _spawnedAsteroidSpeed = 10f;

    void Start()
    {
        //隕石をインスタンス化する(生成する)
        //GameObject asteroid = Instantiate(_roberiaPrefab);
        //asteroid.transform.position = this.transform.position;
        //asteroid.GetComponent<AsteroidScript>().ChangeParam(_spawnedAsteroidSpeed, Earth.transform.position);
    }

    void Update()
    {
        //if (_roberiaPrefab == null)
        //{
        //    SceneManager.LoadScene("Result");
        //}
    }
}
