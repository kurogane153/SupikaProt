using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalTargetRockOnEffectScript : MonoBehaviour
{
    [SerializeField] private SoundPlayer _soundPlanyer;
    [SerializeField] private AudioClip _se_lockOn;
    [SerializeField] private GameObject _lockOnMarkarPrefab;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private RadialBlur _radialBlur;
    [SerializeField] private ParticleSystem _lineParticle;
    [SerializeField] private PlayerMove _playerMove;
    [SerializeField] private PlayerShot _playerShot;
    [SerializeField] private GameObject[] _disableUIs;

    [Space(10)]
    [SerializeField] private float _rotateTime = 1f;
    [SerializeField] private float[] _markarInstTimes;
    [SerializeField] private Vector3[] _markarScales;

    private Transform mainCamera;
    private Transform targetCollider;
    private bool isCanRotation;

    void Start()
    {
        mainCamera = _cameraController.transform;
    }

    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (!isCanRotation || !targetCollider) return;

        Vector3 targetPositon = targetCollider.position;

        // 高さがずれていると体ごと上下を向いてしまうので便宜的に高さを統一
        if (mainCamera.position.y != targetCollider.position.y) {
            targetPositon = new Vector3(targetCollider.position.x, mainCamera.position.y, targetCollider.position.z);
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetPositon - mainCamera.position);
        mainCamera.rotation = Quaternion.Slerp(mainCamera.rotation, targetRotation, Time.deltaTime * _rotateTime);
    }

    public void StartFinalTgtRockOnEffect(Transform target)
    {
        Pauser.isCanNotPausing = true;

        targetCollider = target;
        targetCollider.root.GetComponent<AsteroidScript>().ChangeSpeed(0f);

        _cameraController.enabled = false;
        _radialBlur.EnableRadialBlur();
        _lineParticle.gameObject.SetActive(true);
        _lineParticle.Play();

        _playerMove.enabled = false;
        _playerShot._isStickControllFlag = false;

        isCanRotation = true;

        foreach(var ui in _disableUIs) {
            ui.SetActive(false);
        }

        StartCoroutine(nameof(InstLockOnMarkars));
    }

    private IEnumerator InstLockOnMarkars()
    {
        Canvas canvas = ReticleController.Instance.GetCanvas();
        Camera mainCamera = _cameraController.GetComponent<Camera>();

        int i = 0;

        foreach (var time in _markarInstTimes) {
            yield return new WaitForSeconds(time);
            GameObject newLockonReticle = Instantiate(_lockOnMarkarPrefab, transform.position, Quaternion.identity);
            LockedOnReticle lockedOnReticle = newLockonReticle.GetComponent<LockedOnReticle>();

            lockedOnReticle.InstantiateSettings(canvas, targetCollider, mainCamera);
            lockedOnReticle.transform.localScale = _markarScales[i++];
            lockedOnReticle.GetComponent<Animator>().enabled = false;
            _soundPlanyer.PlaySE(_se_lockOn);
        }
    }
}
