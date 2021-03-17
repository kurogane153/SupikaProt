using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedOnReticle : MonoBehaviour
{
    private Canvas canvas;
    private RectTransform canvasRectTransform;
    private RectTransform rectTransform;
    private Transform target;

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
        if(target != null) {
            TargetLockOnMove();
        } else {
            ReticleController.Instance.GeneratedReticleCount -= 1;
            Destroy(gameObject);
        }
        
    }

    public void InstantiateSettings(Canvas canvas, Transform target)
    {
        this.canvas = canvas;
        Target = target;

        transform.parent = canvas.transform;
    }

    private void TargetLockOnMove()
    {
        Vector3 newPoint = Camera.main.WorldToScreenPoint(target.position);
        Rect rect = new Rect(0, 0, 1, 1);

        if (rect.Contains(Camera.main.WorldToViewportPoint(target.position))) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, newPoint, canvas.worldCamera, out Vector2 newPos);
            rectTransform.localPosition = newPos;
        } else {
            DestroyProcess();
        }

    }

    private void DestroyProcess()
    {
        target.GetComponent<AsteroidScript>().IsLockedOn = false;
        ReticleController.Instance.GeneratedReticleCount -= 1;
        Destroy(gameObject);
    }
}
