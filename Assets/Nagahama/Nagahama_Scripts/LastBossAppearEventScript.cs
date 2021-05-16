using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;


public class LastBossAppearEventScript : MonoBehaviour
{
    [SerializeField] private PostProcessVolume _postProcessVolume;
    [SerializeField, ColorUsage(false, true)] private Color _filterColor;
    [SerializeField] float _filterColorChangeTime = 3f;
    [SerializeField] float _nextSceneLoadedTime;
    [SerializeField] int _nextSceneBuildIndex;
    [SerializeField] private Transform[] _viewPositions;
    [SerializeField] private float[] _nextPosChangeTime;

    private PostProcessProfile postProcessProfile;

    void Start()
    {
        StartCoroutine(nameof(CameraPosChangeing));
        StartCoroutine(nameof(PostProcessColorFilterChange));
    }

    

    private IEnumerator CameraPosChangeing()
    {
        int i = 0;

        while(i < _viewPositions.Length) {
            yield return new WaitForSeconds(_nextPosChangeTime[i]);
            transform.rotation = _viewPositions[i].rotation;
            transform.position = _viewPositions[i++].position;
        }

        yield return new WaitForSeconds(_nextSceneLoadedTime);
        SceneManager.LoadScene(_nextSceneBuildIndex);

    }

    private IEnumerator PostProcessColorFilterChange()
    {
        float time = 0;

        postProcessProfile = _postProcessVolume.sharedProfile;

        ColorGrading colorGrading = postProcessProfile.GetSetting<ColorGrading>();

        Color startColor = colorGrading.colorFilter.value;
        colorGrading.enabled.Override(true);
        colorGrading.colorFilter.overrideState = true;
        colorGrading.colorFilter.Override(startColor);


        while (time < _filterColorChangeTime) {
            time += Time.deltaTime;
            float rate = time / _filterColorChangeTime;

            colorGrading.colorFilter.Override(Color.Lerp(startColor, _filterColor, rate));
            Debug.Log(colorGrading.colorFilter.value);
            yield return new WaitForFixedUpdate();
        }

        
    }

    


}
