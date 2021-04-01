using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSc : MonoBehaviour
{


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

        #endregion
    }
}
