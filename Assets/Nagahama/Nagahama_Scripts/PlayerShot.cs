using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerShot : MonoBehaviour
{

    [System.Serializable]
    public class MissileShotSettings
    {

        [SerializeField, Tooltip("ミサイルの発射時初速")] 
        public float[] _missileShotPower;

        [SerializeField, Tooltip("次回発射までのクールタイム")]
        public float _missileShotDelay;

        [SerializeField, Tooltip("複数ターゲット発射時、次ターゲット発射までの待機時間")]
        public float _multiTargetMissileDelay;

        [SerializeField, Tooltip("アニメーショントリガーの名前")]
        public string _animationTriggerName;

        [SerializeField, Tooltip("アニメーションの長さ")]
        public float _animWaitTime;

        [SerializeField, Tooltip("ミサイルの発射方向")]
        public Transform[] _halfwayPoints;

        [SerializeField, Tooltip("着弾までの時間")]
        public float[] _impactTimes;

        [SerializeField, Tooltip("発射してから生成までの待機時間")]
        public float[] _instantiateTimes;
    }

    [Header("操作有効無効切り替え")]
    [SerializeField] public bool _isStickControllFlag;
    [SerializeField] public bool _isMissileFireControllFlag;

    [Header("リロード"), Space(10)]
    [SerializeField] public bool _reloadFlags;
    [SerializeField] public bool _autoReloadFlags;
    [SerializeField] private float _reloadTime;
    [SerializeField] private int _missileMaxNum;
    [SerializeField] private int _consumptionPerShot;   // ミサイル1回発射ごとの消費量
    [SerializeField] private string _reloadButtonName = "Reload";

    private float reloadTimeRemain;
    private int missileNum;

    [Space(10)]
    [SerializeField] private PlayerAnimation _playerAnimation;
    [Header("サウンド系")]
    [SerializeField] private SoundPlayer _soundPlayer;
    [SerializeField] private AudioClip _se_MissileLaunch;

    [Header("キルカメラ"), Space(10)]
    [SerializeField] private KillCameraScript _killCamera;
    [SerializeField] private int _killCameraActiveCount = 3;

    [Header("発射設定全般"), Space(10)]
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private FinalTargetRockOnEffectScript _finalRockOnEffectSc;
    [SerializeField] private bool _onDrawGizmosFlags;
    [SerializeField] private ReticleController _reticle;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Transform _launchPoint;
    [SerializeField] private float _laserLength = 1000f;
    [SerializeField] private string _aimXAxisName = "Aim-Horizontal";
    [SerializeField] private string _aimYAxisName = "Aim-Vertical";
    [SerializeField] private string _missileFireButtonName = "Fire2";

    [Header("ミサイルの設定"), Space(10)]
    [SerializeField] private GameObject _missilePrefab;
    [SerializeField] private int _missileDamage = 1;
    [SerializeField, Tooltip("初回発射時の設定番号")] private int _firstShotNumber = 0;
    [SerializeField, Tooltip("例外時デフォルトのミサイル設定")] private int _defaultSettingsNumber;
    [SerializeField, Tooltip("キルカメラ時ミサイル設定")] private int _killCamSettingsNumber;
    
    [Header("下段にそのグループ内の何発目のミサイルに追従するか指定できる")]
    [Header("上段に何回目に発射されたミサイルのグループにキルカメラが追従するか")]
    [SerializeField, Tooltip("何回目に生成されたミサイルのグループにカメラが追従するか")] private int _followKillCamMultiTargetMissilesCount;
    [SerializeField, Tooltip("キルカメラ時何番目に生成されたミサイルをカメラが追従するか")] private int _followKillCamMissileNum;

    [SerializeField, Space(10)] private MissileShotSettings[] _missileShotSettings;

    private PlayerMove playerMove;
    private Transform targetAsteroidCollider;
    private Transform confirmTarget;
    private Transform tmpTarget;
    private float missileShotTimeRemain;
    private bool isFirstShot;
    private int lastMissileSettingsArrayNum;

    #region デバッグ用変数
    [Watch, HideInInspector] public string _dbg_targetAsteroid = "None";
    [Watch, HideInInspector] public string _dbg_tmptarget = "None";

    #endregion

    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        missileNum = _missileMaxNum;
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

        if (_mainCamera == null) {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            Debug.Log(gameObject.name + "が_mainCameraをFindで取得した");
        }

        if (!_reloadFlags) {
            //_reticle._missileGuage.gameObject.SetActive(false);
        }

        _onDrawGizmosFlags = true;

    }

    void Update()
    {
        // ターゲットを捉えているときに発射ボタンを押すと、そのターゲットを「確定したターゲット」として
        // confirmTargetに格納します。
        
        if (_isMissileFireControllFlag && _reloadFlags) {
            // リロードありバージョン
            // 手動リロード
            // リロード中でない、ミサイルが最大じゃないときできる
            if(Input.GetButtonDown(_reloadButtonName) && missileShotTimeRemain <= 0f && reloadTimeRemain <= 0f && missileNum != _missileMaxNum) {
                reloadTimeRemain = _reloadTime * ((float)(_missileMaxNum - missileNum) / _missileMaxNum);
            }

            // リロード中でない、弾が残っていれば発射
            if (Input.GetButtonDown(_missileFireButtonName) && missileShotTimeRemain <= 0f && reloadTimeRemain <= 0f && 0 < missileNum) {
                int arynum = Random.Range(0, _missileShotSettings.Length - 1);
                confirmTarget = targetAsteroidCollider;
                MultiTargetFire(arynum);
            }

        } else if(_isMissileFireControllFlag) {
            // リロード無しバージョン
            if (Input.GetButtonDown(_missileFireButtonName) && missileShotTimeRemain <= 0f) {

                if (_reticle.IsFinalLockOn) {
                    SceneManager.LoadScene(4);
                    return;
                }
                
                int arynum = Random.Range(0, _missileShotSettings.Length - 1);
                if(isFirstShot) {
                    arynum = _firstShotNumber;
                    isFirstShot = true;
                }
                confirmTarget = targetAsteroidCollider;
                MultiTargetFire(arynum);
            }
        }

        // リロード用　あとで消すこと リロードシステムのオンオフをゲーム中に変更できる
        /*
        if (Input.GetAxis("D_Pad_V") > 0.5f) {
            _reloadFlags = true;
            _reticle._missileGuage.gameObject.SetActive(true);
            reloadTimeRemain = 0f;
            missileNum = 6;
        } else if (Input.GetAxis("D_Pad_V") < -0.5f) {
            _reloadFlags = false;
            _reticle._missileGuage.gameObject.SetActive(false);
            reloadTimeRemain = 0f;
            missileNum = 6;
        } */

    }

    private void FixedUpdate()
    {
        TimeRemainManege();

        if (_isStickControllFlag) {
            GetTargetAsteroid_InRectVersion();
        }
        
        Dbg();

        if (ReticleController.Instance._userSuperAimAssistSystemFlags) {
            // 強力エイムアシスト機能がオンのとき

            if (targetAsteroidCollider == null) {
                _reticle.MoveReticle(Input.GetAxis(_aimXAxisName), Input.GetAxis(_aimYAxisName), targetAsteroidCollider);
            } else {
                _reticle.MoveReticle(0, 0, targetAsteroidCollider);
            }

        } else {
            // 強力エイムアシスト機能がOFFのとき

            float horizontal = Input.GetAxis(_aimXAxisName);
            float vertical = Input.GetAxis(_aimYAxisName);
            if (!_isStickControllFlag) {
                horizontal = 0f;
                vertical = 0f;
            }
            _reticle.MoveReticle(horizontal, vertical, targetAsteroidCollider, tmpTarget);
        }        
        
    }

    private void TimeRemainManege()
    {
        // 次弾発射までのクールダウン
        if (0f < missileShotTimeRemain) {
            missileShotTimeRemain -= Time.deltaTime;
            _reticle._missileGuage.fillAmount = (_missileShotSettings[lastMissileSettingsArrayNum]._missileShotDelay - missileShotTimeRemain) / _missileShotSettings[lastMissileSettingsArrayNum]._missileShotDelay;
            
            if (missileShotTimeRemain <= 0f) {
                _reticle.ChangerReticleImageAlpha(false);
            }
        }

        // リロードシステムがオフのときは以下の処理は行わない
        if (!_reloadFlags) return;

        // 自動リロード弾切れ時
        if (missileShotTimeRemain <= 0f && missileNum <= 0 && reloadTimeRemain <= 0f) {
            if (_autoReloadFlags) {
                reloadTimeRemain = _reloadTime;
            }
        }

        // リロードの残り時間
        if (0f < reloadTimeRemain) {
            reloadTimeRemain -= Time.deltaTime;
            _reticle._missileGuage.fillAmount = (_reloadTime - reloadTimeRemain) / _reloadTime;
            if(reloadTimeRemain <= 0f) {
                missileNum = _missileMaxNum;
            }
        }
    }

    /// <summary>
    /// 複数ターゲット指定して多段発射
    /// </summary>
    private void MultiTargetFire(int arynum)
    {
        int num = arynum;
        LockedOnReticle[] reticles = FindObjectsOfType<LockedOnReticle>();

        if(reticles.Length == 0) {
            Debug.Log(nameof(LockedOnReticle) + "が一つも無かった");
            return;
        }

        // キルカメラ発動可能数なら、キルカメラ用の番号を起動する
        if (reticles.Length >= _killCameraActiveCount) {
            num = _killCamSettingsNumber;
        } else {
            // そうでない場合にキルカメラ用の番号になっていたら、
            // 0番にする
            if(num == _killCamSettingsNumber) {
                num = _defaultSettingsNumber;
            }
        }

        lastMissileSettingsArrayNum = num;

        if(_missileShotSettings[num]._animationTriggerName != null) {
            _playerAnimation.SetTrigger(_missileShotSettings[num]._animationTriggerName, _missileShotSettings[num]._animWaitTime);
        }        

        StartCoroutine(MultiTargetMissileInstantiate(reticles, num));

        _reticle.ChangerReticleImageAlpha(true);

        missileShotTimeRemain += _missileShotSettings[num]._missileShotDelay;

        Debug.Log(gameObject.name + "がミサイルを発射した");
    }

    private IEnumerator MultiTargetMissileInstantiate(LockedOnReticle[] reticles, int arynum)
    {

        if (reticles.Length >= _killCameraActiveCount) {
            KillCameraActiveProcess(transform);
        }

        int i = 0;
        int count = 0;
        bool isKillCamFollow;

        foreach (var tgt in reticles) {

            // リロード用
            if (_reloadFlags) {
                missileNum -= _consumptionPerShot;
                _reticle._missileGuage.fillAmount = (float)missileNum / _missileMaxNum;

            }

            count++;

            // リロード用
            if (_reloadFlags) {
                isKillCamFollow = count == reticles.Length || missileNum <= 0;
            } else {
                // 何回目に生成されたミサイル群にカメラが追従するか
                isKillCamFollow = count == _followKillCamMultiTargetMissilesCount - 1 && reticles.Length >= _killCameraActiveCount;
            }


            foreach (var hp in _missileShotSettings[arynum]._halfwayPoints) {
                StartCoroutine(MissileInstantiate(tgt.Target, i++, count, isKillCamFollow, arynum));
            }
            yield return new WaitForSeconds(_missileShotSettings[arynum]._multiTargetMissileDelay);
            i = 0;

            // リロード用
            if (_reloadFlags && missileNum <= 0) {
                yield break;
            }
        }

    }

    private IEnumerator MissileInstantiate(Transform target, int index, int count, bool isKillCamFollow, int arynum)
    {
        yield return new WaitForSeconds(_missileShotSettings[arynum]._instantiateTimes[index]);
        GameObject missile = MissileShot(target, _missileShotSettings[arynum]._halfwayPoints[index], _missileShotSettings[arynum]._impactTimes[index], index, arynum);

        if (isKillCamFollow && index == _followKillCamMissileNum - 1) {

            _killCamera.SetFollowMissile(missile);

        }
        
    }

    /// <summary>
    /// 複数呼び出すときのターゲット指定版
    /// </summary>
    /// <param name="target">ターゲットのTransform</param>
    /// <param name="halfwaypoint">ミサイルが一度通過する中間地点</param>
    /// <param name="impacttime">着弾するまでの時間</param>
    private GameObject MissileShot(Transform target, Transform halfwaypoint, float impacttime, int index, int arynum)
    {
        GameObject missile = Instantiate(_missilePrefab, _launchPoint.position, Quaternion.identity);
        HomingMissileScript homingMissileScript = missile.GetComponent<HomingMissileScript>();

        homingMissileScript.LaunchMissile(target, halfwaypoint, impacttime, _launchPoint.position - halfwaypoint.position, _missileShotSettings[arynum]._missileShotPower[index], _missileDamage);

        _soundPlayer.PlaySE(_se_MissileLaunch);

        return missile;
    }

    public void KillCameraActiveProcess(Transform targetAsteroid)
    {
        _killCamera.KillCameraActive(targetAsteroid);
        enabled = false;

    }

    private void GetTargetAsteroid_InRectVersion()
    {
        tmpTarget = null;
        
        // レティクルが一時的に操作不能になっているかロックオン数が最大値に達していたら新規でターゲットを探索しない
        if (_reticle.IsCanNotReticleMoveTime || _reticle.GeneratedReticleCount >= _reticle.GenerateReticleMax) {
            return;
        }

        targetAsteroidCollider = null;
        //Rect rect = _reticle.GetReticleRect();

        // 隕石コライダーがボタンレクト持ってなかったら飛ばす
        // ボタンレクトを取得して、レティクルの座標がその範囲内に入っているか確認する
        // 入っていたらエイムアシストオンにする
        // さらにraycastがあたったらターゲットにする
        foreach (var tarcol in RectInAsteroidContainer.Instance.targetColliders) {
            
            if (!tarcol.IsExistsButtonRect()) continue;
            if (tarcol.IsLockedOn) continue;
            
            Rect rect = tarcol.GetReticleRect();

            Vector3 reticleviewportPos = _mainCamera.ScreenToViewportPoint(_reticle.GetReticlePos());
            Vector3 tarcolviewportpos = _mainCamera.WorldToViewportPoint(tarcol.transform.position);

            // レティクルの真ん中の座標点が隕石のエイムアシスト範囲内かを判断します。
            if (rect.Contains(reticleviewportPos) && 0f < tarcolviewportpos.z) {

                Ray ray = new Ray(transform.position, tarcol.transform.position - transform.position);  // プレイヤーの位置から対象の位置までのレイです
                //Ray ray = _mainCamera.ScreenPointToRay(_reticle.GetReticlePos()); // レティクルの真ん中からそのワールド座標までのレイです

                // プレイヤーの位置から対象にレイキャストを飛ばし、
                //当たったコライダーのタグが隕石コライダー以外であれば、今回のループを飛ばして次の物の参照に行きます。
                if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, _laserLength, _layerMask) && !hit.transform.CompareTag("AsteroidTargetCollider")) {
                    continue;
                }
                Debug.DrawRay(ray.origin, ray.direction * _laserLength, Color.green);
                Debug.Log("照準内に隕石コライダー発見");

                // 隕石がすでにロックオンされていたら、エイムアシストの対象にしません。
                if (!tarcol.IsLockedOn) {
                    tmpTarget = tarcol.transform;
                }
                
                rect = _reticle.GetReticleRect();

                // 隕石コライダーの真ん中の座標点がレティクルの画像の範囲内かを判断します。
                if (rect.Contains(tarcolviewportpos) && 0f < tarcolviewportpos.z) {
                    // ここまで来たら、プレイヤーが狙っているものとし、ターゲットに設定します。
                    targetAsteroidCollider = tarcol.transform;
                    tmpTarget = null;

                    // ターゲットがラスボス撃破演出シーン遷移用のターゲットなら
                    // 最後の一撃のエフェクトをアクティブにする
                    if (tarcol.IsFinalExcutionTgt) {
                        _finalRockOnEffectSc.StartFinalTgtRockOnEffect(tarcol.transform);
                        _reticle.IsFinalLockOn = true;
                    }

                    // それがターゲットだと確定したので、処理を抜けます。
                    return;
                }
            }
        }        
    }

    private void Dbg()
    {
        if (targetAsteroidCollider) {
            _dbg_targetAsteroid = targetAsteroidCollider.root.name;
        } else {
            _dbg_targetAsteroid = "Null";
        }

        if (tmpTarget) {
            _dbg_tmptarget = tmpTarget.root.name;
        } else {
            _dbg_tmptarget = "Null";
        }

    }
}
