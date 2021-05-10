using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PushStart : MonoBehaviour
{
    [SerializeField] private int _nextLoadSceneBuildIndex;
    [SerializeField] private float _fadeTimeLoadScene;

    [SerializeField] private SoundPlayer _soundPlayer;
    [SerializeField] private AudioClip _se_GameStart;

    public void StartGame()
    {
        //スタートボタンを押したときメインゲームが始まる
        FadeManager.Instance.LoadScene(_nextLoadSceneBuildIndex, _fadeTimeLoadScene);
        _soundPlayer.PlaySE(_se_GameStart);
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
