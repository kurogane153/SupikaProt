using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogolightOn : MonoBehaviour
{
    public GameObject EffectOff;

    void Start()
    {
        EffectOff.SetActive(false);
    }

    void Update()
    {
        
    }

    private void EffectOn()
    {
        EffectOff.SetActive(true);
    }
}
