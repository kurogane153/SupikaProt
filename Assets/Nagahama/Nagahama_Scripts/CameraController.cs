using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Vector3 _positionOffset = new Vector3(0, 1, -6);
    [SerializeField] private Vector3 _rotationOffset = new Vector3(0, 1, -6);
    [SerializeField] private float _lerpFactor = 6;

    private void Start()
    {
        if(_player == null) {
            _player = GameObject.Find("Player").GetComponent<Transform>();
            Debug.Log(gameObject.name + "がPlayerをFindで取得した");
        }
    }

    private void LateUpdate()
    {
        Vector3 localOffset = _player.right * _positionOffset.x + _player.up * _positionOffset.y + _player.forward * _positionOffset.z;

        Vector3 desiredPosition = _player.position + localOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * _lerpFactor);

        Vector3 desiredRotation = _player.rotation.eulerAngles + _rotationOffset;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(desiredRotation), Time.deltaTime * _lerpFactor);
    }
}
