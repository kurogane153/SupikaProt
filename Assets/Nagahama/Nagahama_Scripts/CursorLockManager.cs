﻿using UnityEngine;
using UnityEngine.SceneManagement;

// カーソルを非表示にさせるためにスクリプト
public class CursorLockManager : MonoBehaviour
{
    private void Awake()
    {
        CursorLock();
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F1)) {
            if (Cursor.lockState == CursorLockMode.Locked) {
                CursorUnLock();
            } else if (Cursor.lockState == CursorLockMode.None) {
                CursorLock();
            }
        }

    }

    // カーソルをロックし非表示にする
    private void CursorLock()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // カーソルをアンロック、表示する
    private void CursorUnLock()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
