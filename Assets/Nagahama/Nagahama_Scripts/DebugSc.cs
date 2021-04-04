using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        // Lトリガー押しながらビューボタンでデバッグ用ポーズ
        if ((Input.GetAxis("L_R_Trigger") <= -0.5f && Input.GetButtonDown("DebugPause")) || Input.GetButtonDown("DebugPause_TAB")) {
            if (Pauser.isPaused) {
                Pauser.Resume();
            } else {
                Pauser.Pause();
            }
        }

        // Lトリガー押しながらビューボタンで操作説明非表示切り替え
        if ((Input.GetAxis("L_R_Trigger") <= -0.5f && Input.GetButtonDown("Reload")) || Input.GetKeyDown(KeyCode.U)) {
            if (isUIEnabled) {
                UIRendererDisable();
            } else {
                UIRendererEnable();
            }
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
