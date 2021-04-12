using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipScript : MonoBehaviour
{
    [SerializeField, Tooltip("オフのときはOffsetの位置に固定されターゲットに追従しません")] private bool _targetFollowFlag;
    [SerializeField, Tooltip("オフセット")] private Vector2 _offset;
    [SerializeField, Tooltip("追従するターゲット")] private Transform _target;
    [SerializeField, Tooltip("画面外に出たときに削除します")] private bool _destroyOnScreenOut;
    [SerializeField, Tooltip("指定の時間が経過したときに削除します")] private bool _destroyOnTimeOver;
    [SerializeField, Tooltip("削除するまでの時間")] private float _destroyTime;

    [Space(10)]
    [SerializeField, Tooltip("非表示時に別のUIと切り替えるか")] private bool _uiSwapFlag;
    [SerializeField, Tooltip("初期表示オブジェクト")] private GameObject _startUIObject;
    [SerializeField, Tooltip("切り替え先UIオブジェクト")] private GameObject _uiObjectToSwap;

    private Canvas canvas;
    private RectTransform canvasRectTransform;
    private RectTransform rectTransform;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();

        if(_target == null) {
            _target = GameObject.Find("Player").GetComponent<Transform>();
            Debug.Log(gameObject.name + "が_targetをFindで取得した");
        }

        // 指定の時間が経過したときに削除するフラグがオンの場合、自動破壊コルーチンを実行します
        if (_destroyOnTimeOver) {
            StartCoroutine(nameof(AutoDestroy));
        }
    }

    void Update()
    {
        if (_target != null) {
            TargetLockOnMove();
        } else {
            // 画面外に出たときに削除するフラグがオンの場合、削除します
            if (_destroyOnScreenOut) {
                Destroy(gameObject);
            }
        }
    }

    private void TargetLockOnMove()
    {
        Vector3 newPoint = Camera.main.WorldToScreenPoint(_target.position);
        Rect rect = new Rect(0, 0, 1, 1);

        // ターゲットが画面内にいるときかつ追従フラグがオンのときに座標を更新します
        if (_targetFollowFlag && rect.Contains(Camera.main.WorldToViewportPoint(_target.position))) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, newPoint, canvas.worldCamera, out Vector2 newPos);
            rectTransform.localPosition = newPos + _offset;

        } else {
            // 画面外に出たときに削除するフラグがオンの場合、削除します
            if (_destroyOnScreenOut) {
                DestroyProcess();
            }
        }

    }

    private void DestroyProcess()
    {
        Destroy(gameObject);
    }

    // 参照先に、このツールチップがアクティブかどうか返します
    public bool GetTooltipActiveSelf()
    {
        return gameObject.activeSelf;
    }

    // このツールチップのアクティブ状況を変更します
    // canvasの参照が取れていない場合はStartを呼び出し初期化を済ませます
    // swapにtrueのときは切り替えをします
    public void SetTooltipActive(bool flag, bool swap = false)
    {
        if (_uiSwapFlag) {
            if (swap) {
                _startUIObject.SetActive(flag);
                _uiObjectToSwap.SetActive(!flag);
            } else {
                _startUIObject.SetActive(_startUIObject.activeSelf && flag);
                _uiObjectToSwap.SetActive(_uiObjectToSwap.activeSelf && flag);
            }

            
            if (canvas == null) {
                Start();
            }
            TargetLockOnMove();

        } else {
            gameObject.SetActive(flag);
            if (flag) {
                if (canvas == null) {
                    Start();
                }
                TargetLockOnMove();
            }
        }
        
    }

    #region Coroutine
    private IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(_destroyTime);
        DestroyProcess();
    }
    #endregion
}
