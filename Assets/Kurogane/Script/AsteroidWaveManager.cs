using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidWaveManager : MonoBehaviour
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

    [Header("隕石の速さ")]
    //隕石の速さ
    public float _spawnedAsteroidSpeed = 50f;

    private Vector3 SpikaPos;

    private Vector3 EarthPos;

    public static Transform InstnsAsteroid;
    public static Vector3 InstantiatePosition;

    public static bool Instansflg = false;

    // Start is called before the first frame update
    void Start()
    {
        SpikaPos = new Vector3(Spika.transform.position.x, 0, 0);
        EarthPos = new Vector3(Earth.transform.position.x, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {

        if (Instansflg)
        {
            GameObject asteroid = Instantiate(enemyPrefab);
            GameObject asteroid2 = Instantiate(enemyPrefab);
            asteroid.transform.position = InstantiatePosition;
            asteroid2.transform.position = new Vector3(InstantiatePosition.x + 300, InstantiatePosition.y + 200 , InstantiatePosition.z);

            if (Spikaflg)
            {
                asteroid.GetComponent<AsteroidScript>().ChangeParam(_spawnedAsteroidSpeed, SpikaPos);
                asteroid2.GetComponent<AsteroidScript>().ChangeParam(_spawnedAsteroidSpeed, SpikaPos);
            }
            else
            {
                asteroid.GetComponent<AsteroidScript>().ChangeParam(_spawnedAsteroidSpeed, EarthPos);
                asteroid2.GetComponent<AsteroidScript>().ChangeParam(_spawnedAsteroidSpeed, EarthPos);
            }
        }

        Instansflg = false;
        InstantiatePosition = new Vector3(0, 0, 0);

    }
}
