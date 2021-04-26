using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitGuideLightScript : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    
    [SerializeField] private float _period;
    [SerializeField] private float _resetSec;

    private Quaternion angleAxis;
    [SerializeField] private Vector3 rotateAxis;
    [SerializeField] private Transform orbitOrigin;
    private ParticleSystem particleSystem;

    private float resetTime;

    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    void Start()
    {
        
    }

    void Update()
    {
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
                particleSystem.Clear();
                transform.position = _playerTransform.position;
                transform.rotation = _playerTransform.rotation;
                particleSystem.Play();
            }
        }
    }

    public void OrbitGuideStatusChange(Vector3 newAxis, Transform newOrbitOrigin)
    {
        particleSystem.Clear();
        rotateAxis = newAxis;
        orbitOrigin = newOrbitOrigin;
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
    }


}
