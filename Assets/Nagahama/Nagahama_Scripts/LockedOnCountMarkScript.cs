using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockedOnCountMarkScript : MonoBehaviour
{
    static List<LockedOnCountMarkScript> marks = new List<LockedOnCountMarkScript>();

    [SerializeField] private Sprite _lockedOnImage;
    [SerializeField] private int _countNumber;

    private Sprite nonLockedOnImage;
    private Image image;

    void Awake()
    {
        // カウント対象に追加する
        marks.Add(this);
    }

    private void OnDestroy()
    {
        // カウント対象から除外する
        marks.Remove(this);
    }

    void Start()
    {
        image = GetComponent<Image>();
        nonLockedOnImage = image.sprite;
    }

    // ロックオン数が増減したときに呼ばれる
    public void OnLockedOnCountCheck()
    {
        if (ReticleController.Instance.GeneratedReticleCount >= this._countNumber) {
            image.sprite = _lockedOnImage;
        } else {
            image.sprite = nonLockedOnImage;
        }
    }

    public static void LockedOnCountCheck()
    {
        foreach (var obj in marks) {
            obj.OnLockedOnCountCheck();
        }
    }
}
