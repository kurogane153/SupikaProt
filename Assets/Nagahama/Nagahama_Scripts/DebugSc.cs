using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugSc : MonoBehaviour
{
    [SerializeField] private GameObject[] _canvasRenderers;
    [SerializeField] private bool isUIEnabled = true;

    void Start()
    {
        
    }

    void Update()
    {
        #region デバッグ用
        // Rトリガー押しながらビューボタンでデバッグカメラ起動
        if ((Input.GetAxis("L_R_Trigger") >= 0.5f && Input.GetButtonDown("DebugPause")) || Input.GetButtonDown("DebugCamera")) {
            DebugCamera.Instance.IsEnable = !DebugCamera.Instance.IsEnable;
        }

        

        // Lトリガー押しながらビューボタンで操作説明非表示切り替え
        if ((Input.GetAxis("L_R_Trigger") <= -0.5f && Input.GetButtonDown("Reload")) || Input.GetKeyDown(KeyCode.U)) {
            if (isUIEnabled) {
                UIRendererDisable();
            } else {
                UIRendererEnable();
            }
        }

        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha1)) {
            SceneManager.LoadScene(0);
        }

        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha2)) {
            SceneManager.LoadScene(1);
        }

        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha3)) {
            SceneManager.LoadScene(2);
        }

        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha4)) {
            SceneManager.LoadScene(3);
        }

        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha5)) {
            SceneManager.LoadScene(4);
        }

        #endregion
    }

    void UIRendererDisable()
    {
        foreach(var ui in _canvasRenderers) {
            ui.SetActive(false);
        }
        isUIEnabled = false;
    }

    void UIRendererEnable()
    {
        foreach (var ui in _canvasRenderers) {
            ui.SetActive(true);
        }
        isUIEnabled = true;
    }
}
