using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleAnim : MonoBehaviour
{
    [SerializeField] GameObject LogoOnOff;
    [SerializeField] GameObject[] _buttons;

    [SerializeField] GameObject _enterButtonUI;
    
    void Start()
    {
        LogoOnOff.SetActive(false);

        foreach(var button in _buttons) {
            button.SetActive(false);
        }

        _enterButtonUI.SetActive(false);

        Invoke("TitleOn", 3.0f);
        Invoke("PanelOn", 4.8f);
        Invoke("ButtonOn", 5.7f);
    }

    private void TitleOn()
    {
        LogoOnOff.SetActive(true);
    }

    private void PanelOn()
    {
        foreach (var button in _buttons) {
            button.SetActive(true);
        }
    }

    private void ButtonOn()
    {
        _enterButtonUI.SetActive(true);
    }
}
