using System.Collections.Generic;
using UnityEngine;

public class RectInAsteroidContainer : MonoBehaviour
{
    #region Singleton
    private static RectInAsteroidContainer instance;

    public static RectInAsteroidContainer Instance
    {
        get
        {
            if (instance == null) {
                instance = (RectInAsteroidContainer)FindObjectOfType(typeof(RectInAsteroidContainer));

                if (instance == null) {
                    Debug.LogError(typeof(RectInAsteroidContainer) + "is nothing");
                }
            }

            return instance;
        }
    }
    #endregion

    public List<TargetCollider> targetColliders = new List<TargetCollider>(); // 当たり判定対象の隕石たち

}