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

    private float beforeVertical;
    private Quaternion angleAxis;
    private float nowAngle;

    #region デバッグ用変数
    [Watch, HideInInspector]
    public float _dgb_playerNowAngle;

    #endregion

    public Transform GetEarthTransform()
    {
        return _earthTransform;
    }

    public Quaternion GetAngleAxis()
    {
        return angleAxis;
    }

    private void Awake()
    {
            
    }

    void Start()
    {
        if (_earthTransform == null) {
            _earthTransform = GameObject.Find("Earth").GetComponent<Transform>();
            Debug.Log(gameObject.name + "がEarthをFindで取得した");
        }
    }

    void Update()
    {
        SpeedControllInput();
    }

    private void FixedUpdate()
    {
        MoveMent();
        UpdateNowAngle();
        Dbg();
        
    }

    private void SpeedControllInput()
    {
        float nowVertical = Input.GetAxis("Vertical");
        bool isNotInputedVertical = (-0.5f < beforeVertical && beforeVertical < 0.5f);

        if (nowVertical >= 0.5f && isNotInputedVertical) {
            SpeedUp();
            Debug.Log(gameObject.name + "が速度を" + _speedUpRate + "上げた。現在" + _period);
        }

        if (nowVertical <= -0.5f && isNotInputedVertical) {
            SpeedDown();
            Debug.Log(gameObject.name + "が速度を" + _speedDownRate + "下げた。現在" + _period);
        }

        beforeVertical = nowVertical;
    }

    private void MoveMent()
    {
        angleAxis = Quaternion.AngleAxis(360 / _period * Time.deltaTime, _rotateAxis);
        Vector3 newPos = transform.position;

        newPos -= _earthTransform.position;
        newPos = angleAxis * newPos;
        newPos += _earthTransform.position;

        transform.position = newPos;

        if (_isUpdateRotation) {
            transform.rotation = transform.rotation * angleAxis;
        }

    }

    private void UpdateNowAngle()
    {
        Vector3 vec = transform.position - _earthTransform.position;
        float rad = Mathf.Atan2(vec.x, vec.z);
        nowAngle = rad * Mathf.Rad2Deg;

        if (nowAngle < 0) {
            nowAngle += 360;
        }

    }

    private void SpeedDown()
    {
        _period += _speedUpRate;
        if(_maxPeriod < _period) {
            _period = _maxPeriod;
        }
    }

    private void SpeedUp()
    {
        _period -= _speedDownRate;
        if(_period < _minPeriod) {
            _period = _minPeriod;
        }
    }

    private void Dbg()
    {
        _dgb_playerNowAngle = nowAngle;
    }
    
}
