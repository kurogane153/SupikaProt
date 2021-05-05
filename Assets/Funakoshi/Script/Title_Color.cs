using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title_Color : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip ShineSE;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void EndFadeInAnimation()
    {
        gameObject.SetActive(false);
    }

    public void PlayEffectSound()
    {
        audioSource.PlayOneShot(ShineSE);
    }
}
