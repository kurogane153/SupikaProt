using System.Collections;
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
    private bool isChangeError;

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

    public void SpeedChangeError()
    {
        if (isChangeError) return;
        StartCoroutine(nameof(ChangeErrorAnimation));
    }

    private IEnumerator ChangeErrorAnimation()
    {
        isChangeError = true;

        float waitTime = 0.4f;
        float time = 0;
        float startTime;
        Color startColor = _fillImage.color;
        Color errorColor = new Color(1, 1, 1, 1);

        while (time < waitTime) {
            startTime = Time.timeSinceLevelLoad;

            _fillImage.color = errorColor;

            for(int i = 0; i < 3; i++) {
                yield return new WaitForFixedUpdate();
            }

            _fillImage.color = startColor;

            for (int i = 0; i < 3; i++) {
                yield return new WaitForFixedUpdate();
            }

            time += Time.timeSinceLevelLoad - startTime;
        }

        isChangeError = false;
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

        Debug.Log("おおん！");
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
