using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField, Tooltip("ポーズ解除時に効果音を再生し直すか")] private bool _isLoopSource;
    private AudioSource audioSource;
    private float startVolume;
    private bool isStoped;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        startVolume = audioSource.volume;
        isStoped = false;
    }

    void Start()
    {
        OptionDataManagerScript.Instance.optionValueChanges.AddListener(ChangeSEVolume);
        ChangeSEVolume();
    }

    void Update()
    {
        
    }

    private void OnEnable()
    {
        if (isStoped && _isLoopSource) {
            PlaySE();
        }
    }

    private void OnDisable()
    {
        isStoped = true;
    }

    private void ChangeSEVolume()
    {
        audioSource.volume = startVolume * OptionDataManagerScript.Instance.optionData._seVolume;
        Debug.Log("SE処理完了");
    }

    public void PlaySE(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }

    public void PlaySE()
    {
        audioSource.Play();
    }

    public void ChangePitchLevel(float pitchLevel)
    {
        audioSource.pitch = pitchLevel;
    }

    public void DestroyCall(float time)
    {
        StartCoroutine(SelfDestroy(time));
    }

    public IEnumerator SelfDestroy(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (OptionDataManagerScript.Instance != null) {
            OptionDataManagerScript.Instance.optionValueChanges.RemoveListener(ChangeSEVolume);
        }
        
    }
}
