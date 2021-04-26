using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoberiaSpawnAsteroidPointChange : MonoBehaviour
{

    [Header("Set Earth Prefab")]
    //隕石が向かう場所の指定
    public GameObject Earth;

    [Header("隕石の速さ")]
    //隕石の速さ
    public float _spawnedAsteroidSpeed = 50f;

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Asteroid")
        {
            collision.GetComponent<AsteroidScript>().ChangeParam(_spawnedAsteroidSpeed, Earth.transform.position);
            collision.GetComponent<AsteroidScript>().ChangeRotation(false);
        }
    }
}
