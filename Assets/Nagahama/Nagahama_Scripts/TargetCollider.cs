using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCollider : MonoBehaviour
{
    [SerializeField] private GameObject _targetButtonPrefab;

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
                GameObject buttonObj = Instantiate(_targetButtonPrefab, transform.position, Quaternion.identity);
                buttonObj.transform.parent = ReticleController.Instance.GetCanvas().transform;
                buttonObj.GetComponent<AsteroidSelectButton>().SetTooltipActive(true, transform);

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

    private void OnDestroy()
    {
       //RectInAsteroidContainer.Instance.targetColliders.Remove(this);
    }
}
