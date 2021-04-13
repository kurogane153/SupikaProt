using UnityEngine;
using UnityEngine.UI;

public class AlertFlush : MonoBehaviour
{

    [Header("Set Alert Prefab")]
    //アラートの場所
    public GameObject _alert;

    [Header("Set Alert Prefab")]
    //アラートの場所
    public GameObject _alertspika;

    Image img;
    Animator anim;

    void Start()
    {
        img = GetComponent<Image>();
        anim = GetComponent<Animator>();
        img.color = Color.clear;
        anim.SetBool("AretKey", false);
    }

    void Update()
    {
        if (_alert.GetComponent<AlertLine>().GetArertFlg() || _alertspika.GetComponent<AlertLine>().GetArertFlg())
        {
            OnAlert();
        }
        else
        {
            OffAlert();
        }
    }

    public void OnAlert()
    {
        anim.SetBool("AretKey", true);
    }
    public void OffAlert()
    {
        anim.SetBool("AretKey", false);
    }
}