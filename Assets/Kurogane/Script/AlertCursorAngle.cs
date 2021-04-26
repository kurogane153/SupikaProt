using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertCursorAngle : MonoBehaviour
{

    private Vector3 _asteroidposition = new Vector3();
    private Vector3 diff;
    private GameObject _alert;
    private GameObject _alertSpika;

    void Start()
    {
        diff = Vector3.zero;
        //SetAlertCursor();
    }

    void Update()
    {
        //if (!_alert.GetComponent<AlertLine>().GetArertFlg())
        //{
        //    Destroy(this.gameObject);
        //}
        //RotationCursor();
    }

    public void SetAlertCursor()
    {
        _asteroidposition = _alert.GetComponent<AlertLine>().GetAsteroid();
    }

    public void SetAlertPos(GameObject alert)
    {
        _alert = alert;
    }

    void RotationCursor()
    {
        Vector3 vec = new Vector3(1,0,1);
        diff = _asteroidposition - transform.position;
        diff.y = 0;
        transform.rotation = Quaternion.FromToRotation(diff, vec);
    }
}
