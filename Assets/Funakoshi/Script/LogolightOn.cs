using UnityEngine;

public class LogolightOn : MonoBehaviour
{
    public GameObject EffectOff;

    void Start()
    {
        EffectOff.SetActive(false);
    }

    private void EffectOn()
    {
        EffectOff.SetActive(true);
    }
}
