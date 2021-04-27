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

    private Vector3 prePosOffset;
    private Vector3 preRotOffset;
    private Vector3 beforePlayerPos;

    private float orbitShiftRotWaitTime;
    private float posLerpFactor;
    private float rotLerpFactor;
    private float startTime;

    public void SetOrbitShiftRotWaitTime()
    {
        if (pm == null) {
            pm = _player.GetComponent<PlayerMove>();
        }

        
        if (pm.OriginPlanet == PlayerMove.OrbitOriginPlanet.Earth) {
            prePosOffset = _positionOffset[1];
            preRotOffset = _rotationOffset[1];
        } else {
            prePosOffset = _positionOffset[0];
            preRotOffset = _rotationOffset[0];
        }

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
        beforePlayerPos = _player.position;
    }

    private void LateUpdate()
    {
        

        if (0 < orbitShiftRotWaitTime) {
            var diff = Time.timeSinceLevelLoad - startTime;
            var lerpRate = diff / _orbitShiftDelayTime;
            orbitShiftRotWaitTime -= Time.deltaTime;
            posLerpFactor = Mathf.Lerp(_orbitShiftPostionLerpFactor, _lerpFactor, lerpRate);
            rotLerpFactor = Mathf.Lerp(_orbitShiftRotationLerpFactor, _rotLerpFactor, lerpRate);

            OrbitShiftingPosUpdate();
            OrbitShiftingRotUpdate();

            if (orbitShiftRotWaitTime <= 0) {
                posLerpFactor = _lerpFactor;
                rotLerpFactor = _rotLerpFactor;
                orbitShiftRotWaitTime = 0;
            }
        } else {
            MainCameraPositionUpdate();
            MainCameraRotationUpdate();
        }
        
    }

    private void MainCameraPositionUpdate()
    {
        Vector3 localOffset = _player.right * _positionOffset[(int)pm.OriginPlanet].x + _player.up * _positionOffset[(int)pm.OriginPlanet].y + _player.forward * _positionOffset[(int)pm.OriginPlanet].z;

        Vector3 desiredPosition = _player.position + localOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * posLerpFactor);
    }

    private void OrbitShiftingPosUpdate()
    {
        Vector3 localOffset = _player.right * 0 + _player.up * prePosOffset.y + _player.forward * 0;

        beforePlayerPos = Vector3.Lerp(transform.position, _player.position + localOffset, 0.7f);

        Vector3 desiredPosition = beforePlayerPos;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * posLerpFactor);
    }

    private void MainCameraRotationUpdate()
    {
        Vector3 desiredRotation = _player.rotation.eulerAngles + _rotationOffset[(int)pm.OriginPlanet];
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(desiredRotation), Time.deltaTime * rotLerpFactor);
    }

    private void OrbitShiftingRotUpdate()
    {
        Vector3 desiredRotation = _player.rotation.eulerAngles + _rotationOffset[(int)pm.OriginPlanet];
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(desiredRotation), Time.deltaTime * rotLerpFactor);
    }

}
