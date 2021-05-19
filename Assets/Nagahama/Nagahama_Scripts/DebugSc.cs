using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugSc : MonoBehaviour
{
    [SerializeField] private bool isUIEnabled = true;

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
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
        }

        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha2)) {
            Time.timeScale = 1;
            SceneManager.LoadScene(1);
        }

        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha3)) {
            Time.timeScale = 1;
            SceneManager.LoadScene(2);
        }

        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha4)) {
            Time.timeScale = 1;
            SceneManager.LoadScene(3);
        }

        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha5)) {
            Time.timeScale = 1;
            SceneManager.LoadScene(4);
        }

        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha6)) {
            Time.timeScale = 1;
            SceneManager.LoadScene(5);
        }

        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha7)) {
            Time.timeScale = 1;
            SceneManager.LoadScene(6);
        }

        #endregion
    }

    void UIRendererDisable()
    {
        GameObject.FindObjectOfType<Canvas>().enabled = false;
        isUIEnabled = false;
    }

    void UIRendererEnable()
    {
        GameObject.FindObjectOfType<Canvas>().enabled = true;
        isUIEnabled = true;
    }
}
