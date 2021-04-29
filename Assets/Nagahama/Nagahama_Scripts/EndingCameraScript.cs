using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingCameraScript : MonoBehaviour
{
    public bool rotflg = false;
    public bool followflg = false;
    public Transform target;
    public Transform explosionViwePos;
    public float speed = 2f;
    public LastBossDisolveEffectScript lastBossDisolve;
    float step;
    RadialBlur radialBlur;

    void Start()
    {
        StartCoroutine(nameof(TimeScaleDown));
        radialBlur = GetComponent<RadialBlur>();
    }

    void Update()
    {
        if (rotflg) {
            if(target == null) {
                rotflg = false;
                return;
            }
            Vector3 targetDir = target.position - transform.position;
            targetDir.y = transform.position.y; //targetと高さが異なると体ごと上下を向いてしまうので制御
            float step = speed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 10.0F);
            transform.rotation = Quaternion.LookRotation(newDir);
        }

        if(followflg) {
            Vector3 desiredPosition = target.position + new Vector3(0, 10, -90);
            transform.position = Vector3.Lerp(transform.position, desiredPosition, 0.059f);
        }
    }

    private IEnumerator TimeScaleDown()
    {
        // ミサイル1発目が発射されるタイミングに合わせて回転フラグオン
        // スローモーションにするためにtimeScaleを下げる
        yield return new WaitForSeconds(1.75f);
        Time.timeScale = 0.2f;
        rotflg = true;

        // 2秒間ミサイルに向き続け、スローモーション解除
        // ミサイルの進行方向にカメラを向け、ミサイルの背後にカメラを移動する
        yield return new WaitForSeconds(2 * 0.2f);
        Time.timeScale = 1f;
        rotflg = false;
        transform.rotation = target.rotation;
        transform.Rotate(9, 180, 8);
        Vector3 desiredPosition = target.position + new Vector3(0, 10, -15);
        transform.position = desiredPosition;
        followflg = true;

        // 2秒後、爆発を鑑賞する位置に移動する
        yield return new WaitForSeconds(2f);
        followflg = false;
        transform.position = explosionViwePos.position;
        transform.rotation = explosionViwePos.rotation;

        // ラスボスのHPが0になるタイミングで放射ブラー起動
        yield return new WaitForSeconds(1.3f + (0.32f * 3));
        lastBossDisolve.StartDisovle();

        yield return new WaitForSeconds(3f);
        radialBlur.EnableRadialBlur();

        // 爆発エフェクトが終了するタイミングで放射ブラーオフ
        yield return new WaitForSeconds(2.5f);
        radialBlur.DisableRadialBlur();
    }
}
