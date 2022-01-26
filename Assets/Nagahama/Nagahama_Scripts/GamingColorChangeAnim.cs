using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GamingColorChangeAnim : MonoBehaviour
{
    /// <summary>
    /// 色変更間隔
    /// </summary>
    [SerializeField] private float ChangeColorTime = 0.01f;

    /// <summary>
    /// 色変更のなめらかさ
    /// </summary>
    [SerializeField] private float Smooth = 0.01f;

    /// <summary>
    /// 色彩
    /// </summary>
    [SerializeField, Range(0, 1)] private float HSV_Hue = 1.0f;

    /// <summary>
    /// 彩度
    /// </summary>
    [SerializeField, Range(0, 1)] private float HSV_Saturation = 1.0f;

    /// <summary>
    /// 明度
    /// </summary>
    [SerializeField, Range(0, 1)] private float HSV_Brightness = 1.0f;

    /// <summary>
    /// 色彩MAX
    /// </summary>
    [SerializeField, Range(0, 1)] private float HSV_Hue_Max = 1.0f;

    /// <summary>
    /// 色彩MIN
    /// </summary>
    [SerializeField, Range(0, 1)] private float HSV_Hue_Min = 0.0f;

    private TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        HSV_Hue = HSV_Hue_Min;
        StartCoroutine(nameof(ChangeColor));
    }

    /// <summary>
    /// TextのColorを1680万色に光らせるアニメーションコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator ChangeColor()
    {
        // 無限ループ
        while (true) {
            HSV_Hue += Smooth;

            if (HSV_Hue >= HSV_Hue_Max) {
                HSV_Hue = HSV_Hue_Min;
            }

            // HSVをRGBに変換し、TextのColorにセット
            text.color = Color.HSVToRGB(HSV_Hue, HSV_Saturation, HSV_Brightness);

            yield return new WaitForSecondsRealtime(ChangeColorTime);
        }

    }
}