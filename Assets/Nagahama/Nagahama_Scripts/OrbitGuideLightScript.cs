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

    [SerializeField] private float[] _lagrangePointMinAngle;
    [SerializeField] private float[] _lagrangePointMaxAngle;

    private Quaternion angleAxis;
    [SerializeField] private Vector3 rotateAxis;
    [SerializeField] private Transform orbitOrigin;
    private ParticleSystem particleSystem;

    private float resetTime;
    private bool changeFlg;
    private int orbitNum = 0;
    private float nowAngle;

    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    void Start()
    {
        
    }

    void Update()
    {

        if (changeFlg && _lagrangePointMinAngle[orbitNum] < nowAngle && nowAngle < _lagrangePointMaxAngle[orbitNum]) {
            if(orbitNum == 0) {
                rotateAxis = _rotateSpicaAxis;
                orbitOrigin = _colonyTransform;

            } else {
                rotateAxis = _rotateEarthAxis;
                orbitOrigin = _earthTransform;
            }
        }

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
        if(0 < resetTime) {
            resetTime -= Time.deltaTime;
            if(resetTime <= 0) {
                resetTime = _resetSec;
                transform.position = _playerTransform.position;
                transform.rotation = _playerTransform.rotation;
                particleSystem.Play();
                if (orbitNum == 0) {
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
        resetTime = 0.1f;
        changeFlg = flg;
        if(orbitNum == 0) {
            orbitNum = 1;
        } else {
            orbitNum = 0;
        }
        transform.position = _playerTransform.position;
        transform.rotation = _playerTransform.rotation;
    }

    private void OnEnable()
    {
        transform.position = _playerTransform.position;
        transform.rotation = _playerTransform.rotation;
        particleSystem.Play();
        resetTime = _resetSec;
    }

    private void OnDisable()
    {
        particleSystem.Stop();
        changeFlg = false;
    }


}
