using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private void Update()
    {
        // list内の隕石をすべて出力する
        if (Input.GetKeyDown(KeyCode.L)) {
            ShowListContentsInTheDebugLog(targetColliders);
        }
    }

    public void ShowListContentsInTheDebugLog<T>(List<T> list)
    {
        string log = "";

        foreach (var content in list.Select((val, idx) => new { val, idx })) {
            if (content.idx == list.Count - 1)
                log += content.val.ToString();
            else
                log += content.val.ToString() + ", ";
        }

        Debug.Log("<color=red>" + log + "</color>");
    }
}