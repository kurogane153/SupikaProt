using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShot : MonoBehaviour
{
    [SerializeField] private PlayerAnimation _playerAnimation;
    [Header("サウンド系")]
    [SerializeField] private SoundPlayer _soundPlayer;
    [SerializeField] private AudioClip _se_MissileLaunch;

    [Header("キルカメラ"), Space(10)]
    [SerializeField] private KillCameraScript _killCamera;

    [Header("発射設定全般"), Space(10)]
    [SerializeField] private ReticleController _reticle;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Transform _launchPoint;
    [SerializeField] private float _laserLength = 1000f;
    [SerializeField] private string _aimXAxisName = "Aim-Horizontal";
    [SerializeField] private string _aimYAxisName = "Aim-Vertical";
    [SerializeField] private string _missileFireButtonName = "Fire2";

    [Header("ミサイルの設定"), Space(10)]
    [SerializeField] private GameObject _missilePrefab;
    [SerializeField] private float _missileShotPower = 100f;
    [SerializeField] private float _missileShotDelay = 0.5f;
    [SerializeField] private float _multiTargetMissileDelay = 0.1f;
    [SerializeField] private int _missileDamage = 1;
    [SerializeField] private Transform[] _halfwayPoints;
    [SerializeField] private float[] _impactTimes;
    [SerializeField] private float[] _instantiateTimes;

    private PlayerMove playerMove;
    private Transform targetAsteroid;
    private Transform confirmTarget;
    private float shotTimeRemain;
    private float missileShotTimeRemain;

    #region デバッグ用変数
    [Watch, HideInInspector] public string _dbg_targetAsteroid = "None";

    #endregion

    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
    }

    void Start()
    {
        if (_playerAnimation == null) {
            _playerAnimation = GetComponentInChildren<PlayerAnimation>();
            Debug.Log(gameObject.name + "がPlayerAnimationをGetComponentInChildrenで取得した");
        }

        if (_reticle == null) {
            _reticle = GameObject.Find("Reticle").GetComponent<ReticleController>();
            Debug.Log(gameObject.name + "がReticleをFindで取得した");
        }

        if (_launchPoint == null) {
            Debug.LogError(gameObject.name + "の" + nameof(_launchPoint) + "が空です");
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

    }

    void Update()
    {
        // ターゲットを捉えているときに発射ボタンを押すと、そのターゲットを「確定したターゲット」として
        // confirmTargetに格納します。
        if (Input.GetButtonDown(_missileFireButtonName) && missileShotTimeRemain <= 0f && targetAsteroid != null) {
            confirmTarget = targetAsteroid;
            //MultiStageFire();
            //MultiTargetFire();
            //Debug.Log(gameObject.name + "がミサイルを発射した");
        }

        if (Input.GetButtonDown(_missileFireButtonName) && missileShotTimeRemain <= 0f) {
            confirmTarget = targetAsteroid;
            //MultiStageFire();
            MultiTargetFire();
            Debug.Log(gameObject.name + "がミサイルを発射した");
        }

    }

    private void FixedUpdate()
    {
        TimeRemainManege();
        GetTargetAsteroid();
        Dbg();
        _reticle.MoveReticle( Input.GetAxis(_aimXAxisName), Input.GetAxis(_aimYAxisName), targetAsteroid);
    }

    private void TimeRemainManege()
    {
        if (0f < shotTimeRemain) {
            shotTimeRemain -= Time.deltaTime;
        }

        if (0f < missileShotTimeRemain) {
            missileShotTimeRemain -= Time.deltaTime;
        }
    }

    /// <summary>
    /// 多段発射ミサイル。
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

        missileShotTimeRemain += _missileShotDelay;
    }

    /// <summary>
    /// 複数ターゲット指定して多段発射
    /// </summary>
    private void MultiTargetFire()
    {
        LockedOnReticle[] reticles = FindObjectsOfType<LockedOnReticle>();
        if(reticles.Length == 0) {
            Debug.Log(nameof(LockedOnReticle) + "が一つも無かった");
            return;
        }

        if (_halfwayPoints.Length != _impactTimes.Length) {
            Debug.LogError(nameof(_halfwayPoints) + "の要素数が、" + nameof(_impactTimes) + "の要素数と同じになっていません。");
            return;
        }

        if (_halfwayPoints.Length != _instantiateTimes.Length) {
            Debug.LogError(nameof(_halfwayPoints) + "の要素数が、" + nameof(_instantiateTimes) + "の要素数と同じになっていません。");
            return;
        }

        StartCoroutine(MultiTargetMissileInstantiate(reticles));

        missileShotTimeRemain += _missileShotDelay;
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

        homingMissileScript.LaunchMissile(confirmTarget, halfwaypoint, impacttime, halfwaypoint.position - _launchPoint.position, _missileShotPower, _missileDamage);

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

        homingMissileScript.LaunchMissile(confirmTarget, halfwaypoint, impacttime, halfwaypoint.position - _launchPoint.position, _missileShotPower, _missileDamage);

        missileShotTimeRemain += _missileShotDelay;

        _soundPlayer.PlaySE(_se_MissileLaunch);
    }

    /// <summary>
    /// 複数呼び出すときのターゲット指定版
    /// </summary>
    /// <param name="target">ターゲットのTransform</param>
    /// <param name="halfwaypoint">ミサイルが一度通過する中間地点</param>
    /// <param name="impacttime">着弾するまでの時間</param>
    private GameObject MissileShot(Transform target, Transform halfwaypoint, float impacttime)
    {
        GameObject missile = Instantiate(_missilePrefab, _launchPoint.position, Quaternion.identity);
        HomingMissileScript homingMissileScript = missile.GetComponent<HomingMissileScript>();

        homingMissileScript.LaunchMissile(target, halfwaypoint, impacttime, _launchPoint.position - halfwaypoint.position, _missileShotPower, _missileDamage);

        _soundPlayer.PlaySE(_se_MissileLaunch);

        return missile;
    }

    private IEnumerator MissileInstantiate(int index)
    {
        yield return new WaitForSeconds(_instantiateTimes[index]);
        MissileShot(_halfwayPoints[index], _impactTimes[index]);

    }

    private IEnumerator MissileInstantiate(Transform target, int index, int count)
    {
        yield return new WaitForSeconds(_instantiateTimes[index]);
        GameObject missile = MissileShot(target, _halfwayPoints[index], _impactTimes[index]);

        if(count == ReticleController.Instance.GenerateReticleMax && index == _halfwayPoints.Length - 1) {
            
            _killCamera.SetFollowMissile(missile);
            
        }
    }

    private IEnumerator MultiTargetMissileInstantiate(LockedOnReticle[] reticles)
    {

        if(reticles.Length == ReticleController.Instance.GenerateReticleMax) {
            _killCamera.GetCamera().enabled = true;
            _killCamera.SwitchStagingPhase_Pan();
            playerMove.enabled = false;
            ReticleController.Instance.GetCanvas().enabled = false;
            Pauser.Pause();
            Time.timeScale = 1f;
            enabled = false;
        }

        int i = 0;
        int count = 0;
        foreach (var tgt in reticles) {
            count++;
            foreach (var hp in _halfwayPoints) {
                StartCoroutine(MissileInstantiate(tgt.Target, i++, count));
            }
            yield return new WaitForSeconds(_multiTargetMissileDelay);
            i = 0;
            
        }
        
    }


    private void GetTargetAsteroid()
    {
        Ray ray = Camera.main.ScreenPointToRay(_reticle.GetReticlePos());

        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, _laserLength, _layerMask) && hit.transform.CompareTag("Asteroid")) {
            targetAsteroid = hit.transform;

        } else {
            targetAsteroid = null;

        }

        //Debug.DrawRay(ray.origin, ray.direction * _laserLength, Color.green);
    }

    private void Dbg()
    {
        Ray ray = Camera.main.ScreenPointToRay(_reticle.GetReticlePos());

        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, _laserLength, _layerMask) && hit.transform.CompareTag("Asteroid")) {
            _dbg_targetAsteroid = targetAsteroid.name;
        } else {
            _dbg_targetAsteroid = "None";
        }

    }
}
