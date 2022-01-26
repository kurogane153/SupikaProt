using UnityEngine;
using TMPro;

public class ButtonScript : MonoBehaviour
{
    [SerializeField] private GameObject _highScore;
    [SerializeField] private TextMeshProUGUI _highScoreText;
    [SerializeField] private SoundPlayer _soundPlayer;
    [SerializeField] private AudioClip _se_OnClick;
    [SerializeField] private AudioClip _se_OnCancel;
    [SerializeField] private AudioClip _se_OnSelect;

    private void Start()
    {
        _highScoreText.text = OptionDataManagerScript.Instance.optionData._highScore.ToString("N0");
    }

    public void OnClickSE()
    {
        _soundPlayer.PlaySE(_se_OnClick);
    }

    public void OnCancelSE()
    {
        _soundPlayer.PlaySE(_se_OnCancel);
    }

    public void OnSelectSE()
    {
        _soundPlayer.PlaySE(_se_OnSelect);
    }

    public void OpenHighScore()
    {
        _highScore.SetActive(true);
    }

    public void CloseHighScore()
    {
        _highScore.SetActive(false);
    }
}
