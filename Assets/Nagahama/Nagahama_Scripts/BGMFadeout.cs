using UnityEngine;

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
            audioSource.volume = (float)(startVolume - fadeDeltaTime / _fadeoutSeconds);

            if (!isFadeout) {
                BGMManagerScript.Instance.StopBGM();
                audioSource.volume = startVolume;
            }
        }
    }

    public void FadeoutStart(float interval)
    {
        isFadeout = true;
        _fadeoutSeconds = interval - 0.017f;
    }
}
