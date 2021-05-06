using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleAnim : MonoBehaviour
{
    [SerializeField] GameObject LogoOnOff;
    [SerializeField] GameObject StartOnOff;
    [SerializeField] GameObject OptionOnOff;
    [SerializeField] GameObject EndOnOff;
    [SerializeField] GameObject ButtonOnOff;
    

    // Start is called before the first frame update
    void Start()
    {
        LogoOnOff.SetActive(false);
        StartOnOff.SetActive(false);
        OptionOnOff.SetActive(false);
        EndOnOff.SetActive(false);
        ButtonOnOff.SetActive(false);

        Invoke("TitleOn", 3.0f);
        Invoke("PanelOn", 4.8f);
        Invoke("ButtonOn", 5.7f);
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
        StartOnOff.SetActive(true);
        OptionOnOff.SetActive(true);
        EndOnOff.SetActive(true);

    }

    private void ButtonOn()
    {
        ButtonOnOff.SetActive(true);
        //StartOnOff.GetComponent<Selectable>().Select();
    }
}
