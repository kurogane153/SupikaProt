using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    [SerializeField] private SoundPlayer _soundPlayer;
    [SerializeField] private AudioClip _se_OnClick;
    [SerializeField] private AudioClip _se_OnCancel;
    [SerializeField] private AudioClip _se_OnSelect;

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
}
