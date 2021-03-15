using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReticleController : MonoBehaviour
{
    [SerializeField] private GameObject _lockedOnReticlePrefab;
    [SerializeField] private Color _lockOnColor;
    [SerializeField] private float _speedX = 1f;
    [SerializeField] private float _speedY = 1f;
    [SerializeField] private float _aimAssistDegreeX = 3;
    [SerializeField] private float _aimAssistDegreeY = 3;

    private Canvas canvas;
    private RectTransform canvasRectTransform;
    private RectTransform rectTransform;
    private Image image;
    private Color defaultColor;
    private bool isBeforeTargeting;

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
        if (target && (Mathf.Abs(x) < 0.2f) && (Mathf.Abs(y)) < 0.2f) {
            //TargetLockOnMove(target);
        }

        float newX = x * _speedX;
        float newY = y * _speedY;
        bool isNowTargeting;

        if (target) {
            image.color = _lockOnColor;
            newX /= _aimAssistDegreeX;
            newY /= _aimAssistDegreeY;

            isNowTargeting = true;

            if(isNowTargeting && !isBeforeTargeting) {
                AsteroidScript asteroid = target.GetComponent<AsteroidScript>();

                if (!asteroid.IsLockedOn) {
                    asteroid.IsLockedOn = true;

                    GameObject newLockonReticle = Instantiate(_lockedOnReticlePrefab, transform.position, Quaternion.identity);
                    LockedOnReticle lockedOnReticle = newLockonReticle.GetComponent<LockedOnReticle>();

                    lockedOnReticle.InstantiateSettings(canvas, target);
                }
            }

        } else {
            image.color = defaultColor;
            isNowTargeting = false;
        }

        Vector3 movePos = new Vector3(newX, newY);

        if(!IsNewScreenPositionScreenOutsite(rectTransform.position + movePos)) {
            rectTransform.position += movePos;
        }

        isBeforeTargeting = isNowTargeting;

    }

    private bool IsNewScreenPositionScreenOutsite(Vector3 pos)
    {
        Rect rect = new Rect(0, 0, 1, 1);

        if (rect.Contains(Camera.main.ScreenToViewportPoint(pos))){
            return false;
        }
        return true;
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
