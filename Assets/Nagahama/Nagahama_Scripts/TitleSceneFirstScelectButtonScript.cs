using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class TitleSceneFirstScelectButtonScript : MonoBehaviour
{
    private EventSystem eventSystem;
    [SerializeField] private Selectable _startSelectable;
    [SerializeField] private Selectable _tutorialSelectable;

    void Start()
    {
        eventSystem = GetComponent<EventSystem>();
    }

    void Update()
    {
        if (eventSystem.currentSelectedGameObject == null && _startSelectable.gameObject.activeSelf) {
            // イベントシステムが何も選んでいないときにスタートボタンがアクティブになって
            // チュートリアルをプレイしたことがあったら
            if (OptionDataManagerScript.Instance.optionData._tutorialPlayedFlag) {
                // スタートボタンを選択する
                _startSelectable.Select();
            } else {
                // チュートリアルをプレイしたことがなかったら
                // チュートリアルを選択する
                _tutorialSelectable.Select();
            }
        }
    }
}
