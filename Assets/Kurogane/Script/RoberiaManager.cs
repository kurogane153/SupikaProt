using System.Collections;
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

    [Header("ロベリアの速さ")]
    //隕石の速さ
    public float _spawnedAsteroidSpeed = 20f;

    [Header("TextWindowManager")]
    [SerializeField]
    public TextWindowManager _textmanager;

    [Header("最後に表示するテキストナンバー")]
    [SerializeField]
    private int _textNo = 4;

    private bool _lastshotflg;
    private Vector3 _earthPos;

    //public GameObject _blackout;

    void Start()
    {
        _lastshotflg = false;
        _earthPos = new Vector3(_earth.transform.position.x, 0, 0);
        _roberiaPrefab.GetComponent<AsteroidScript>().ChangeParam(_spawnedAsteroidSpeed, _earthPos);
        _roberiacolliderPrefab.SetActive(false);
    }

    void Update()
    {
        if (_lastshotflg)
        {
            _textmanager.TextWindowOn(_textNo);
            Invoke("DelayChangeTextWindow", 3f);
        }
        HpColliderOn();

    }

    void DelayChangeTextWindow()
    {
        _textmanager.TextWindowOff();
        Invoke("DelayChangeWave", 1f);
    }
    void DelayChangeWave()
    {
        SceneManager.LoadScene("S3_EndingScene_Nagahama");
    }

    void HpColliderOn()
    {
        if (_roberiaPrefab.gameObject.GetComponent<AsteroidScript>().GetAsteroidHp() <= 120)
        {
            _roberiacolliderPrefab.SetActive(true);
            _roberiaPrefab.GetComponent<AsteroidScript>().ChangeParam(_spawnedAsteroidSpeed * 2, _earthPos);
        }

        if (_roberiaPrefab.gameObject.GetComponent<AsteroidScript>().GetAsteroidHp() <= 20)
        {
            _lastshotflg = true;
        }
    }
}
