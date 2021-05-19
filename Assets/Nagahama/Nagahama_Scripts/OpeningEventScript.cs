using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningEventScript : MonoBehaviour
{ 

    [SerializeField] float _nextSceneLoadedTime;
    [SerializeField] int _nextSceneBuildIndex;

    void Start()
    {
        StartCoroutine(nameof(OpeningEvent));
    }

    private IEnumerator OpeningEvent()
    {
        yield return new WaitForSeconds(_nextSceneLoadedTime);
        SceneManager.LoadScene(_nextSceneBuildIndex);
    }
}
