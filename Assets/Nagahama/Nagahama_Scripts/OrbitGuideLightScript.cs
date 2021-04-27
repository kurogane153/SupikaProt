using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitGuideLightScript : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField, Tooltip("地球")] private Transform _earthTransform;
    [SerializeField, Tooltip("コロニー")] private Transform _colonyTransform;
    [SerializeField, Tooltip("地球の外周を移動するときの回転軸")] private Vector3 _rotateEarthAxis;
    [SerializeField, Tooltip("コロニーの外周を移動するときの回転軸")] private Vector3 _rotateSpicaAxis;

    [SerializeField] private float _period;
    [SerializeField] private float _resetSec;

    [SerializeField] private float _lagrangePointAngle;

    private Quaternion angleAxis;
    [SerializeField] private Vector3 rotateAxis;
    [SerializeField] private Transform orbitOrigin;
    private ParticleSystem particleSystem;

    private float resetTime;
    private bool changeFlg;
    private int afterOrbitNum = 0;
    private int nownum;
    private float nowAngle;
    private float beforeAngle;

    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    void Start()
    {
        
    }

    void Update()
    {

        if (changeFlg) {

            if(afterOrbitNum == 1 && beforeAngle <= _lagrangePointAngle && _lagrangePointAngle <= nowAngle) {
                rotateAxis = _rotateSpicaAxis;
                orbitOrigin = _colonyTransform;

            }
            
            if(afterOrbitNum == 0 && nowAngle <= _lagrangePointAngle && _lagrangePointAngle <= beforeAngle) {
                rotateAxis = _rotateEarthAxis;
                orbitOrigin = _earthTransform;
            }
            Debug.Log("ラグランジュ点入った");
        }

        beforeAngle = nowAngle;

        Vector3 vec = transform.position - orbitOrigin.position;
        float rad = Mathf.Atan2(vec.x, vec.z);
        nowAngle = rad * Mathf.Rad2Deg;

        if (rotateAxis == _rotateSpicaAxis) {
            nowAngle -= 180;
        }

        if (nowAngle < 0) {
            nowAngle += 360;
        }

        angleAxis = Quaternion.AngleAxis(360 / _period * Time.deltaTime, rotateAxis);
        Vector3 newPos = transform.position;

        newPos -= orbitOrigin.position;
        newPos = angleAxis * newPos;
        newPos += orbitOrigin.position;

        transform.position = newPos;

        transform.rotation = transform.rotation * angleAxis;

        
    }

    private void FixedUpdate()
    {
        if(0 <= resetTime) {
            resetTime -= Time.deltaTime;
            if(resetTime < 0) {
                resetTime = _resetSec;
                transform.position = _playerTransform.position;
                transform.rotation = _playerTransform.rotation;
                particleSystem.Play();
                if (nownum == 0) {
                    rotateAxis = _rotateEarthAxis;
                    orbitOrigin = _earthTransform;

                } else {
                    rotateAxis = _rotateSpicaAxis;
                    orbitOrigin = _colonyTransform;
                }
            }
        }
    }

    public void OrbitGuideStatusChange(bool flg)
    {
        particleSystem.Clear();
        particleSystem.Stop();
        resetTime = _resetSec;
        changeFlg = flg;
        if(afterOrbitNum == 0) {
            afterOrbitNum = 1;
        } else {
            afterOrbitNum = 0;
        }
        transform.position = _playerTransform.position;
        transform.rotation = _playerTransform.rotation;
    }

    private void OnEnable()
    {
        transform.position = _playerTransform.position;
        transform.rotation = _playerTransform.rotation;
        particleSystem.Play();
        nownum = afterOrbitNum;
        
    }

    private void OnDisable()
    {
        particleSystem.Stop();
        changeFlg = false;
        nownum = afterOrbitNum;
    }


}
