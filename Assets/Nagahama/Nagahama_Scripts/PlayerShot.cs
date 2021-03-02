using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShot : MonoBehaviour
{
    [SerializeField] private SoundPlayer _soundPlayer;
    [SerializeField] private AudioClip _se_MissileLaunch;

    [Space(10)]
    [SerializeField] private Transform _launchPoint;
    [SerializeField] private float _laserLength = 1000f;
    [SerializeField] private string _fireButtonName = "Fire1";
    [SerializeField] private string _missileFireButtonName = "Fire2";

    [Space(10)]
    [SerializeField] private GameObject _ballPrefab;
    [SerializeField] private float _shotPower = 50f;
    [SerializeField] private float _shotDelay = 0.1f;

    [Space(10)]
    [SerializeField] private Transform _targetAsteroid;

    [Space(10)]
    [SerializeField] private GameObject _missilePrefab;
    [SerializeField] private Transform[] _halfwayPoints;
    [SerializeField] private float[] _impactTimes;
    [SerializeField] private float[] _instantiateTimes;

    private PlayerMove playerMove;
    private float shotTimeRemain;

    void Start()
    {
        if(_launchPoint == null) {
            Debug.LogError(gameObject.name + "の" + nameof(_launchPoint) + "が空です");
        }

        if(_ballPrefab == null) {
            Debug.LogError(gameObject.name + "の" + nameof(_ballPrefab) + "が空です");
        }

        if (_missilePrefab == null) {
            Debug.LogError(gameObject.name + "の" + nameof(_missilePrefab) + "が空です");
        }

        if (_soundPlayer == null) {
            if((_soundPlayer = GetComponentInChildren<SoundPlayer>()) == null) {
                Debug.LogError(gameObject.name + "の" + nameof(_soundPlayer) + "が空です");
            }
            Debug.Log(gameObject.name + "は、子要素にアタッチされているAudioSourceを自動的に" + nameof(_soundPlayer) + "にアタッチしました");
        }

        playerMove = GetComponent<PlayerMove>();
    }

    void Update()
    {
        if (Input.GetButtonDown(_missileFireButtonName) && shotTimeRemain <= 0f) {
            MultiStageFire();
            Debug.Log(gameObject.name + "がミサイルを発射した");
        }

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
        ball.transform.parent = gameObject.transform;
        TimeLeapBallScript timeLeapBallScript = ball.GetComponent<TimeLeapBallScript>();

        timeLeapBallScript.LaunchBall(_launchPoint);

        shotTimeRemain += _shotDelay;
        _soundPlayer.PlaySE(_se_MissileLaunch);
    }

    /// <summary>
    /// 多段発射ミサイル。
    /// 
    /// </summary>
    private void MultiStageFire()
    {
        if(_halfwayPoints.Length != _impactTimes.Length) {
            Debug.LogError(nameof(_halfwayPoints) + "の要素数が、" + nameof(_impactTimes) + "の要素数と同じになっていません。");
            return;
        }

        if (_halfwayPoints.Length != _instantiateTimes.Length) {
            Debug.LogError(nameof(_halfwayPoints) + "の要素数が、" + nameof(_instantiateTimes) + "の要素数と同じになっていません。");
            return;
        }

        int i = 0;

        foreach(var hp in _halfwayPoints) {
            StartCoroutine(MissileInstantiate(i++));
        }

        shotTimeRemain += _shotDelay;
    }

    /// <summary>
    /// 一度に複数回呼び出すとき用
    /// </summary>
    /// <param name="halfwaypoint">ミサイルが一度通過する中間地点</param>
    /// <param name="impacttime">着弾するまでの時間</param>
    private void MissileShot(Transform halfwaypoint, float impacttime)
    {
        GameObject missile = Instantiate(_missilePrefab, _launchPoint.position, Quaternion.identity);
        HomingMissileScript homingMissileScript = missile.GetComponent<HomingMissileScript>();

        homingMissileScript.LaunchMissile(_targetAsteroid, halfwaypoint, impacttime, halfwaypoint.position - transform.position);
        _soundPlayer.PlaySE(_se_MissileLaunch);
    }

    /// <summary>
    /// 一回だけ呼び出すとき用
    /// </summary>
    /// <param name="halfwaypoint">ミサイルが一度通過する中間地点</param>
    /// <param name="impacttime">着弾するまでの時間</param>
    /// <param name="delaytime">発射ディレイ</param>
    private void MissileShot(Transform halfwaypoint, float impacttime, float delaytime)
    {
        GameObject missile = Instantiate(_missilePrefab, _launchPoint.position, Quaternion.identity);
        HomingMissileScript homingMissileScript = missile.GetComponent<HomingMissileScript>();

        homingMissileScript.LaunchMissile(_targetAsteroid, halfwaypoint, impacttime, halfwaypoint.position - transform.position * 50f);
        shotTimeRemain += delaytime;
        _soundPlayer.PlaySE(_se_MissileLaunch);
    }

    private IEnumerator MissileInstantiate(int index)
    {
        yield return new WaitForSeconds(_instantiateTimes[index]);
        MissileShot(_halfwayPoints[index], _impactTimes[index]);

    }

    private void DrawLaserLine()
    {
        RaycastHit hit;
        Ray ray = new Ray(_launchPoint.position, _launchPoint.TransformDirection(new Vector3(0, 0, 1)));

        Physics.Raycast(ray.origin, ray.direction, out hit, _laserLength);

        Debug.DrawRay(ray.origin, ray.direction * _laserLength, Color.green);
    }
}
