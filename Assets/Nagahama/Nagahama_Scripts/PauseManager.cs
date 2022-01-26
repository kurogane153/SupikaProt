using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [Header("スコアアタックモード時に使用")]
    public GameObject _timeOverPanel;
    public GameObject _scoreDispPanel;
    public TextMeshProUGUI _scoreUI;
    public GameObject _highScoreUI;

    [Space(10f)]
    [SerializeField] GameObject _pausingCamera;

    [SerializeField, Space(10)] Menu _pausePanel;
    [SerializeField] Menu[] menus;    

    private Camera pauseCam;
    private AudioListener pauseAudioListener;


    void Start()
    {
        if (_pausingCamera == null) {
            _pausingCamera = GameObject.Find("PausingCamera");
            Debug.Log(gameObject.name + "が_pausingCameraをFindで取得した");
        }

        pauseCam = _pausingCamera.GetComponent<Camera>();
        pauseAudioListener = _pausingCamera.GetComponent<AudioListener>();
    }

    
    void Update()
    {
        // Menuボタンでポーズ
        if (!Pauser.isCanNotPausing && Input.GetButtonDown("Pause")) {
            if (Pauser.isPaused) {
                Resume();
            } else {
                _pausePanel.Open();
                Pauser.Pause();
                pauseCam.enabled = true;
                pauseAudioListener.enabled = true;
            }
        }
    }

    public void Pause()
    {
        Pauser.Pause();
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().enabled = true;
        pauseAudioListener.enabled = true;
        Pauser.isCanNotPausing = true;
        _timeOverPanel.SetActive(true);
        StartCoroutine(nameof(ScoreDisplayCoroutine));
    }

    public void Resume()
    {
        pauseCam.enabled = false;
        pauseAudioListener.enabled = false;

        for (int i = 0; i < menus.Length; i++) {
            CloseMenu(menus[i]);
        }
        Pauser.Resume();
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }

    private IEnumerator ScoreDisplayCoroutine()
    {
        yield return new WaitForSecondsRealtime(3f);

        _scoreDispPanel.SetActive(true);

        yield return new WaitForSecondsRealtime(0.5f);

        _scoreUI.gameObject.SetActive(true);
        _scoreUI.text = ScoreManager.Instance.Score.ToString("N0");

        if(OptionDataManagerScript.Instance.optionData._highScore < ScoreManager.Instance.Score) {
            yield return new WaitForSecondsRealtime(0.5f);
            _highScoreUI.SetActive(true);
            OptionDataManagerScript.Instance.optionData._highScore = ScoreManager.Instance.Score;
            OptionDataManagerScript.Instance.Save();
        }

        yield return new WaitForSecondsRealtime(4f);

        FadeManager.Instance.LoadScene(0, 2f);
    }
}