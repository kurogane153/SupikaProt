using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Transform _earthTransform;
    [SerializeField] private Vector3 _rotateAxis;
    [SerializeField] private float _period = 10f;
    [SerializeField] private float _speedUpRate = 10f;
    [SerializeField] private float _speedDownRate = 10f;
    [SerializeField] private float _maxPeriod = 200f;
    [SerializeField] private float _minPeriod = 10f;
    [SerializeField] private bool _isUpdateRotation = false;

    private void Awake()
    {
            
    }

    void Start()
    {
        if (_earthTransform == null) {
            _earthTransform = GameObject.Find("Earth").GetComponent<Transform>();
        }
    }

    void Update()
    {
        if(Input.GetButtonDown("SpeedUp") || Input.GetAxis("D_Pad_V") >= 0.5f) {
            SpeedUp();
        }

        if(Input.GetButtonDown("SpeedDown") || Input.GetAxis("D_Pad_V") <= -0.5f) {
            SpeedDown();
        }
    }

    private void FixedUpdate()
    {
        MoveMent();
        
    }

    private void MoveMent()
    {
        Quaternion angleAxis = Quaternion.AngleAxis(360 / _period * Time.deltaTime, _rotateAxis);
        Vector3 newPos = transform.position;

        newPos -= _earthTransform.position;
        newPos = angleAxis * newPos;
        newPos += _earthTransform.position;

        transform.position = newPos;

        if (_isUpdateRotation) {
            transform.rotation = transform.rotation * angleAxis;
        }

    }

    private void SpeedUp()
    {
        _period += _speedUpRate;
        if(_maxPeriod < _period) {
            _period = _maxPeriod;
        }
    }

    private void SpeedDown()
    {
        _period -= _speedDownRate;
        if(_period < _minPeriod) {
            _period = _minPeriod;
        }
    }

   
}
