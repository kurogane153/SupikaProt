using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShot : MonoBehaviour
{
    [SerializeField] private Transform _launchPoint;
    [SerializeField] private float _laserLength = 1000f;
    [SerializeField] private string _fireButtonName = "Fire1";

    [SerializeField] private GameObject _ballPrefab;
    [SerializeField] private float _shotPower = 50f;
    [SerializeField] private float _shotDelay = 0.1f;

    private float shotTimeRemain;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetButtonDown(_fireButtonName) && shotTimeRemain <= 0f) {
            BallShot();
        }
    }

    private void FixedUpdate()
    {
        if(0f < shotTimeRemain) {
            shotTimeRemain -= Time.deltaTime;
        }
        DrawLaserLine();
    }

    private void BallShot()
    {
        GameObject ball = Instantiate(_ballPrefab, _launchPoint.position, Quaternion.identity);
        TimeLeapBallScript timeLeapBallScript = ball.GetComponent<TimeLeapBallScript>();

        timeLeapBallScript.LaunchBall(_launchPoint.forward, _shotPower);

        shotTimeRemain += _shotDelay;
    }

    private void DrawLaserLine()
    {
        RaycastHit hit;
        Ray ray = new Ray(_launchPoint.position, _launchPoint.TransformDirection(Vector3.forward));

        Physics.Raycast(ray.origin, ray.direction, out hit, _laserLength);

        Debug.DrawRay(ray.origin, ray.direction * _laserLength, Color.green);
    }
}
