using System.Collections;
using UnityEngine;

public class EndingCameraScript : MonoBehaviour
{
    #region Parametors
    [Header("撃破演出開始からスローモーション開始までのディレイ")]
    public float _slowMotionStartDelay = 1.75f;

    [Header("スローモーションの持続時間"), Space(5)]
    public float _slowMotionEndDelay = 2f;

    [Header("スローモーション終了からラスボスが爆発する座標に移動するまでの時間"), Space(5)]
    public float _moveExplosionPosDelay = 2f;

    [Header("座標移動からラスボスのディゾルブ効果を開始するまでの時間"), Space(5)]
    public float _disolveEffectStartDelay = 1.3f;

    [Header("ミサイル1ループあたりの時間"), Space(5)]
    public float _missileOneLoopPerTime = 0.35f;

    [Header("ディゾルブ開始から放射ブラー開始までの時間"), Space(5)]
    public float _radiulBlurStartDelay = 6f;

    [Header("放射ブラーの持続時間"), Space(5)]
    public float _radiulBlurEndDelay = 2.5f;

    [Header("ミサイルに注目してカメラで追うときのスピード"), Space(5)]
    public float lookTargetSpeed = 2f;

    [Header("追尾するターゲット"), Space(5)]
    public Transform target;

    [Header("ラスボスが爆発するさまを見る座標"), Space(5)]
    public Transform explosionViwePos;    

    [Header("ラスボスのディゾルブ制御スクリプト"), Space(5)]
    public LastBossDisolveEffectScript lastBossDisolve;

    [Header("ミサイルのループ回数"), Space(5)]
    public int _looptimes;

    #endregion

    [Space(10)]
    [HideInInspector] public bool rotflg = false;
    [HideInInspector] public bool followflg = false;
    
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
            float step = lookTargetSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 10.0f);
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
        yield return new WaitForSeconds(_slowMotionStartDelay);
        Time.timeScale = 0.2f;
        rotflg = true;

        // 2秒間ミサイルに向き続け、スローモーション解除
        // ミサイルの進行方向にカメラを向け、ミサイルの背後にカメラを移動する
        yield return new WaitForSeconds(2f * 0.2f);
        Time.timeScale = 1f;
        rotflg = false;
        transform.rotation = target.rotation;
        transform.Rotate(9, 180, 8);
        Vector3 desiredPosition = target.position + new Vector3(0, 10, -15);
        transform.position = desiredPosition;
        followflg = true;

        // 2秒後、爆発を鑑賞する位置に移動する
        yield return new WaitForSeconds(_moveExplosionPosDelay);
        followflg = false;
        transform.position = explosionViwePos.position;
        transform.rotation = explosionViwePos.rotation;

        // ディゾルブシェーダーアニメーション起動
        yield return new WaitForSeconds(_disolveEffectStartDelay + (0.35f * _looptimes));
        lastBossDisolve.StartDisovle();

        // ラスボスのHPが0になるタイミングで放射ブラー起動
        yield return new WaitForSeconds(_radiulBlurStartDelay);
        radialBlur.EnableRadialBlur();

        // 爆発エフェクトが終了するタイミングで放射ブラーオフ
        yield return new WaitForSeconds(_radiulBlurEndDelay);
        radialBlur.DisableRadialBlur();
    }
}
