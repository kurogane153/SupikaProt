using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LastBossAppearEventScript : MonoBehaviour
{
    [SerializeField] float _nextSceneLoadedTime;
    [SerializeField] int _nextSceneBuildIndex;
    [SerializeField] private Transform[] _viewPositions;
    [SerializeField] private float[] _nextPosChangeTime;

    void Start()
    {
        StartCoroutine(nameof(CameraPosChangeing));
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


}
