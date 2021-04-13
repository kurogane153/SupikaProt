using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertCursorAngle : MonoBehaviour
{

    //// 中心点
    //[SerializeField] private Vector3 _center = Vector3.zero;
    //// 回転軸
    //[SerializeField] private Vector3 _axis = Vector3.up;
    //// 円運動周期
    //[SerializeField] private float _period = 2;
    //private Quaternion pos = new Quaternion(0, 0, 0, 0);
    //private GameObject _player;

    private Vector3 _asteroidposition = new Vector3();
    private Vector3 diff = Vector3.zero;
    private GameObject _alert;
    private GameObject _alertSpika;

    void Start()
    {
        _alert = GameObject.Find("AlertLine");
        _alertSpika = GameObject.Find("AlertLineSpika");
        SetAlertCursor();
    }

    void Update()
    {
        //_center = _player.transform.position;
        if (_alert.GetComponent<AlertLine>().GetArertFlg())
        {
            RotationCursor();
            
        }
        else
        {
            Destroy(this.gameObject);
        }
        
    }

    public void SetAlertCursor()
    {
        _asteroidposition = _alert.GetComponent<AlertLine>().GetAsteroid();
    }

    void RotationCursor()
    {
        Vector3 vec = new Vector3(1,0,1);
        diff = _asteroidposition - transform.position;
        diff.y = 0;
        diff.Normalize();
        transform.rotation = Quaternion.FromToRotation(diff, vec);

        //pos = this.transform.rotation;
        //if (transform.rotation.y > 0f)
        //{
        //    transform.RotateAround(_center, Vector3.up, (pos.y - transform.rotation.y) * 50);
        //}
        //else
        //{
        //    transform.RotateAround(_center, Vector3.up, (pos.y - transform.rotation.y) * -50);
        //}


        //Vector3 posA = transform.eulerAngles;
        //Vector3 posB = _asteroidposition;

        //float distance = Mathf.Sqrt(Mathf.Pow(posA.x - posB.x, 2) + Mathf.Pow(posA.z - posB.z, 2));
        //transform.RotateAround(_center, Vector3.up, distance);
    }
}
