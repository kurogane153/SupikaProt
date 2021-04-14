using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSelectButton : MonoBehaviour
{
    [SerializeField] private Vector2 _offset;
    [SerializeField] private Transform _target;
    [SerializeField] private bool _destroyOnScreenOut;
    [SerializeField] private bool _destroyOnTimeOver;
    [SerializeField] private float _destroyTime;
    [SerializeField] private Camera _mainCamera;

    private Canvas canvas;
    private RectTransform canvasRectTransform;
    private RectTransform rectTransform;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();

        if (_target == null) {
            _target = GameObject.Find("Player").GetComponent<Transform>();
            Debug.Log(gameObject.name + "が_targetをFindで取得した");
        }

        if (_mainCamera == null) {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            Debug.Log(gameObject.name + "が_mainCameraをFindで取得した");
        }

        if (_destroyOnTimeOver) {
            StartCoroutine(nameof(AutoDestroy));
        }
    }

    void Update()
    {
        if (_target != null && _target.gameObject.activeSelf) {
            TargetLockOnMove();
        } else {
            if (_destroyOnScreenOut) {
                Destroy(gameObject);
            }
        }
    }

    private void TargetLockOnMove()
    {
        Vector3 newPoint = _mainCamera.WorldToScreenPoint(_target.position);
        Rect rect = new Rect(0, 0, 1, 1);

        if (rect.Contains(_mainCamera.WorldToViewportPoint(_target.position))) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, newPoint, canvas.worldCamera, out Vector2 newPos);
            rectTransform.localPosition = newPos + _offset;
        } else {
            if (_destroyOnScreenOut) {
                DestroyProcess();
            }
        }

    }

    private void DestroyProcess()
    {
        ReticleController.Instance.SelectFirstButton();
        Destroy(gameObject);
    }

    public bool GetTooltipActiveSelf()
    {
        return gameObject.activeSelf;
    }

    public void SetTooltipActive(bool flag, Transform target)
    {
        gameObject.SetActive(flag);
        if (flag) {
            SetTarget(target);
            if (canvas == null) {
                Start();
            }
            TargetLockOnMove();
        }
    }

    public void SetTargetReticleController()
    {
        ReticleController.Instance.SetTarget(_target);
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    #region Coroutine
    private IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(_destroyTime);
        DestroyProcess();
    }
    #endregion
}
