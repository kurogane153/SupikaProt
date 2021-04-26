using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitGuideLightScript : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    
    [SerializeField] private float _period;

    private Quaternion angleAxis;
    private Vector3 rotateAxis;
    private Transform orbitOrigin;
    private ParticleSystem particleSystem;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        angleAxis = Quaternion.AngleAxis(360 / _period * Time.deltaTime, rotateAxis);
        Vector3 newPos = transform.position;

        newPos -= orbitOrigin.position;
        newPos = angleAxis * newPos;
        newPos += orbitOrigin.position;

        transform.position = newPos;
    }

    public void OrbitGuideStatusChange(Vector3 newAxis, Transform newOrbitOrigin)
    {
        particleSystem.Clear();
        rotateAxis = newAxis;
        orbitOrigin = newOrbitOrigin;
    }

    private void OnEnable()
    {
        transform.position = _playerTransform.position;
        transform.rotation = _playerTransform.rotation;
        particleSystem.Play();
    }

    private void OnDisable()
    {
        particleSystem.Stop();
    }


}
