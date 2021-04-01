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
        // Lトリガー押しながらビューボタンでデバッグカメラ起動
        if (Input.GetAxis("L_R_Trigger") >= 0.5f && Input.GetButtonDown("DebugPause")) {
            DebugCamera.Instance.IsEnable = !DebugCamera.Instance.IsEnable;
        }

        // Rトリガー押しながらビューボタンでデバッグ用ポーズ
        if (Input.GetAxis("L_R_Trigger") <= -0.5f && Input.GetButtonDown("DebugPause")) {
            if (Pauser.isPaused) {
                Pauser.Resume();
            } else {
                Pauser.Pause();
            }
        }

        #endregion
    }
}
