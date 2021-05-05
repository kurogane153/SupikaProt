using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PushStart : MonoBehaviour
{
    [SerializeField] private int _nextLoadSceneBuildIndex;

    public void StartGame()
    {
        //スタートボタンを押したときメインゲームが始まる
        SceneManager.LoadScene(_nextLoadSceneBuildIndex);
    }

    public void PushEnd()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

#elif UNITY_STANDALONE
        UnityEngine.Application.Quit();
#endif
    }
}
