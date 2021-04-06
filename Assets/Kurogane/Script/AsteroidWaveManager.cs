using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private Vector3 _spikaPos;
    private Vector3 _earthPos;
    public static Vector3 _instantiatePosition;
    public static bool _instansflg = false;
    public static int _asteroidnum = 5;
    // Start is called before the first frame update
    void Start()
    {
        _spikaPos = new Vector3(_spika.transform.position.x, 0, 0);
        _earthPos = new Vector3(_earth.transform.position.x, 0, 0);
    }
    // Update is called once per frame
    void Update()
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
}
