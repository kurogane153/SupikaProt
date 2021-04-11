using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertLine : MonoBehaviour
{

    [Header("Set Spika Prefab")]
    //アラートの場所
    public GameObject Spika;

    [Header("Set Earth Prefab")]
    //アラートの場所
    public GameObject Earth;

    [Header("スピカのアラートエリアならON")]
    public bool Spikaflg;

    [Header("ゲームオーバーまでの隕石の個数")]
    public int drawingTime = 3;

    [Header("ArertTextObject")]
    public GameObject ArertText = null; // Textオブジェクト
    public static bool _alertflg;

    private GameObject _asteroid;

    private Text ArertMessage;

    void Start()
    {

        _alertflg = false;
        if (Spikaflg)
        {
            this.transform.position = Spika.transform.position;
        }
        else
        {
            this.transform.position = Earth.transform.position;
        }
        ArertMessage = ArertText.GetComponentInChildren<Text>();

    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Asteroid")
        {
            DelayMethod();
            ArertText.SetActive(true);
            _asteroid = collision.gameObject;
        }
    }

    void DelayMethod()
    {
        _alertflg = true;
        if (Spikaflg)
        {
            ArertMessage.text = "コロニーに隕石接近";
        }
        else
        {
            ArertMessage.text = "地球に隕石接近";
        }
        Invoke("ArertFalse", drawingTime);
    }

    void ArertFalse()
    {
        _alertflg = false;
        ArertText.SetActive(false);
    }

    public Vector3 GetAsteroid()
    {
        return _asteroid.transform.position;
    }

    public bool GetArertFlg()
    {
        return _alertflg;
    }
}
