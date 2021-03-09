using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReticleController : MonoBehaviour
{
    [SerializeField] private Color _lockOnColor;
    [SerializeField] private float _speedX = 1f;
    [SerializeField] private float _speedY = 1f;

    private Canvas canvas;
    private RectTransform canvasRectTransform;
    private RectTransform rectTransform;
    private Image image;
    private Color defaultColor;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        defaultColor = image.color;
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    public void MoveReticle(float x, float y, Transform target = null)
    {
        if (target && (-0.2f < x && x < 0.2f) && (-0.2f < y && y < 0.2f)) {
            TargetLockOnMove(target);
        }

        float newX = x * _speedX;
        float newY = y * _speedY;

        if (target) {
            image.color = _lockOnColor;
            newX /= 4;
            newY /= 4;
        } else {
            image.color = defaultColor;
        }

        Vector3 movePos = new Vector3(newX, newY);

        rectTransform.position += movePos;
    }

    private void TargetLockOnMove(Transform target)
    {
        Vector3 newPoint = Camera.main.WorldToScreenPoint(target.position);
        Rect rect = new Rect(0, 0, 1, 1);

        if (rect.Contains(Camera.main.WorldToViewportPoint(target.position))) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, newPoint, canvas.worldCamera, out Vector2 newPos);
            rectTransform.localPosition = newPos;
        }
        
    }

    public Vector2 GetReticlePos()
    {
        return RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, rectTransform.position);
    }
}
