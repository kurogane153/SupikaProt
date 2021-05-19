using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class FinalTargetRockOnEffectScript : MonoBehaviour
{
    [SerializeField] private PostProcessVolume _postProcessVolume;
    [SerializeField] float _filterColorChangeTime = 3f;

    [SerializeField,Space(5)] private SoundPlayer _soundPlanyer;
    [SerializeField] private float[] _lockOnSEPitchs;
    [SerializeField] private AudioClip _se_lockOn;

    [SerializeField, Space(5)] private GameObject _lockOnMarkarPrefab;
    [SerializeField, Space(5)] private CameraController _cameraController;
    [SerializeField, Space(5)] private RadialBlur _radialBlur;
    [SerializeField, Space(5)] private ParticleSystem _lineParticle;
    [SerializeField, Space(5)] private PlayerMove _playerMove;
    [SerializeField, Space(5)] private PlayerShot _playerShot;
    [SerializeField, Space(5)] private GameObject[] _disableUIs;

    [Space(10)]
    [SerializeField] private float _rotateTime = 1f;
    [SerializeField] private float[] _markarInstTimes;
    [SerializeField] private Vector3[] _markarScales;

    private PostProcessProfile postProcessProfile;
    private ColorGrading colorGrading;
    private Transform mainCamera;
    private Transform targetCollider;
    private bool isCanRotation;

    void Start()
    {
        postProcessProfile = _postProcessVolume.sharedProfile;

        colorGrading = postProcessProfile.GetSetting<ColorGrading>();
        mainCamera = _cameraController.transform;
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
        StartCoroutine(nameof(PostProcessColorFilterChange));
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
            lockedOnReticle.transform.localScale = _markarScales[i];
            lockedOnReticle.GetComponent<Animator>().enabled = false;
            _soundPlanyer.ChangePitchLevel(_lockOnSEPitchs[i++]);
            _soundPlanyer.PlaySE(_se_lockOn);
        }
    }

    private IEnumerator PostProcessColorFilterChange()
    {
        float time = 0;

        Color startColor = colorGrading.colorFilter.value;
        Color white = new Color(1, 1, 1);
        colorGrading.enabled.Override(true);
        colorGrading.colorFilter.overrideState = true;
        colorGrading.colorFilter.Override(startColor);


        while (time < _filterColorChangeTime) {
            time += Time.deltaTime;
            float rate = time / _filterColorChangeTime;

            colorGrading.colorFilter.Override(Color.Lerp(startColor, white, rate));
            Debug.Log(colorGrading.colorFilter.value);
            yield return new WaitForFixedUpdate();
        }

    }

}