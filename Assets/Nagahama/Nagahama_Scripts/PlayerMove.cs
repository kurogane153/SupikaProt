using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// プレイヤー移動処理
public class PlayerMove : MonoBehaviour
{
    public enum OrbitOriginPlanet
    {
        Earth, Colony
    }

    [SerializeField] private PlayerAnimation _playerAnimation;
    [SerializeField] private CameraController _mainCameraController;
    [SerializeField] private RadialBlur _mainCameraRadialBlur;
    [SerializeField] private BoostEffect _boostEffect;
    [SerializeField] private OrbitGuideLightScript _orbitGuideLight;

    [Header("サウンド系")]
    [SerializeField] private SoundPlayer _soundPlayer;
    [SerializeField] private AudioClip _se_SpeedUp;
    [SerializeField] private AudioClip _se_SpeedUpNormal;
    [SerializeField] private AudioClip _se_SpeedDown;
    [SerializeField] private AudioClip _se_OrbitChangeAccept;

    [Space(10)]
    [SerializeField, Tooltip("地球")] private Transform _earthTransform;
    [SerializeField, Tooltip("コロニー")] private Transform _colonyTransform;
    [SerializeField, Tooltip("軌道の中心の惑星")] private OrbitOriginPlanet _enum_orbitOriginPlanet;
    [SerializeField, Tooltip("地球の外周を移動するときの回転軸")] private Vector3 _rotateEarthAxis;
    [SerializeField, Tooltip("コロニーの外周を移動するときの回転軸")] private Vector3 _rotateSpicaAxis;

    [Header("速度操作"), Space(10)]
    [SerializeField] private string _speedUpButtonName = "SpeedUp";
    [SerializeField] private string _speedDownButtonName = "SpeedDown";
    [SerializeField] private float[] _maxDistances;
    [SerializeField] private float[] _minDistances;
    [SerializeField] private float _startPeriod = 10f;

    [Header("各段階での移動速度。数字が小さいほど速い"), Space(10)]
    [SerializeField] private float[] _periods;
    [SerializeField] private float _speedChangeDelay = 1.23f;
    [SerializeField] private bool _isUpdateRotation = false;

    [Header("ラグランジュ"), Space(10)]
    [SerializeField] private TooltipScript _orbitShiftTooltip;
    [SerializeField] private string _orbitOriginChangeButtonName = "OrbitOriginChange";
    [SerializeField] private float[] _orbitOriginChangeCanMinAngle;
    [SerializeField] private float[] _orbitOriginChangeCanMaxAngle;
    [SerializeField] private float _lagrangePointAngle;

    private Transform orbitOrigin;
    private Vector3 rotateAxis;
    private Quaternion angleAxis;
    private float nowAngle;
    private float beforeAngle;
    private bool isAcceptedOrbitChange;
    private float speedChangeTimeRemain;
    private float period;
    private int periodNum;

    public OrbitOriginPlanet OriginPlanet
    {
        get { return _enum_orbitOriginPlanet; }
    }

    #region デバッグ用変数
    [Watch, HideInInspector] public float _dbg_playerNowAngle;
    [Watch, HideInInspector] public bool _dbg_isAcceptedOrbitChange;
    [Watch, HideInInspector] public string _dbg_orbitOriginPlanet;
    [Watch, HideInInspector] public float _dbg_distance_player_to_planet;
    #endregion

    private void Awake()
    {
        period = _startPeriod;
    }

    void Start()
    {
        if (_playerAnimation == null) {
            _playerAnimation = GetComponentInChildren<PlayerAnimation>();
            Debug.Log(gameObject.name + "がPlayerAnimationをGetComponentInChildrenで取得した");
        }

        if (_earthTransform == null) {
            _earthTransform = GameObject.Find("Earth").GetComponent<Transform>();
            Debug.Log(gameObject.name + "がEarthをFindで取得した");
        }

        if (_colonyTransform == null) {
            _colonyTransform = GameObject.Find("Spica").GetComponent<Transform>();
            Debug.Log(gameObject.name + "がSpicaをFindで取得した");
        }

        if (_mainCameraController == null) {
            _mainCameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
            Debug.Log(gameObject.name + "が_mainCameraControllerをFindで取得した");
        }

        if (_mainCameraRadialBlur == null) {
            _mainCameraRadialBlur = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<RadialBlur>();
            Debug.Log(gameObject.name + "が_mainCameraRadialBlurをFindで取得した");
        }

        if (_boostEffect == null) {
            _boostEffect = GetComponentInChildren<BoostEffect>();
            Debug.Log(gameObject.name + "がBoostEffectをGetComponentInChildrenで取得した");
        }

        if (_soundPlayer == null) {
            if ((_soundPlayer = GetComponentInChildren<SoundPlayer>()) == null) {
                Debug.LogError(gameObject.name + "の" + nameof(_soundPlayer) + "が空です");
            }
            Debug.Log(gameObject.name + "は、子要素にアタッチされているAudioSourceを自動的に" + nameof(_soundPlayer) + "にアタッチしました");
        }

        OrbitVarChange();
        
    }

    void Update()
    {
        SpeedControllInput();       // 速度変更操作を受け付けます
        AcceptOrbitOriginChange();  // 軌道変更操作を受け付けます

        MoveMent();                 // プレイヤーの移動処理
        UpdateNowAngle();           // プレイヤーの回転処理

        FixDistance();              // 惑星の原点との距離が設定した値以上、また以下のとき、設定した値まで戻します
        OrbitShift();               // 軌道を変更します

        Dbg();                      // デバッグ用変数表示を更新します
    }

    private void FixedUpdate()
    {
        TimeRemainManege();         // クールタイムなどが0より上の数値なら減らします
    }

    private void TimeRemainManege()
    {
        if (0f < speedChangeTimeRemain) {
            speedChangeTimeRemain -= Time.deltaTime;
        }
    }

    private void SpeedControllInput()
    {
        if (speedChangeTimeRemain <= 0f && Input.GetButtonDown(_speedUpButtonName)) {
            SpeedUp();
            Debug.Log(gameObject.name + "が速度を上げた。現在" + period);
        }

        if (speedChangeTimeRemain <= 0f && Input.GetButtonDown(_speedDownButtonName)) {
            SpeedDown();
            Debug.Log(gameObject.name + "が速度を下げた。現在" + period);
        }
    }

    private void MoveMent()
    {
        beforeAngle = nowAngle;

        angleAxis = Quaternion.AngleAxis(360 / period * Time.deltaTime, rotateAxis);
        Vector3 newPos = transform.position;

        newPos -= orbitOrigin.position;
        newPos = angleAxis * newPos;
        newPos += orbitOrigin.position;

        transform.position = newPos;

        if (_isUpdateRotation) {
            transform.rotation = transform.rotation * angleAxis;
        }

    }

    private void FixDistance()
    {
        float distance_player_from_planet = (transform.position - orbitOrigin.position).magnitude;

        if(_maxDistances[(int)_enum_orbitOriginPlanet] < distance_player_from_planet) {
            transform.position = Vector3.MoveTowards(transform.position, (transform.position - orbitOrigin.position).normalized * _maxDistances[(int)_enum_orbitOriginPlanet], Time.deltaTime * 5f);
        } else if (distance_player_from_planet < _minDistances[(int)_enum_orbitOriginPlanet]) {
            transform.position = Vector3.MoveTowards(transform.position, (transform.position - orbitOrigin.position).normalized * _minDistances[(int)_enum_orbitOriginPlanet], Time.deltaTime * 5f);
        }
    }

    private void UpdateNowAngle()
    {
        Vector3 vec = transform.position - orbitOrigin.position;
        float rad = Mathf.Atan2(vec.x, vec.z);
        nowAngle = rad * Mathf.Rad2Deg;

        if(rotateAxis == _rotateSpicaAxis) {
            nowAngle -= 180;
        }

        if (nowAngle < 0) {
            nowAngle += 360;
        }

    }

    /// <summary>
    /// 軌道原点を変更したいという操作を受け付けておく。
    /// 対応するボタンを押していて、変更可能な角度に来ているときに、操作を受け付けてあげる。
    /// </summary>
    private void AcceptOrbitOriginChange()
    {
        // ラグランジュポイント変更可能範囲に入ってるときかつ変更操作がまだのとき
        // 軌道変更操作ヘルプを表示し、軌道変更ガイドライトエフェクトをアクティブにします
        if(_orbitOriginChangeCanMinAngle[(int)_enum_orbitOriginPlanet] < nowAngle && nowAngle < _orbitOriginChangeCanMaxAngle[(int)_enum_orbitOriginPlanet] &&
            !isAcceptedOrbitChange) {


            _orbitGuideLight.enabled = true;
            _orbitShiftTooltip.SetTooltipActive(true , true);


        // ラグランジュポイント変更可能範囲に入ってるときかつ変更操作を受け付けた状態のとき
        // 軌道変更をキャンセルするヘルプを表示します
        } else if(_orbitOriginChangeCanMinAngle[(int)_enum_orbitOriginPlanet] < nowAngle && nowAngle < _orbitOriginChangeCanMaxAngle[(int)_enum_orbitOriginPlanet] &&
            isAcceptedOrbitChange) {


             _orbitGuideLight.enabled = true;
            _orbitShiftTooltip.SetTooltipActive(false, true);


        // 上のどれにも当てはまらないとき
        // 軌道変更ヘルプを非表示にし、軌道変更ガイドライトエフェクトを非アクティブにします
        } else {
            _orbitGuideLight.enabled = false;
            _orbitShiftTooltip.SetTooltipActive(false);
        }

        // 軌道変更ボタンを押したとき、
        // ラグランジュポイント変更可能範囲に入ってるときかつ変更操作がまだなら
        // 軌道変更を受け付けて、軌道変更ガイドライトエフェクトにもそれを知らせます
        if (Input.GetButtonDown(_orbitOriginChangeButtonName) && 
            _orbitOriginChangeCanMinAngle[(int)_enum_orbitOriginPlanet] < nowAngle && nowAngle < _orbitOriginChangeCanMaxAngle[(int)_enum_orbitOriginPlanet] &&
            !isAcceptedOrbitChange) {


            _soundPlayer.PlaySE(_se_OrbitChangeAccept);
            _orbitGuideLight.OrbitGuideStatusChange(true);
            isAcceptedOrbitChange = true;


        // 軌道変更ボタンを押したとき、
        // ラグランジュポイント変更可能範囲に入ってるときかつ変更操作を受け付けた状態なら
        // 軌道変更をキャンセルします。
        } else if (Input.GetButtonDown(_orbitOriginChangeButtonName) &&
            _orbitOriginChangeCanMinAngle[(int)_enum_orbitOriginPlanet] < nowAngle && nowAngle < _orbitOriginChangeCanMaxAngle[(int)_enum_orbitOriginPlanet] &&
            isAcceptedOrbitChange) {


            _orbitGuideLight.OrbitGuideStatusChange(false);
            isAcceptedOrbitChange = false;
        }
    }

    /// <summary>
    /// 現在の軌道原点が地球ならスピカに、コロニーなら地球に変更する。
    /// ここでは変更したいという操作だけ受け付けておき、実際に変更するのは
    /// ラグランジュポイントに到達してから。
    /// </summary>
    private void OrbitOriginSwitch()
    {
        switch (_enum_orbitOriginPlanet) {
            case OrbitOriginPlanet.Earth:
                _enum_orbitOriginPlanet = OrbitOriginPlanet.Colony;
                break;
            case OrbitOriginPlanet.Colony:
                _enum_orbitOriginPlanet = OrbitOriginPlanet.Earth;
                break;
            default:
                _enum_orbitOriginPlanet = OrbitOriginPlanet.Earth;
                break;
        }
        
    }

    /// <summary>
    /// 変更したい操作をしたことがあり、ラグランジュポイントに到達していたら、
    /// 軌道の原点を変更する。
    /// </summary>
    private void OrbitShift()
    {
        
        // どちらの惑星が軌道の原点かで処理を分けています
        // 軌道変更操作を受けてつけていない、受け付けているがラグランジュポイントに到達していないときは
        // returnで抜けています
        switch (_enum_orbitOriginPlanet) {
            case OrbitOriginPlanet.Earth:
                if (!isAcceptedOrbitChange || !(beforeAngle <= _lagrangePointAngle && _lagrangePointAngle <= nowAngle)) return;
                break;

            case OrbitOriginPlanet.Colony:
                if (!isAcceptedOrbitChange || !(nowAngle <= _lagrangePointAngle && _lagrangePointAngle <= beforeAngle)) return;
                break;

            default:
                if (!isAcceptedOrbitChange || !(beforeAngle <= _lagrangePointAngle && _lagrangePointAngle <= nowAngle)) return;
                break;
        }

        // 軌道変更操作を受け付けており、ラグランジュポイントを通過したしたとき、以下の処理をします
        OrbitOriginSwitch();
        OrbitVarChange();

        if (speedChangeTimeRemain <= 0f) {
            _playerAnimation.ChangeOrbit(_speedChangeDelay);
            speedChangeTimeRemain = _speedChangeDelay;
        }

        isAcceptedOrbitChange = false;
    }

    /// <summary>
    /// 軌道系のパラメータを変更する。
    /// </summary>
    private void OrbitVarChange()
    {
        switch (_enum_orbitOriginPlanet) {
            case OrbitOriginPlanet.Earth:
                orbitOrigin = _earthTransform;
                rotateAxis = _rotateEarthAxis;
                break;

            case OrbitOriginPlanet.Colony:
                orbitOrigin = _colonyTransform;
                rotateAxis = _rotateSpicaAxis;
                break;

            default:
                orbitOrigin = _earthTransform;
                rotateAxis = _rotateEarthAxis;
                break;
        }

        _mainCameraController.SetOrbitShiftRotWaitTime();
    }

    private void SpeedUp()
    {
        int beforennum = periodNum;
        if (++periodNum >= _periods.Length) {
            periodNum = _periods.Length - 1;
        }

        if(beforennum != periodNum && periodNum == _periods.Length - 1) {
            _mainCameraRadialBlur.EnableRadialBlur();
            _boostEffect.PlayBoostParticles();
            _soundPlayer.PlaySE(_se_SpeedUp);
        } else {
            _soundPlayer.PlaySE(_se_SpeedUpNormal);
        }

        period = _periods[periodNum];
        _playerAnimation.Spiral(_speedChangeDelay);
        speedChangeTimeRemain = _speedChangeDelay;

        
    }

    private void SpeedDown()
    {
        int beforennum = periodNum;

        if (--periodNum < 0) {
            periodNum = 0;
        }

        if (beforennum != periodNum && beforennum == _periods.Length - 1) {
            _mainCameraRadialBlur.DisableRadialBlur();
            _boostEffect.StopBoostParticles();
        }

        period = _periods[periodNum];
        _playerAnimation.AcrobatLoop(_speedChangeDelay);
        speedChangeTimeRemain = _speedChangeDelay;

        _soundPlayer.PlaySE(_se_SpeedDown);
    }    

    private void Dbg()
    {
        _dbg_playerNowAngle = nowAngle;
        _dbg_isAcceptedOrbitChange = isAcceptedOrbitChange;
        _dbg_orbitOriginPlanet = _enum_orbitOriginPlanet.ToString();
        _dbg_distance_player_to_planet = (transform.position - orbitOrigin.position).magnitude;
    }

    private IEnumerator DelayChange_isAcceptedOrbitChange()
    {
        yield return new WaitForSeconds(1f);

        isAcceptedOrbitChange = false;
    }


}
