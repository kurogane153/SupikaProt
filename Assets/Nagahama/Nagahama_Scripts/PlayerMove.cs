using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Transform _earthTransform;
    [SerializeField] private Vector3 _rotateAxis;
    [SerializeField] private float _period = 10f;
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
}
