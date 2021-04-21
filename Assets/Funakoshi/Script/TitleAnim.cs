using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleAnim : MonoBehaviour
{
    public GameObject LogoOnOff;
    public GameObject PanelOnOff;
    public GameObject ButtonOnOff;

    // Start is called before the first frame update
    void Start()
    {
        LogoOnOff.SetActive(false);
        PanelOnOff.SetActive(false);
        ButtonOnOff.SetActive(false);

        Invoke("TitleOn", 3.0f);
        Invoke("PanelOn", 4.0f);
        Invoke("ButtonOn", 4.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void TitleOn()
    {
        LogoOnOff.SetActive(true);
    }

    private void PanelOn()
    {
        PanelOnOff.SetActive(true);

    }

    private void ButtonOn()
    {
        ButtonOnOff.SetActive(true);
    }
}
