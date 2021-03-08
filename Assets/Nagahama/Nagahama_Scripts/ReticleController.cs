using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleController : MonoBehaviour
{
    [SerializeField] private float _speedX = 1f;
    [SerializeField] private float _speedY = 1f;

    private Canvas canvas;
    private RectTransform rectTransform;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    public void MoveReticle(float x, float y)
    {
        float newX = x * _speedX;
        float newY = y * _speedY;
        Vector3 movePos = new Vector3(newX, newY);

        rectTransform.position += movePos;
    }

    public Vector2 GetReticlePos()
    {
        return RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, rectTransform.position);
    }
}
