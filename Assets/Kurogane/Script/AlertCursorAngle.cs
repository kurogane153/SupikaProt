using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertCursorAngle : MonoBehaviour
{

    [Header("Set Alert Prefab")]
    //アラートの場所
    public GameObject _alert;

    [Header("Set Player Prefab")]
    //プレイヤー
    public GameObject _player;

    // 中心点
    [SerializeField] private Vector3 _center = Vector3.zero;

    // 回転軸
    [SerializeField] private Vector3 _axis = Vector3.up;

    // 円運動周期
    [SerializeField] private float _period = 2;

    private Vector3 _asteroidposition = new Vector3();

    private Quaternion pos = new Quaternion(0, 0, 0, 0);

    private Vector3 diff = Vector3.zero;
    private bool flg = false;

    void Update()
    {
        _center = _player.transform.position;
        if (_alert.GetComponent<AlertLine>().GetArertFlg())
        {
            _asteroidposition = _alert.GetComponent<AlertLine>().GetAsteroid();
            flg = true;
        }
        if (flg)
        {
            RotationCursor();
        }

    }

    void RotationCursor()
    {
        Vector3 vec = new Vector3(1,0,1);
        pos = this.transform.rotation;
        diff = _asteroidposition - transform.position;
        diff.y = 0;
        diff.Normalize();
        transform.rotation = Quaternion.FromToRotation(diff, vec);

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
