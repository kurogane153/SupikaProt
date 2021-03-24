using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipScript : MonoBehaviour
{
    [SerializeField] private Vector2 _offset;
    [SerializeField] private Transform _target;
    [SerializeField] private bool _destroyOnScreenOut;
    [SerializeField] private bool _destroyOnTimeOver;
    [SerializeField] private float _destroyTime;

    private Canvas canvas;
    private RectTransform canvasRectTransform;
    private RectTransform rectTransform;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();

        if(_target == null) {
            _target = GameObject.Find("Player").GetComponent<Transform>();
            Debug.Log(gameObject.name + "が_targetをFindで取得した");
        }

        if (_destroyOnTimeOver) {
            StartCoroutine(nameof(AutoDestroy));
        }
    }

    void Update()
    {
        if (_target != null) {
            TargetLockOnMove();
        } else {
            if (_destroyOnScreenOut) {
                Destroy(gameObject);
            }
        }
    }

    private void TargetLockOnMove()
    {
        Vector3 newPoint = Camera.main.WorldToScreenPoint(_target.position);
        Rect rect = new Rect(0, 0, 1, 1);

        if (rect.Contains(Camera.main.WorldToViewportPoint(_target.position))) {
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
        Destroy(gameObject);
    }

    public bool GetTooltipActiveSelf()
    {
        return gameObject.activeSelf;
    }

    public void SetTooltipActive(bool flag)
    {
        gameObject.SetActive(flag);
        if (flag) {
            if(canvas == null) {
                Start();
            }
            TargetLockOnMove();
        }
    }

    #region Coroutine
    private IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(_destroyTime);
        DestroyProcess();
    }
    #endregion
}
