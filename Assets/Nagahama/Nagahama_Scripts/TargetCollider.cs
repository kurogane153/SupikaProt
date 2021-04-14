using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCollider : MonoBehaviour
{
    [SerializeField] private GameObject _targetButtonPrefab;
    [SerializeField] private float _reticleRectSizeScaling = 1.2f;  // 2D矩形の大きさ
    [SerializeField] private float _hp;

    private RectTransform _rectTransform;

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
        _hp -= damage;
        if(_hp <= 0) {
            RectInAsteroidContainer.Instance.targetColliders.Remove(this);
            gameObject.SetActive(false);
        }
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
        if (_hp <= 0) return;
        bool isNowInScreen = IsScreenPositionScreenInsite();

        if (isNowInScreen != beforeInScreenFlags) {
            
            if (isNowInScreen) {
                // 画面内に収まった1回目だけ処理します
                RectInAsteroidContainer.Instance.targetColliders.Add(this);

                // エイムアシスト用のボタンプレハブを隕石の子要素に登録させる。
                // canvasを親要素にさせる

                GameObject buttonObj = Instantiate(_targetButtonPrefab, transform.position, Quaternion.identity);
                buttonObj.transform.SetParent(ReticleController.Instance.GetCanvas().transform, false);
                buttonObj.GetComponent<AsteroidSelectButton>().SetTooltipActive(true, transform);
                _rectTransform = buttonObj.GetComponent<RectTransform>();

                Rect rect = GetReticleRect();
                Debug.Log("<color=yellow>ReticleRect.X : " + rect.x + "</color>");
                Debug.Log("<color=yellow>ReticleRect.Y : " + rect.y + "</color>");
                Debug.Log("<color=yellow>ReticleRect.Width : " + rect.width + "</color>");
                Debug.Log("<color=yellow>ReticleRect.Height : " + rect.height + "</color>");

                Debug.Log(gameObject.name + "が" + nameof(RectInAsteroidContainer) + "のListに追加された！" + mainCamera.WorldToViewportPoint(transform.position));

            } else {
                // 画面外に出たとき1回目だけ処理ます
                RectInAsteroidContainer.Instance.targetColliders.Remove(this);
                Debug.Log(gameObject.name + "が" + nameof(RectInAsteroidContainer) + "のListから削除された……");
            }

        }

        beforeInScreenFlags = isNowInScreen;
    }

    // 画面内に収まっているか返します
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

    // ボタンUIがあるかを返します
    public bool IsExistsButtonRect()
    {
        if(_rectTransform == null) {
            return false;
        } else {
            return true;
        }
    }

    private void OnDestroy()
    {
       //RectInAsteroidContainer.Instance.targetColliders.Remove(this);
    }
}
