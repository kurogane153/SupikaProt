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

    private Text ArertMessage;

    void Start()
    {
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

    void Update()
    {


    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Asteroid")
        {
            Debug.Log("隕石だああああああああああああうわああああああああああああああ");
            DelayMethod();
            ArertText.SetActive(true);
        }
    }

    void DelayMethod()
    {
        if (Spikaflg)
        {
            ArertMessage.text = "スピカに隕石襲来";
        }
        else
        {
            ArertMessage.text = "地球に隕石襲来";
        }
        Invoke("ArertFalse", drawingTime);
    }

    void ArertFalse()
    {
        ArertText.SetActive(false);
    }
}
