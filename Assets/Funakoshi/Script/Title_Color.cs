using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title_Color : MonoBehaviour
{
    [SerializeField] SoundPlayer _soundPlayer;
    public AudioClip ShineSE;

    private void Start()
    {
        
    }

    public void EndFadeInAnimation()
    {
        gameObject.SetActive(false);
    }

    public void PlayEffectSound()
    {
        _soundPlayer.PlaySE(ShineSE);
    }
}
