using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;

public class PushStart : MonoBehaviour
{
    [SerializeField] private int _mainSceneBuildIndex;
    [SerializeField] private int _tutorialSceneBuildIndex;
    [SerializeField] private int _scoreAttackSceneBuildIndex;
    [SerializeField] private float _fadeTimeLoadScene;

    [SerializeField] private SoundPlayer _soundPlayer;
    [SerializeField] private AudioClip _se_GameStart;
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private PostProcessVolume _postProcessVolume;

    private PostProcessProfile postProcessProfile;

    private void Start()
    {
        postProcessProfile = _postProcessVolume.sharedProfile;

        ColorGrading colorGrading = postProcessProfile.GetSetting<ColorGrading>();
        Color white = new Color(1, 1, 1);
        colorGrading.enabled.Override(true);
        colorGrading.colorFilter.overrideState = true;
        colorGrading.colorFilter.Override(white);
    }

    public void StartGame()
    {
        //スタートボタンを押したときメインゲームが始まる
        FadeManager.Instance.LoadScene(_mainSceneBuildIndex, _fadeTimeLoadScene);
        _soundPlayer.PlaySE(_se_GameStart);
        _eventSystem.enabled = false;
    }

    public void TutorialSceneLoad()
    {
        //スタートボタンを押したときメインゲームが始まる
        FadeManager.Instance.LoadScene(_tutorialSceneBuildIndex, _fadeTimeLoadScene);
        _soundPlayer.PlaySE(_se_GameStart);
        OptionDataManagerScript.Instance.optionData._tutorialPlayedFlag = true;
        OptionDataManagerScript.Instance.Save();
        _eventSystem.enabled = false;
    }

    public void ScoreAttackSceneLoad()
    {
        FadeManager.Instance.LoadScene(_scoreAttackSceneBuildIndex, _fadeTimeLoadScene);
        _soundPlayer.PlaySE(_se_GameStart);
        _eventSystem.enabled = false;
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
