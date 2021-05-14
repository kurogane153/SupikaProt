using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PushStart : MonoBehaviour
{
    [SerializeField] private int _mainSceneBuildIndex;
    [SerializeField] private int _tutorialSceneBuildIndex;
    [SerializeField] private float _fadeTimeLoadScene;

    [SerializeField] private SoundPlayer _soundPlayer;
    [SerializeField] private AudioClip _se_GameStart;

    public void StartGame()
    {
        //スタートボタンを押したときメインゲームが始まる
        FadeManager.Instance.LoadScene(_mainSceneBuildIndex, _fadeTimeLoadScene);
        _soundPlayer.PlaySE(_se_GameStart);
    }

    public void TutorialSceneLoad()
    {
        //スタートボタンを押したときメインゲームが始まる
        FadeManager.Instance.LoadScene(_tutorialSceneBuildIndex, _fadeTimeLoadScene);
        _soundPlayer.PlaySE(_se_GameStart);
        OptionDataManagerScript.Instance.optionData._tutorialPlayedFlag = true;
        OptionDataManagerScript.Instance.Save();
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
