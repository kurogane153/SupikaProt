using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RadialBlur : MonoBehaviour
{
    [SerializeField, Tooltip("放射ブラーシェーダー")] private Shader _shader;
    [SerializeField, Range(0, 16), Tooltip("放射ブラーの厚み")] private int _sampleCount = 8;
    [SerializeField, Range(0.0f, 1.0f), Tooltip("放射ブラーの強さ")] private float _strength = 0.5f;
    [SerializeField, Range(0.0f, 1.0f), Tooltip("放射ブラー強さの最大値")] private float _maxStrength = 0.25f;
    [SerializeField, Tooltip("放射ブラーが最大に達するまでの時間")] private float _arrivalTimeOfMaxStrength;
    [SerializeField, Tooltip("放射ブラーが0に達するまでの時間")] private float _arrivalTimeOfStrengthToZero;
    [SerializeField, Range(0.0f, 5f), Tooltip("画面揺れの強さ")] private float _magnitude;

    private Material _material;
    private float startTime;
    private bool shakeFlags;
    private bool beforeShake;
    private Vector3 beforeLocalPos;

    private void Start()
    {
        _strength = 0f;
    }

    public void EnableRadialBlur()
    {
        enabled = true;
        shakeFlags = true;
        StopCoroutine(nameof(RadiulBlurEnd));
        StartCoroutine(nameof(RadiulBlurStart));
    }

    public void DisableRadialBlur()
    {
        shakeFlags = false;
        StopCoroutine(nameof(RadiulBlurStart));
        StartCoroutine(nameof(RadiulBlurEnd));
    }

    private IEnumerator RadiulBlurStart()
    {
        startTime = Time.timeSinceLevelLoad;

        while (_strength < _maxStrength) {
            var diff = Time.timeSinceLevelLoad - startTime;
            var lerpRate = diff / _arrivalTimeOfMaxStrength;
            _strength = Mathf.Lerp(_strength, _maxStrength, lerpRate);

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator RadiulBlurEnd()
    {
        startTime = Time.timeSinceLevelLoad;

        while (0 < _strength) {
            var diff = Time.timeSinceLevelLoad - startTime;
            var lerpRate = diff / _arrivalTimeOfStrengthToZero;
            _strength = Mathf.Lerp(_strength, 0, lerpRate);

            yield return new WaitForFixedUpdate();
        }
        enabled = false;
    }

    private void CameraShake()
    {
        if (!shakeFlags) return;

        if (beforeShake) {

            transform.localPosition = beforeLocalPos;

            beforeShake = false;

        } else {
             
            var pos = transform.localPosition;
            beforeLocalPos = pos;

            var x = pos.x + Random.Range(-1f, 1f) * _magnitude;
            var y = pos.y + Random.Range(-1f, 1f) * _magnitude;

            transform.localPosition = new Vector3(x, y, pos.z);

            beforeShake = true;
        }

        
    }

    private void Update()
    {
        //CameraShake();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        if (_material == null) {
            if (_shader == null) {
                Graphics.Blit(source, dest);
                return;
            } else {
                _material = new Material(_shader);
            }
        }
        _material.SetInt("_SampleCount", _sampleCount);
        _material.SetFloat("_Strength", _strength);
        Graphics.Blit(source, dest, _material);
    }
}