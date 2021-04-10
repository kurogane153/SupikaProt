using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Vector3[] _positionOffset;
    [SerializeField] private Vector3[] _rotationOffset;
    [SerializeField] private float _lerpFactor = 6f;
    [SerializeField] private float _rotLerpFactor = 3f;
    [SerializeField] private float _orbitShiftPostionLerpFactor = 2f;
    [SerializeField] private float _orbitShiftRotationLerpFactor = 2f;
    [SerializeField] private float _orbitShiftDelayTime = 2f;

    private PlayerMove pm;

    private float orbitShiftRotWaitTime;
    private float posLerpFactor;
    private float rotLerpFactor;
    private float startTime;

    public void SetOrbitShiftRotWaitTime()
    {
        orbitShiftRotWaitTime = _orbitShiftDelayTime;
        posLerpFactor = _orbitShiftPostionLerpFactor;
        rotLerpFactor = _orbitShiftRotationLerpFactor;
        startTime = Time.timeSinceLevelLoad;
    }

    private void Start()
    {
        if(_player == null) {
            _player = GameObject.Find("Player").GetComponent<Transform>();
            Debug.Log(gameObject.name + "がPlayerをFindで取得した");
        }

        pm = _player.GetComponent<PlayerMove>();
    }

    private void LateUpdate()
    {
        MainCameraPositionUpdate();
        MainCameraRotationUpdate();        

        if (0 < orbitShiftRotWaitTime) {
            var diff = Time.timeSinceLevelLoad - startTime;
            var lerpRate = diff / _orbitShiftDelayTime;
            orbitShiftRotWaitTime -= Time.deltaTime;
            posLerpFactor = Mathf.Lerp(posLerpFactor, _lerpFactor, lerpRate);
            rotLerpFactor = Mathf.Lerp(rotLerpFactor, _rotLerpFactor, lerpRate);
        } else {
            posLerpFactor = _lerpFactor;
            rotLerpFactor = _rotLerpFactor;
        }
        
    }

    private void MainCameraPositionUpdate()
    {
        Vector3 localOffset = _player.right * _positionOffset[(int)pm.OriginPlanet].x + _player.up * _positionOffset[(int)pm.OriginPlanet].y + _player.forward * _positionOffset[(int)pm.OriginPlanet].z;

        Vector3 desiredPosition = _player.position + localOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * posLerpFactor);
    }

    private void MainCameraRotationUpdate()
    {
        Vector3 desiredRotation = _player.rotation.eulerAngles + _rotationOffset[(int)pm.OriginPlanet];
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(desiredRotation), Time.deltaTime * rotLerpFactor);
    }

}
