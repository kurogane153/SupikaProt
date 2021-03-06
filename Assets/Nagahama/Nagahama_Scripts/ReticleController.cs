﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReticleController : MonoBehaviour
{
    #region Singleton

    private static ReticleController instance;

    public static ReticleController Instance
    {
        get
        {
            if (instance == null) {
                instance = (ReticleController)FindObjectOfType(typeof(ReticleController));

                if (instance == null) {
                    Debug.LogError(typeof(ReticleController) + "is nothing");
                }
            }

            return instance;
        }
    }

    #endregion

    [Header("デバッグ用")]
    [SerializeField] public bool _debug;

    [Header("リロードゲージUI"), Space(10)]
    [SerializeField] public Image _missileGuage;

    [Header("サウンド系"), Space(10)]
    [SerializeField] private SoundPlayer _soundPlayer;
    [SerializeField] private AudioClip _se_LockOn;
    [SerializeField] private float[] _lockOnSEPitchs;

    [Space(10)]
    [SerializeField] private GameObject _lockedOnReticlePrefab;
    [SerializeField] private Button _firstSelectButton;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Color _lockOnColor;
    [SerializeField] private float _speedX = 1f;
    [SerializeField] private float _speedY = 1f;
    [SerializeField] private float _reticleRectSizeScaling = 1.2f;
    [SerializeField] private float _aimAssistDegreeX = 3f;
    [SerializeField] private float _aimAssistDegreeY = 3f;
    [SerializeField] private float _aimSpeedDecreaseTime = 0.16f;   // 隕石のロックオン時に一瞬だけ操作不能になる時間
    [SerializeField] private float _aimAssistIntensity = 10f;
    [SerializeField] private float _aimDeadZoneX = 0.2f;
    [SerializeField] private float _aimDeadZoneY = 0.2f;
    [SerializeField] private int _generateReticleMax = 3;

    [Header("スティック倒した方向に移動するエイムアシストジャッジレクト"), Space(10)]
    [SerializeField] private RectTransform _aimAssistJudgeRect;
    [SerializeField] private float _aimAssistJudgeInputIntensity;
    [SerializeField] private float _judgeRectSizeScaling;
    private float judgeRectWidth;
    private float judgeRectHeight;

    [Header("ロックオンボタンを押してロックオンする"), Space(10)]
    [SerializeField] public bool _lockOnTargetOnLockOnButtonDown;

    [Header("無操作時ターゲットを追跡する"), Space(10)]
    [SerializeField] public bool _targetLockonmoveFlags;

    [Header("Unityのボタン選択の挙動を利用したロックオンシステム"), Space(10)]
    [SerializeField] public bool _userSuperAimAssistSystemFlags;

    [Header("隕石の周辺に照準が来たときプレイヤー入力を減らし、隕石の方向へ移動させる")]
    [SerializeField] public bool _useSoftAimAssistFlags;

    private Canvas canvas;
    private RectTransform canvasRectTransform;
    private RectTransform rectTransform;
    private Image image;
    private Image[] chiledImages;
    private Color defaultColor;
    private float canNotReticleMoveTime;
    private bool isBeforeTargeting;
    private int generatedReticleCount;
    private bool isFinalLockOn;

    private float startSpeedX;
    private float startSpeedY;
    private float startDegreeX;
    private float startDegreeY;
    private float startAssistIntensity;
    

    #region GetterandSetter
    public int GeneratedReticleCount
    {
        get { return generatedReticleCount; }

        set { generatedReticleCount = value; 
            if(generatedReticleCount < 0) {
                generatedReticleCount = 0;
            }
            LockedOnCountMarkScript.LockedOnCountCheck();
        }
    }

    public bool IsCanNotReticleMoveTime
    {
        get { return 0 < canNotReticleMoveTime; }
    }

    public int GenerateReticleMax
    {
        get { return _generateReticleMax; }
    }

    public Canvas GetCanvas()
    {
        return canvas;
    }

    public bool IsFinalLockOn
    {
        get { return isFinalLockOn; }
        set { isFinalLockOn = value; }
    }

    #endregion

    public void SelectFirstButton()
    {
        _firstSelectButton.Select();
    }

    /// <summary>
    /// レティクル画像を半透明にするか不透明に戻すか指定します
    /// </summary>
    /// <param name="flag">レティクル画像を半透明にするか不透明に戻すか</param>
    public void ChangerReticleImageAlpha(bool flag)
    {
        if (flag) {
            image.color -= new Color(0, 0, 0, 0.5f);
            
            foreach(var im in chiledImages) {
                im.color -= new Color(0, 0, 0, 0.5f);
            }
        } else {
            image.color += new Color(0, 0, 0, 0.5f);

            foreach (var im in chiledImages) {
                im.color += new Color(0, 0, 0, 0.5f);
            }
        }
    }

    private void Awake()
    {
        if (this != Instance) {
            Destroy(this.gameObject);
            return;
        }
        startSpeedX = _speedX;
        startSpeedY = _speedY;
        startDegreeX = _aimAssistDegreeX;
        startDegreeY = _aimAssistDegreeY;
        startAssistIntensity = _aimAssistIntensity;
    }

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        defaultColor = image.color;
        chiledImages = GetComponentsInChildren<Image>();

        if (_soundPlayer == null) {
            if ((_soundPlayer = GetComponentInChildren<SoundPlayer>()) == null) {
                Debug.LogError(gameObject.name + "の" + nameof(_soundPlayer) + "が空です");
            }
            Debug.Log(gameObject.name + "は、子要素にアタッチされているAudioSourceを自動的に" + nameof(_soundPlayer) + "にアタッチしました");
        }

        if (_mainCamera == null) {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            Debug.Log(gameObject.name + "が_mainCameraをFindで取得した");
        }

        judgeRectWidth = _aimAssistJudgeRect.rect.x;
        judgeRectHeight = _aimAssistJudgeRect.rect.y;

        OptionDataManagerScript.Instance.optionValueChanges.AddListener(ChangeReticleControllerValues);
        ChangeReticleControllerValues();

        
    }

    private void ChangeReticleControllerValues()
    {
        _speedX = startSpeedX * OptionDataManagerScript.Instance.optionData._aimSensivity_X;
        _speedY = startSpeedY * OptionDataManagerScript.Instance.optionData._aimSensivity_Y;
        _aimAssistDegreeX = startDegreeX * OptionDataManagerScript.Instance.optionData._aimAssistIntensity;
        _aimAssistDegreeY = startDegreeY * OptionDataManagerScript.Instance.optionData._aimAssistIntensity;
        _aimAssistIntensity = startAssistIntensity * OptionDataManagerScript.Instance.optionData._aimAssistIntensity;
        _useSoftAimAssistFlags = OptionDataManagerScript.Instance.optionData._useAimAssistFlag;
        Debug.Log("レティクル処理完了");
    }

    private void OnDestroy()
    {
        if (OptionDataManagerScript.Instance != null) {
            OptionDataManagerScript.Instance.optionValueChanges.RemoveListener(ChangeReticleControllerValues);
        }
            
    }

    void Update()
    {
        #region デバッグ用
        Rect rect = GetReticleRect();
        if (Input.GetKeyDown(KeyCode.P)) {
            // Debug.Log("<color=yellow>GetReticlePos : " + _mainCamera.ScreenToViewportPoint(GetReticlePos()) + "</color>");
        }

        // Lトリガー押しながら
        if (_debug && Input.GetAxis("L_R_Trigger") >= 0.5f && Input.GetButtonDown("LockOn")) {
            _lockOnTargetOnLockOnButtonDown = !_lockOnTargetOnLockOnButtonDown;
        }

        // Rトリガー押しながら
        if (_debug && Input.GetAxis("L_R_Trigger") <= -0.5f && Input.GetButtonDown("LockOn")) {
            _targetLockonmoveFlags = !_targetLockonmoveFlags;
        }

        // Rトリガー押しながら
        if (_debug && Input.GetAxis("L_R_Trigger") <= -0.5f && Input.GetButtonDown("OrbitOriginChange")) {
            _userSuperAimAssistSystemFlags = !_userSuperAimAssistSystemFlags;
        }

        #endregion

    }

    private void FixedUpdate()
    {
        TimeRemainManege();
    }

    private void TimeRemainManege()
    {
        // 照準操作が一瞬だけできなくなる時間
        if(0 < canNotReticleMoveTime) {
            canNotReticleMoveTime -= Time.deltaTime;
            if(canNotReticleMoveTime <= 0) {
                
            }
        }
    }

    public void MoveReticle(float x, float y, Transform target = null, Transform aimassisttarget = null)
    {   
       
        if (0 < canNotReticleMoveTime) {
            x = 0;
            y = 0;
        }

        MoveAimAssistJudgeRect(x, y);

        if ((0 < canNotReticleMoveTime &&_targetLockonmoveFlags && target && Mathf.Abs(x) < _aimDeadZoneX && Mathf.Abs(y) < _aimDeadZoneY) || isFinalLockOn) {
            TargetLockOnMove(target);
        }
        
        Vector3 newvec = Vector3.zero;
        bool isAimAssistTargetInJudgeRect = false;

        if (aimassisttarget) {
            Vector3 tarcolviewportpos = _mainCamera.WorldToViewportPoint(aimassisttarget.position);
            Rect rect = GetAimAssistJudgeRectRect();
            isAimAssistTargetInJudgeRect = rect.Contains(tarcolviewportpos);
        }

        Debug.Log(isAimAssistTargetInJudgeRect);
       
        if (_useSoftAimAssistFlags && isAimAssistTargetInJudgeRect && aimassisttarget && (Mathf.Abs(x) > _aimDeadZoneX || Mathf.Abs(y) > _aimDeadZoneY)) {
            newvec = _mainCamera.WorldToScreenPoint(aimassisttarget.position) - rectTransform.position;
            newvec.z = 0;
            newvec = newvec.normalized;

        }

        float newX = x * _speedX;
        float newY = y * _speedY;
        bool isNowTargeting = false;

        if (_useSoftAimAssistFlags && isAimAssistTargetInJudgeRect && aimassisttarget) {
            newX /= _aimAssistDegreeX;
            newY /= _aimAssistDegreeY;
        }

        Vector3 movePos = new Vector3(newX, newY);

        if (!IsNewScreenPositionScreenOutsite(rectTransform.position + movePos + newvec * _aimAssistIntensity)) {
            rectTransform.position += movePos + newvec * _aimAssistIntensity;
        }

        if (target) {
            image.color = _lockOnColor;

            if (_lockOnTargetOnLockOnButtonDown && Input.GetButtonDown("LockOn")) {
                isNowTargeting = true;
            } else if(!_lockOnTargetOnLockOnButtonDown){
                isNowTargeting = true;
            }            

            if(isNowTargeting && !isBeforeTargeting) {
                TargetCollider targetCollider = target.GetComponent<TargetCollider>();

                if (!targetCollider.IsLockedOn && generatedReticleCount < _generateReticleMax ) {
                    targetCollider.IsLockedOn = true;

                    GameObject newLockonReticle = Instantiate(_lockedOnReticlePrefab, transform.position, Quaternion.identity);
                    LockedOnReticle lockedOnReticle = newLockonReticle.GetComponent<LockedOnReticle>();

                    lockedOnReticle.InstantiateSettings(canvas, target, _mainCamera);
                    GeneratedReticleCount += 1;

                    canNotReticleMoveTime = _aimSpeedDecreaseTime;

                    _soundPlayer.ChangePitchLevel(_lockOnSEPitchs[GeneratedReticleCount - 1]);
                    _soundPlayer.PlaySE(_se_LockOn);
                }
            }

        } else {
            image.color = defaultColor;
            isNowTargeting = false;
        }

        isBeforeTargeting = isNowTargeting;

    }

    public void SetTarget(Transform target)
    {
        TargetCollider targetCollider = target.GetComponent<TargetCollider>();

        if (!targetCollider.IsLockedOn && generatedReticleCount < _generateReticleMax) {
            targetCollider.IsLockedOn = true;

            GameObject newLockonReticle = Instantiate(_lockedOnReticlePrefab, transform.position, Quaternion.identity);
            LockedOnReticle lockedOnReticle = newLockonReticle.GetComponent<LockedOnReticle>();

            lockedOnReticle.InstantiateSettings(canvas, target, _mainCamera);
            GeneratedReticleCount += 1;

            _soundPlayer.PlaySE(_se_LockOn);
        }

        TargetLockOnMove(target);
    }

    private bool IsNewScreenPositionScreenOutsite(Vector3 pos)
    {
        Rect rect = new Rect(0, 0, 1, 1);

        if (rect.Contains(_mainCamera.ScreenToViewportPoint(pos))){
            return false;
        }
        return true;
    }

    private void TargetLockOnMove(Transform target)
    {
        Vector3 newPoint = _mainCamera.WorldToScreenPoint(target.position);
        Rect rect = new Rect(0, 0, 1, 1);

        if (rect.Contains(_mainCamera.WorldToViewportPoint(target.position))) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, newPoint, canvas.worldCamera, out Vector2 newPos);
            rectTransform.localPosition = newPos;
        }
        
    }

    public Vector2 GetReticlePos()
    {
        return RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, rectTransform.position);
    }

    public Rect GetReticleRect()
    {
        Vector2 rectsize = rectTransform.rect.size;
        Vector2 screensize = new Vector2(Screen.width, Screen.height);

        Vector2 size = Vector2.Scale(rectsize / screensize, rectTransform.localScale);
        Rect rect = new Rect((Vector2)(rectTransform.position / screensize) - (size * 0.5f), size * _reticleRectSizeScaling);

        return rect;
    }

    private void MoveAimAssistJudgeRect(float horizontal, float vertical)
    {
        float newX = judgeRectWidth * -horizontal * _aimAssistJudgeInputIntensity;
        float newY = judgeRectHeight * -vertical * _aimAssistJudgeInputIntensity;
        Vector3 newPos = new Vector3(newX, newY);

        _aimAssistJudgeRect.position = transform.position + newPos;
    }

    private Rect GetAimAssistJudgeRectRect()
    {
        Vector2 rectsize = _aimAssistJudgeRect.rect.size;
        Vector2 screensize = new Vector2(Screen.width, Screen.height);

        Vector2 size = Vector2.Scale(rectsize / screensize, _aimAssistJudgeRect.localScale);
        Rect rect = new Rect((Vector2)(_aimAssistJudgeRect.position / screensize) - (size * 0.5f), size * _judgeRectSizeScaling);

        return rect;
    }
}
