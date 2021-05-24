using System.Collections;
using UnityEngine;

public class KillCameraScript : MonoBehaviour
{
    private enum CameraType
    {
        KillCamera,
        ConflictEventCamera,
    }
    [SerializeField] private CameraType _cameraType;

    [SerializeField] private Transform _followTarget;
    [SerializeField] private Vector3 _positionOffset = new Vector3(0, 1, 0);
    [SerializeField] private Vector3 _rotationOffset = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 _missileFollowOffset = new Vector3(0, 5, -10);
    [SerializeField, Tooltip("隕石の近くから自機を映すときのオフセット")] private Vector3 _playerNearPosOffset;
    [SerializeField] private float _missileFollowXRotate = 15f;
    [SerializeField] private float _cameraPanSpeed = 20f;
    [SerializeField, Tooltip("プレイヤーの近くに位置を変えるまでの時間")] private float _warpTimeofMovetoPlayerNearPos = 0.5f;
    [SerializeField] private float _followModeSwitchDelay = 0.75f;
    [SerializeField, Tooltip("キルカメラ時ミサイルが消えてからキルカメラ終了までの待機時間")] private float _killCamResetDelay;
    [SerializeField, Tooltip("衝突イベントカメラ終了までの待機時間")] private float _conflictEventCamResetDelay;
    [SerializeField] private float _normalMoveLerpFactor = 6;
    [SerializeField, Range(0.0f, 1.0f)] private float _followMissileLerpFactor = 0.3f;
    [SerializeField] private float _showExplosionModeSwitchDelay = 1f;
    [SerializeField, Range(0.0f, 1.0f)] private float _showExplosionZoomOutLerpFactor = 0.3f;

    [SerializeField] private PlayerMove _playerMove;
    [SerializeField] private PlayerShot _playerShot;
    [SerializeField] private Camera _mainCamera;

    [Space(10)]
    [SerializeField] private TooltipScript _conflictTooltip;
    [SerializeField] private TooltipScript _orbitShiftTooltip;

    private Transform startFollowTarget;
    private Camera camera;

    public Camera GetCamera()
    {
        if(camera == null) {
            camera = GetComponent<Camera>();
        }
        return camera;
    }

    private enum StagingPhase
    {
        Yet,
        Pan,
        LookAtMissile,
        FollowMissile,
        ShowExplosion
    }
    private StagingPhase stagingPhase = StagingPhase.Yet;

    private enum ConflictStagingPhase
    {
        Yet,
        ShowExplosion
    }
    private ConflictStagingPhase conflictStagingPhase = ConflictStagingPhase.Yet;

    void Start()
    {
        if (_followTarget == null) {
            _followTarget = GameObject.Find("Player").GetComponent<Transform>();
            Debug.Log(gameObject.name + "が_followTargetをFindで取得した");
        }

        if (_playerMove == null) {
            _playerMove = GameObject.Find("Player").GetComponent<PlayerMove>();
            Debug.Log(gameObject.name + "が_playerMoveをFindで取得した");
        }

        if(_mainCamera == null) {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            Debug.Log(gameObject.name + "が_mainCameraをFindで取得した");
        }

        camera = GetComponent<Camera>();

        startFollowTarget = _followTarget;
    }

    void Update()
    {
        
    }

    private void NormalMove()
    {
        Vector3 offset = _followTarget.right * _positionOffset.x + _followTarget.up * _positionOffset.y + _followTarget.forward * _positionOffset.z;

        Vector3 desiredPosition = _followTarget.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * _normalMoveLerpFactor);

        Vector3 desiredRotation = _followTarget.rotation.eulerAngles + _rotationOffset;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(desiredRotation), Time.deltaTime * _normalMoveLerpFactor);
    }

    private void StopPhase_CameraPanMove()
    {
        transform.Translate(-Vector3.forward * Time.deltaTime * _cameraPanSpeed);
    }

    private void StopPhase_CameraZoomMove()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * _cameraPanSpeed);
    }

    private void FollowTargetMove()
    {
        if(_followTarget == null) {
            StartCoroutine(nameof(SwitchStagingPhase_ShowExplosion));
            return;
        }
        Vector3 desiredPosition = _followTarget.position + _missileFollowOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, _followMissileLerpFactor);
    }

    private void WarpTargetBehind()
    {

        transform.rotation = _followTarget.rotation;
        transform.Rotate(_missileFollowXRotate, 180, 0);

        Vector3 desiredPosition = _followTarget.position + _missileFollowOffset;
        transform.position = desiredPosition;

    }

    public Vector3 Focus(Vector3[] points)
    {
        // 全点をビュー空間に移動
        var toView = transform.worldToLocalMatrix;
        for (int i = 0; i < points.Length; i++) {
            points[i] = toView.MultiplyPoint3x4(points[i]);
        }

        // 各種傾き
        var ay1 = Mathf.Tan(camera.fieldOfView * _showExplosionZoomOutLerpFactor * Mathf.Deg2Rad);
        var ay0 = -ay1;
        var ax1 = ay1 * camera.aspect;
        var ax0 = -ax1;

        // XY軸で最小最大を取って不要データを捨てる
        var y0Min = float.MaxValue;
        var y1Max = -float.MaxValue;
        var x0Min = float.MaxValue;
        var x1Max = -float.MaxValue;

        for (int i = 0; i<points.Length; i++)
        {
            var p = points[i];
            var by0 = p.y - (ay0 * p.z);
            var by1 = p.y - (ay1 * p.z);
            var bx0 = p.x - (ax0 * p.z);
            var bx1 = p.x - (ax1 * p.z);
            y0Min = Mathf.Min(y0Min, by0);
            y1Max = Mathf.Max(y1Max, by1);
            x0Min = Mathf.Min(x0Min, bx0);
            x1Max = Mathf.Max(x1Max, bx1);
        }

        // zを2つ求め、小さい方を採用する。x,yはそのまま使う
        var zy = (y1Max - y0Min) / (ay0 - ay1);
        var y = y0Min + (ay0 * zy);
        var zx = (x1Max - x0Min) / (ax0 - ax1);
        var x = x0Min + (ax0 * zx);
        var posInView = new Vector3(x, y, Mathf.Min(zy, zx));

        // ワールドに戻す
        return transform.localToWorldMatrix.MultiplyPoint3x4(posInView);
    }

    private void LookAtTarget()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((_followTarget.position - transform.position).normalized), Time.deltaTime * _normalMoveLerpFactor);
    }

    private void LateUpdate()
    {

        if (_cameraType == CameraType.KillCamera) {
            KillCameraProcess();
            if (Input.GetKeyDown(KeyCode.B)) {
                Debug.Log(ReticleController.Instance.GeneratedReticleCount);
            }
           
        } else if (_cameraType == CameraType.ConflictEventCamera) {
            ConflictEventCameraProcess();
        }
        
    }

    private void KillCameraProcess()
    {
        switch (stagingPhase) {
            case StagingPhase.Yet:
                NormalMove();
                break;

            case StagingPhase.Pan:
                StopPhase_CameraZoomMove();
                break;

            case StagingPhase.LookAtMissile:
                StopPhase_CameraZoomMove();
                break;

            case StagingPhase.FollowMissile:
                FollowTargetMove();
                break;

            case StagingPhase.ShowExplosion:
                StopPhase_CameraPanMove();
                break;
        }
    }

    private void ConflictEventCameraProcess()
    {
        switch (conflictStagingPhase) {
            case ConflictStagingPhase.Yet:
                break;
            case ConflictStagingPhase.ShowExplosion:
                StopPhase_CameraPanMove();
                break;
        }
    }

    #region キルカメラ

    public void SetFollowMissile(GameObject target)
    {
        _followTarget = target.transform;
        stagingPhase = StagingPhase.LookAtMissile;
        StartCoroutine(nameof(SwitchStagingPhase_FollowMode));
    }

    public void SwitchStagingPhase_Pan()
    {
        stagingPhase = StagingPhase.Pan;
    }

    private IEnumerator WarpToPlayerNearPosition(Transform target)
    {
        yield return new WaitForSeconds(_warpTimeofMovetoPlayerNearPos);

        transform.position = target.position;
        transform.LookAt(_playerMove.transform);
        transform.localPosition = transform.localPosition + _playerNearPosOffset;
        transform.LookAt(_playerMove.transform);
    }

    private IEnumerator SwitchStagingPhase_FollowMode()
    {
        yield return new WaitForSeconds(_followModeSwitchDelay);
        stagingPhase = StagingPhase.FollowMissile;
        WarpTargetBehind();

        yield return new WaitForSeconds(_showExplosionModeSwitchDelay);
        StartCoroutine(nameof(SwitchStagingPhase_ShowExplosion));

        LockedOnReticle[] reticles = FindObjectsOfType<LockedOnReticle>();
        Vector3[] points = new Vector3[reticles.Length];
        Debug.Log(reticles.Length);
        int i = 0;

        foreach(var point in reticles) {
            points[i++] = point.Target.position;
        }

        transform.position = _mainCamera.transform.position;
        transform.position = Focus(points);

    }

    private IEnumerator SwitchStagingPhase_ShowExplosion()
    {
        stagingPhase = StagingPhase.ShowExplosion;

        while(ReticleController.Instance.GeneratedReticleCount > 0) {
            yield return new WaitForSeconds(0);
        }

        yield return new WaitForSeconds(_killCamResetDelay);
        Reset();
    }

    public void KillCameraActive(Transform targetAsteroid)
    {
        GetCamera().enabled = true;
        SwitchStagingPhase_Pan();
        StartCoroutine(WarpToPlayerNearPosition(targetAsteroid));
        _playerMove.enabled = false;
        ReticleController.Instance.GetCanvas().enabled = false;
        Pauser.SoftPause();
        Time.timeScale = 1f;
        transform.position = _mainCamera.transform.position;
        transform.rotation = _mainCamera.transform.rotation;

    }

    public void Reset()
    {
        camera.enabled = false;
        _followTarget = startFollowTarget;
        stagingPhase = StagingPhase.Yet;
        _playerMove.enabled = true;
        ReticleController.Instance.GetCanvas().enabled = true;
        Pauser.SoftResume();
        Time.timeScale = 1f;
        _playerShot.enabled = true;
    }

    #endregion

    #region 惑星衝突イベント

    private IEnumerator ConflictSwitchStagingPhase_ShowExplosion()
    {
        _orbitShiftTooltip.SetTooltipActive(false , false);
        conflictStagingPhase = ConflictStagingPhase.ShowExplosion;
        yield return new WaitForSeconds(0.2f);

        _conflictTooltip.SetTooltipActive(true , false);

        yield return new WaitForSeconds(_conflictEventCamResetDelay);
        CFCameraReset();
    }

    public void ConflictEventCameraActive(GameObject target, Vector3 pos, float waittime = 0f)
    {
        if(0f < waittime) {
            _conflictEventCamResetDelay = waittime;
        }
        transform.position = pos;
        _followTarget = target.transform;
        GetCamera().enabled = true;
        
        _playerMove.enabled = false;
        _playerShot.enabled = false;
        ReticleController.Instance.gameObject.SetActive(false);
        Pauser.SoftPause();
        Time.timeScale = 1f;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((_followTarget.position - transform.position).normalized), 1);

        StartCoroutine(nameof(ConflictSwitchStagingPhase_ShowExplosion));
    }

    public void CFCameraReset()
    {
        camera.enabled = false;
        _followTarget = startFollowTarget;
        conflictStagingPhase = ConflictStagingPhase.Yet;

        _playerMove.enabled = true;
        _playerShot.enabled = true;
        ReticleController.Instance.gameObject.SetActive(true);
        Pauser.SoftResume();
        Time.timeScale = 1f;

        _conflictTooltip.SetTooltipActive(false);
    }

    #endregion
}
