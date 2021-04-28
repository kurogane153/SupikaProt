using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoberiaAsteroidLasCollider : MonoBehaviour
{
    [Header("Set RoberiaCollider Prefab")]
    //隕石プレハブ
    public GameObject _roberiacolliderPrefab;

    public static bool _lastshotflg;

    void Start()
    {
        _lastshotflg = false;
        _roberiacolliderPrefab.SetActive(false);
    }
    void Update()
    {
        if(this.gameObject.GetComponent<AsteroidScript>().GetAsteroidHp() <= 50)
        {
            _roberiacolliderPrefab.SetActive(true);
        }
        if (_roberiacolliderPrefab != null && _lastshotflg)
        {
            Debug.Log("ラストやで");
        }
    }
}
