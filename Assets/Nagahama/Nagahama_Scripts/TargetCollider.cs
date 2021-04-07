using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCollider : MonoBehaviour
{
    [SerializeField] private GameObject _targetButtonPrefab;
    [SerializeField] private float _reticleRectSizeScaling = 1.2f;  // 2D矩形の大きさ
    [SerializeField] private RectTransform _rectTransform;

    private AsteroidScript parentAsteroidSc;
    private bool isLockedOn;
    private bool beforeInScreenFlags;
    private Camera mainCamera;

    public bool IsLockedOn
    {
        get { return isLockedOn; }
        set { isLockedOn = value; }
    }

    public AsteroidScript GetAsteroidScript
    {
        get { return parentAsteroidSc; }
    }

    public void ReceiveDamage(int damage)
    {
        parentAsteroidSc.ReceiveDamage(damage);
    }


    void Start()
    {
        parentAsteroidSc = transform.root.GetComponent<AsteroidScript>();

        if (mainCamera == null) {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            Debug.Log(gameObject.name + "がmainCameraをFindで取得した");
        }
    }

    private void FixedUpdate()
    {
        bool isNowInScreen = IsScreenPositionScreenInsite();

        if (isNowInScreen != beforeInScreenFlags) {
            if (isNowInScreen) {
                RectInAsteroidContainer.Instance.targetColliders.Add(this);

                // スティックの動きでロックオン対象を変更するためのボタンプレハブを隕石の子要素に登録させる。
                // canvasを親要素にさせる
                // 強力エイムアシスト機能がオンの状態のときのみ処理する

                // 2021/4/7　池村先生の仕様書に合わせるために一時的にif文の外に置く。
                GameObject buttonObj = Instantiate(_targetButtonPrefab, transform.position, Quaternion.identity);
                buttonObj.transform.SetParent(ReticleController.Instance.GetCanvas().transform, false);
                buttonObj.GetComponent<AsteroidSelectButton>().SetTooltipActive(true, transform);
                _rectTransform = buttonObj.GetComponent<RectTransform>();

                if (ReticleController.Instance._userSuperAimAssistSystemFlags) {
                    
                }                

                Debug.Log(gameObject.name + "が" + nameof(RectInAsteroidContainer) + "のListに追加された！" + mainCamera.WorldToViewportPoint(transform.position));

            } else {
                RectInAsteroidContainer.Instance.targetColliders.Remove(this);
                Debug.Log(gameObject.name + "が" + nameof(RectInAsteroidContainer) + "のListから削除された……");
            }

        }

        beforeInScreenFlags = isNowInScreen;
    }

    private bool IsScreenPositionScreenInsite()
    {
        Rect rect = new Rect(0, 0, 1, 1);
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);

        if (rect.Contains(viewportPos) && 0f < viewportPos.z) {
            return true;
        }
        return false;
    }

    // 隕石コライダーの2D矩形取得
    public Rect GetReticleRect()
    {
        Vector2 rectsize = _rectTransform.rect.size;
        Vector2 screensize = new Vector2(Screen.width, Screen.height);

        Vector2 size = Vector2.Scale(rectsize / screensize, _rectTransform.localScale);
        Rect rect = new Rect((Vector2)(_rectTransform.position / screensize) - (size * 0.5f), size * _reticleRectSizeScaling);

        return rect;
    }

    private void OnDestroy()
    {
       //RectInAsteroidContainer.Instance.targetColliders.Remove(this);
    }
}
