using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionMenuActionScript : MonoBehaviour
{
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _seSlider;
    [SerializeField] private Slider _aimSensivity_XSlider;
    [SerializeField] private Slider _aimSensivity_YSlider;
    [SerializeField] private Slider _aimAssistIntensitySlider;
    [SerializeField, Space(10)] private Toggle _useAimAssistFlagToggle;


    void Start()
    {

        OptionDataLoad();
    }

    public void OptionDataLoad()
    {
        _bgmSlider.value = OptionDataManagerScript.Instance.optionData._bgmVolume;
        _seSlider.value = OptionDataManagerScript.Instance.optionData._seVolume;
        _aimSensivity_XSlider.value = OptionDataManagerScript.Instance.optionData._aimSensivity_X;
        _aimSensivity_YSlider.value = OptionDataManagerScript.Instance.optionData._aimSensivity_Y;
        _aimAssistIntensitySlider.value = OptionDataManagerScript.Instance.optionData._aimAssistIntensity;
        _useAimAssistFlagToggle.isOn = OptionDataManagerScript.Instance.optionData._useAimAssistFlag;
    }

    public void BGMVolumeChange(float sliderBGMvolume)
    {
        OptionDataManagerScript.Instance.optionData._bgmVolume = sliderBGMvolume;
    }

    public void SEVolumeChange(float sliderSEvolume)
    {
        OptionDataManagerScript.Instance.optionData._seVolume = sliderSEvolume;
    }

    public void Sensivity_X_Change(float sliderSensivity_X)
    {
        OptionDataManagerScript.Instance.optionData._aimSensivity_X = sliderSensivity_X;
    }

    public void Sensivity_Y_Change(float sliderSensivity_Y)
    {
        OptionDataManagerScript.Instance.optionData._aimSensivity_Y = sliderSensivity_Y;
    }

    public void AimAssistIntensityChange(float sliderAimAssistIntensity)
    {
        OptionDataManagerScript.Instance.optionData._aimAssistIntensity = sliderAimAssistIntensity;
    }

    public void ToggleAimAssist(bool toggleIsOn)
    {
        OptionDataManagerScript.Instance.optionData._useAimAssistFlag = toggleIsOn;
    }

    public void OptionDataSave()
    {
        OptionDataManagerScript.Instance.Save();
    }


}
