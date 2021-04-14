using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Pauser : MonoBehaviour
{
    public static List<Pauser> targets = new List<Pauser>();   // ポーズ対象のスクリプト
    public static bool isPaused;

    [SerializeField] private bool _killCameraPause; // キルカメラ中に止ってほしいときやつだけオンにする

    // ポーズ対象のコンポーネント
    Behaviour[] pauseBehavs = null;

    Rigidbody[] rgBodies = null;
    Vector3[] rgBodyVels = null;
    Vector3[] rgBodyAVels = null;

    Rigidbody2D[] rg2dBodies = null;
    Vector2[] rg2dBodyVels = null;
    float[] rg2dBodyAVels = null;

    // 初期化
    void Awake()
    {
        // ポーズ対象に追加する
        targets.Add(this);
    }

    private void OnDestroy()
    {
        // ポーズ対象から除外する
        targets.Remove(this);
    }

    // ポーズされたとき
    void OnPause()
    {
        if (!ReferenceEquals(pauseBehavs, null)) {
            return;
        }

        // 有効なコンポーネントを取得
        if(ReferenceEquals(pauseBehavs, null))
            pauseBehavs = Array.FindAll(GetComponentsInChildren<Behaviour>(), (obj) => { return obj.enabled; });
        foreach (var com in pauseBehavs) {
            com.enabled = false;
        }

        rgBodies = Array.FindAll(GetComponentsInChildren<Rigidbody>(), (obj) => { return !obj.IsSleeping(); });
        rgBodyVels = new Vector3[rgBodies.Length];
        rgBodyAVels = new Vector3[rgBodies.Length];
        for (var i = 0; i < rgBodies.Length; ++i) {
            rgBodyVels[i] = rgBodies[i].velocity;
            rgBodyAVels[i] = rgBodies[i].angularVelocity;
            rgBodies[i].Sleep();
        }

        rg2dBodies = Array.FindAll(GetComponentsInChildren<Rigidbody2D>(), (obj) => { return !obj.IsSleeping(); });
        rg2dBodyVels = new Vector2[rg2dBodies.Length];
        rg2dBodyAVels = new float[rg2dBodies.Length];
        for (var i = 0; i < rg2dBodies.Length; ++i) {
            rg2dBodyVels[i] = rg2dBodies[i].velocity;
            rg2dBodyAVels[i] = rg2dBodies[i].angularVelocity;
            rg2dBodies[i].Sleep();
        }

        
    }

    // ポーズ解除されたとき
    void OnResume()
    {
        if (pauseBehavs == null) {
            return;
        }

        // ポーズ前の状態にコンポーネントの有効状態を復元
        foreach (var com in pauseBehavs) {
            com.enabled = true;
        }

        for (var i = 0; i < rgBodies.Length; ++i) {
            rgBodies[i].WakeUp();
            rgBodies[i].velocity = rgBodyVels[i];
            rgBodies[i].angularVelocity = rgBodyAVels[i];
        }

        for (var i = 0; i < rg2dBodies.Length; ++i) {
            rg2dBodies[i].WakeUp();
            rg2dBodies[i].velocity = rg2dBodyVels[i];
            rg2dBodies[i].angularVelocity = rg2dBodyAVels[i];
        }

        pauseBehavs = null;

        rgBodies = null;
        rgBodyVels = null;
        rgBodyAVels = null;

        rg2dBodies = null;
        rg2dBodyVels = null;
        rg2dBodyAVels = null;

       
    }

    // キルカメラ用ポーズ
    public static void SoftPause()
    {
        foreach (var obj in targets) {
            if (!obj._killCameraPause) continue;
            if (obj.CompareTag("AsteroidTargetCollider")) {

                TargetCollider targetCollider = obj.GetComponent<TargetCollider>();
                if (targetCollider.IsLockedOn) {
                    targetCollider.GetAsteroidScript.IsMovePause = true;
                } else {
                    obj.OnPause();
                }

            } else {
                obj.OnPause();
            }
            
        }
    }

    // キルカメラ用ポーズ解除
    public static void SoftResume()
    {
        foreach (var obj in targets) {
            if (!obj._killCameraPause) continue;
            if (obj.CompareTag("AsteroidTargetCollider")) {

                TargetCollider targetCollider = obj.GetComponent<TargetCollider>();
                if (targetCollider.IsLockedOn) {
                    targetCollider.GetAsteroidScript.IsMovePause = false;
                } else {
                    obj.OnResume();
                }

            } else {
                obj.OnResume();
            }
        }
    }

    // 強制ポーズ
    public static void Pause()
    {
        foreach (var obj in targets) {
            obj.OnPause();
        }
        isPaused = true;
        Time.timeScale = 0f;
    }

    // 強制ポーズ解除
    public static void Resume()
    {
        foreach (var obj in targets) {
            obj.OnResume();
        }
        isPaused = false;
        Time.timeScale = 1f;
    }
}