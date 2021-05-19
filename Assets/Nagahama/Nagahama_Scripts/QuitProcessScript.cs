using UnityEngine;

public class QuitProcessScript : MonoBehaviour
{
    public void GoToTitle()
    {
        Time.timeScale = 1f;
        FadeManager.Instance.LoadScene(0, 3f);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
            UnityEngine.Application.Quit();
#endif
    }


}
