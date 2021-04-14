using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedOnReticle : MonoBehaviour
{
    private Canvas canvas;
    private RectTransform canvasRectTransform;
    private RectTransform rectTransform;
    private Transform target;
    private Camera mainCamera;
    private TargetCollider targetCollider;

    public Transform Target
    {
        get { return target; }
        set
        {
            if(value != null) {
                target = value;
            }
        }
    }

    void Start()
    {
        //canvas = GetComponentInParent<Canvas>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if(target != null && targetCollider.gameObject.activeSelf) {
            TargetLockOnMove();
        } else {
            ReticleController.Instance.GeneratedReticleCount -= 1;
            Destroy(gameObject);
        }
        
    }

    public void InstantiateSettings(Canvas canvas, Transform target, Camera maincamera)
    {
        this.canvas = canvas;
        Target = target;
        mainCamera = maincamera;
        targetCollider = target.GetComponent<TargetCollider>();

        transform.SetParent(canvas.transform);
    }

    private void TargetLockOnMove()
    {
        Vector3 newPoint = mainCamera.WorldToScreenPoint(target.position);
        Rect rect = new Rect(0, 0, 1, 1);

        if (rect.Contains(mainCamera.WorldToViewportPoint(target.position))) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, newPoint, canvas.worldCamera, out Vector2 newPos);
            rectTransform.localPosition = newPos;
        } else {
            DestroyProcess();
        }

    }

    private void DestroyProcess()
    {
        target.GetComponent<TargetCollider>().IsLockedOn = false;
        ReticleController.Instance.GeneratedReticleCount -= 1;
        Destroy(gameObject);
    }
}
