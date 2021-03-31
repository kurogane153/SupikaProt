using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCollider : MonoBehaviour
{
    private AsteroidScript parentAsteroidSc;
    private bool isLockedOn;
    private bool beforeInScreenFlags;

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
    }

    private void FixedUpdate()
    {
        bool isNowInScreen = IsScreenPositionScreenInsite();

        if (isNowInScreen != beforeInScreenFlags) {
            if (isNowInScreen) {
                RectInAsteroidContainer.Instance.targetColliders.Add(this);
                Debug.Log(gameObject.name + "が" + nameof(RectInAsteroidContainer) + "のListに追加された！" + Camera.main.WorldToViewportPoint(transform.position));

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
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

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
