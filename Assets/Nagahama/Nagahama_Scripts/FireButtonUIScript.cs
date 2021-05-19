using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FireButtonUIScript : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _text;

    // レティクルのカウント数を参照し、1個でもロックオンしていれば発射ボタンUIをアクティブにする
    void Update()
    {
        if (ReticleController.Instance.GeneratedReticleCount >= 1) {
            UIElementSetActive(true);
        } else {
            UIElementSetActive(false);
        }
    }

    private void UIElementSetActive(bool flag)
    {
        _image.enabled = flag;
        _text.enabled = flag;
    }
}
