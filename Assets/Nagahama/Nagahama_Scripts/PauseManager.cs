using UnityEngine;

public class PauseManager : MonoBehaviour
{
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
}