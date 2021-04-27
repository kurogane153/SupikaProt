using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogolightOn : MonoBehaviour
{
    public GameObject EffectOff;

    // Start is called before the first frame update
    void Start()
    {
        EffectOff.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void EffectOn()
    {
        EffectOff.SetActive(true);
    }
}
