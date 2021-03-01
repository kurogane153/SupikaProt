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

    private PlayerMove playerMove;
    private float shotTimeRemain;

    void Start()
    {
        if(_launchPoint == null) {
            Debug.Log(gameObject.name + "の_launchPointが空です");
        }

        if(_ballPrefab == null) {
            Debug.Log(gameObject.name + "の_ballPrefabが空です");
        }

        playerMove = GetComponent<PlayerMove>();
    }

    void Update()
    {
        if (Input.GetButtonDown(_fireButtonName) && shotTimeRemain <= 0f) {
            BallShot();
            Debug.Log(gameObject.name + "が弾を発射した");
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

        timeLeapBallScript.LaunchBall(_launchPoint.TransformDirection(new Vector3(0,0,1)), _shotPower, playerMove.GetAngleAxis(), _launchPoint);

        shotTimeRemain += _shotDelay;
    }

    private void DrawLaserLine()
    {
        RaycastHit hit;
        Ray ray = new Ray(_launchPoint.position, _launchPoint.TransformDirection(new Vector3(0, 0, 1)));

        Physics.Raycast(ray.origin, ray.direction, out hit, _laserLength);

        Debug.DrawRay(ray.origin, ray.direction * _laserLength, Color.green);
    }
}
