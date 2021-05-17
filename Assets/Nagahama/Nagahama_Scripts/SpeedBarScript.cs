using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeedBarScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _speedStatusText;
    [SerializeField] Image _fillImage;
    [SerializeField] float[] _speedStatusValue;
    [SerializeField] string[] _speedStatusName;
    [SerializeField] Color[] _speedStatusColor;

    private Slider slider;
    private float valueChangeDelay;
    private int nowPeriodNum;
    private float nowSliderValue;
    private Color nowFillColor;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        _speedStatusText.text = _speedStatusName[0];
        _fillImage.color = _speedStatusColor[0];
    }

    public void Init(float speedChangeDelay)
    {
        valueChangeDelay = speedChangeDelay;
    }

    public void SetPeriodNum(int periodNum)
    {
        nowSliderValue = slider.value;
        nowFillColor = _fillImage.color;
        _speedStatusText.text = _speedStatusName[periodNum];

        if (slider.gameObject.activeSelf) {
            StartCoroutine(SliderValuesChange(periodNum));
        }
        
    }

    private IEnumerator SliderValuesChange(int newAryNum)
    {
        float time = 0;

        while (time < valueChangeDelay) {
            time += Time.deltaTime;
            float rate = time / valueChangeDelay;

            slider.value = Mathf.Lerp(nowSliderValue, _speedStatusValue[newAryNum], rate);
            _fillImage.color = Color.Lerp(nowFillColor, _speedStatusColor[newAryNum], rate);

            yield return new WaitForFixedUpdate();
        }

        slider.value = _speedStatusValue[newAryNum];
        _fillImage.color = _speedStatusColor[newAryNum];
    }

    
}
