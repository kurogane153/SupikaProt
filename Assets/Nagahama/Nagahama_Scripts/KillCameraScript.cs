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
    [SerializeField] private float _missileFollowXRotate = 15f;
    [SerializeField] private float _cameraPanSpeed = 20f;
    [SerializeField] private float _followModeSwitchDelay = 0.75f;
    [SerializeField] private float _resetDelay = 2.2f;
    [SerializeField] private float _lerpFactor = 6;

    [SerializeField] private PlayerMove _playerMove;
    [SerializeField] private PlayerShot _playerShot;

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
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * _lerpFactor);

        Vector3 desiredRotation = _followTarget.rotation.eulerAngles + _rotationOffset;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(desiredRotation), Time.deltaTime * _lerpFactor);
    }

    private void StopPhase_CameraPanMove()
    {
        transform.Translate(-Vector3.forward * Time.deltaTime * _cameraPanSpeed);
    }

    private void FollowTargetMove()
    {
        if(_followTarget == null) {
            StartCoroutine(nameof(SwitchStagingPhase_ShowExplosion));
            return;
        }
        Vector3 desiredPosition = _followTarget.position + _missileFollowOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * _lerpFactor);
    }

    private void WarpTargetBehind()
    {

        transform.rotation = _followTarget.rotation;
        transform.Rotate(_missileFollowXRotate, 180, 0);

        Vector3 desiredPosition = _followTarget.position + _missileFollowOffset;
        transform.position = desiredPosition;

    }

    private void LookAtTarget()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((_followTarget.position - transform.position).normalized), Time.deltaTime * _lerpFactor);
    }

    private void LateUpdate()
    {

        if (_cameraType == CameraType.KillCamera) {
            KillCameraProcess();
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
                StopPhase_CameraPanMove();
                break;

            case StagingPhase.LookAtMissile:
                LookAtTarget();
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

    private IEnumerator SwitchStagingPhase_FollowMode()
    {
        yield return new WaitForSeconds(_followModeSwitchDelay);
        stagingPhase = StagingPhase.FollowMissile;
        WarpTargetBehind();
    }

    private IEnumerator SwitchStagingPhase_ShowExplosion()
    {
        stagingPhase = StagingPhase.ShowExplosion;

        yield return new WaitForSeconds(_resetDelay);
        Reset();
    }

    public void KillCameraActive()
    {
        GetCamera().enabled = true;
        SwitchStagingPhase_Pan();
        _playerMove.enabled = false;
        ReticleController.Instance.GetCanvas().enabled = false;
        Pauser.Pause();
        Time.timeScale = 1f;

    }

    public void Reset()
    {
        camera.enabled = false;
        _followTarget = startFollowTarget;
        stagingPhase = StagingPhase.Yet;
        _playerMove.enabled = true;
        ReticleController.Instance.GetCanvas().enabled = true;
        Pauser.Resume();
        Time.timeScale = 1f;
        _playerShot.enabled = true;
    }

    #endregion

    #region 衝突イベント

    private IEnumerator ConflictSwitchStagingPhase_ShowExplosion()
    {
        _orbitShiftTooltip.SetTooltipActive(false);
        conflictStagingPhase = ConflictStagingPhase.ShowExplosion;
        yield return new WaitForSeconds(0.2f);

        _conflictTooltip.SetTooltipActive(true);

        yield return new WaitForSeconds(_resetDelay);
        CFCameraReset();
    }

    public void ConflictEventCameraActive(GameObject target, Vector3 pos)
    {
        transform.position = pos;
        _followTarget = target.transform;
        GetCamera().enabled = true;
        
        _playerMove.enabled = false;
        _playerShot.enabled = false;
        ReticleController.Instance.gameObject.SetActive(false);
        Pauser.Pause();
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
        Pauser.Resume();
        Time.timeScale = 1f;

        _conflictTooltip.SetTooltipActive(false);
    }

    #endregion
}
