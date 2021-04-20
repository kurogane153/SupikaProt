using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] Menu[] menus;
    [SerializeField] Menu _pausePanel;

    void Start()
    {
        
    }

    
    void Update()
    {
        // Menuーボタンでデバッグ用ポーズ
        if (Input.GetButtonDown("Pause") || Input.GetButtonDown("DebugPause_TAB")) {
            if (Pauser.isPaused) {
                Resume();
            } else {
                _pausePanel.Open();
                Pauser.Pause();
            }
        }
    }

    public void Resume()
    {
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
