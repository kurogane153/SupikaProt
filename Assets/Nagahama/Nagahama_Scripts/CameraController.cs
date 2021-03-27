using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Vector3[] _positionOffset;
    [SerializeField] private Vector3[] _rotationOffset;
    [SerializeField] private float _lerpFactor = 6;

    private PlayerMove pm;

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
        Vector3 localOffset = _player.right * _positionOffset[(int)pm.OriginPlanet].x + _player.up * _positionOffset[(int)pm.OriginPlanet].y + _player.forward * _positionOffset[(int)pm.OriginPlanet].z;

        Vector3 desiredPosition = _player.position + localOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * _lerpFactor);

        Vector3 desiredRotation = _player.rotation.eulerAngles + _rotationOffset[(int)pm.OriginPlanet];
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(desiredRotation), Time.deltaTime * _lerpFactor);
    }
}
