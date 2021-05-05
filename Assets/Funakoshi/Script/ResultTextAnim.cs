using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultTextAnim : MonoBehaviour
{
    [SerializeField] GameObject RestartOnOff;
    [SerializeField] GameObject BackToTitleOnOff;
    
    void Start()
    {
        RestartOnOff.SetActive(false);
        BackToTitleOnOff.SetActive(false);

        Invoke("ResultPanelOn", 2.0f);
    }

    
    void Update()
    {
        
    }

    private void ResultPanelOn()
    {
        RestartOnOff.SetActive(true);
        BackToTitleOnOff.SetActive(true);
    }
}
