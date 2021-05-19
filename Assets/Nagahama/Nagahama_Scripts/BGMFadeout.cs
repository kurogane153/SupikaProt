using UnityEngine;
using System.Collections;

public class BGMFadeout : MonoBehaviour
{
    private float _fadeoutSeconds = 1.0f;
    private AudioSource audioSource;
    private bool isFadeout = false;
    private float fadeDeltaTime = 0;
    private float startVolume;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        startVolume = audioSource.volume;
    }

    private void FixedUpdate()
    {
        if (isFadeout) {
            fadeDeltaTime += Time.deltaTime;
            if (fadeDeltaTime >= _fadeoutSeconds)
            {
                fadeDeltaTime = _fadeoutSeconds;
                isFadeout = false;
            }
            audioSource.volume = (startVolume - fadeDeltaTime / _fadeoutSeconds);

            if (!isFadeout) {
                BGMManagerScript.Instance.StopBGM();
                audioSource.volume = startVolume;
            }
        }
    }

    public void FadeoutStart(float interval)
    {
        isFadeout = true;
        audioSource.volume = startVolume;
        _fadeoutSeconds = interval - 0.017f;
        fadeDeltaTime = 0;
    }

    public void TmpFadeEffect(float outStartDelay, float outTime, float inStartDelay, float inTime)
    {
        StartCoroutine(TmpFadeOutAndFadeIn(outStartDelay, outTime, inStartDelay, inTime));
    }

    // ラスボス撃破演出で音を一瞬フェードアウトさせて、すぐにフェードインする用
    private IEnumerator TmpFadeOutAndFadeIn(float outStartDelay, float outTime, float inStartDelay, float inTime)
    {
        yield return new WaitForSeconds(outStartDelay);
        audioSource.volume = startVolume;
        _fadeoutSeconds = outTime;
        fadeDeltaTime = 0;

        while(fadeDeltaTime < _fadeoutSeconds) {
            fadeDeltaTime += Time.deltaTime;
            audioSource.volume = (startVolume - fadeDeltaTime / _fadeoutSeconds);

            yield return new WaitForFixedUpdate();
        }
        audioSource.Pause();

        yield return new WaitForSeconds(inStartDelay);
        audioSource.UnPause();
        _fadeoutSeconds = inTime;
        fadeDeltaTime = 0;

        while (fadeDeltaTime < _fadeoutSeconds) {
            fadeDeltaTime += Time.deltaTime;
            audioSource.volume = (fadeDeltaTime / _fadeoutSeconds * startVolume);

            yield return new WaitForFixedUpdate();
        }
    }
}
