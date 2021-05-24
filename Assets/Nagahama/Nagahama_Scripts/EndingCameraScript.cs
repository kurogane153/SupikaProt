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
    public Transform playerBehindPos;

    [Header("ラスボスが爆発するさまを見る座標"), Space(5)]
    public Transform explosionViwePos;

    [Header("ラスボスが爆発するさまを見る座標_2"), Space(5)]
    public Transform explosionViwePos_2;

    [Header("ラスボスのAsteroidScript"), Space(5)]
    public AsteroidScript lastBossAsteroidSC;

    [Header("ラスボスのディゾルブ制御スクリプト"), Space(5)]
    public LastBossDisolveEffectScript lastBossDisolve;

    [Header("ミサイルのループ回数"), Space(5)]
    public int _looptimes;

    #endregion

    [Space(10)]
    [HideInInspector] public bool rotflg = false;
    [HideInInspector] public bool panflg = false;
    [HideInInspector] public bool followflg = false;
    [HideInInspector] public bool diagonallyMoveflg = false;

    RadialBlur radialBlur;
    private bool isLastBossExplosion;

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

        if (panflg) {
            transform.Translate(-Vector3.forward * Time.deltaTime * 20f);
        }

        if(followflg && target != null) {
            Vector3 desiredPosition = target.position + new Vector3(0, 10, -90);
            transform.position = Vector3.Lerp(transform.position, desiredPosition, 0.059f);
        }

        if (diagonallyMoveflg) {
            Vector3 desiredPosition = transform.position + new Vector3(0f, 0f, -1f);
            transform.position = desiredPosition;
        }

        if(lastBossAsteroidSC != null && lastBossAsteroidSC.GetAsteroidHp() <= 0 && !isLastBossExplosion) {
            StartCoroutine(nameof(LastBossExplosionStart));
            isLastBossExplosion = true;
        }
    }

    private IEnumerator TimeScaleDown()
    {
        yield return new WaitForSeconds(_slowMotionStartDelay);

        // ミサイル1発目が発射されるタイミングに合わせて回転フラグオン
        // スローモーションにするためにtimeScaleを下げる
        Time.timeScale = 0.2f;
        rotflg = true;
        yield return new WaitForSeconds(_slowMotionEndDelay * 0.2f);

        // _slowMotionEndDelay秒間ミサイルに向き続け、スローモーション解除
        // ミサイルの進行方向にカメラを向け、ミサイルの背後にカメラを移動する
        rotflg = false;
        Time.timeScale = 1f;
        transform.position = playerBehindPos.position;
        transform.rotation = playerBehindPos.rotation;
        panflg = true;
        yield return new WaitForSeconds(1.5f);

        // 数秒間プレイヤーの背後からミサイルが飛ぶさまを見せる
        panflg = false;
        
        transform.rotation = target.rotation;
        transform.Rotate(9, 180, 8);
        Vector3 desiredPosition = target.position + new Vector3(0, 10, -15);
        transform.position = desiredPosition;
        followflg = true;
        yield return new WaitForSeconds(_moveExplosionPosDelay);

        // _moveExplosionPosDelay秒後、爆発を鑑賞する位置に移動する
        followflg = false;
        diagonallyMoveflg = true;
        transform.position = explosionViwePos_2.position;
        transform.rotation = explosionViwePos_2.rotation;
        
    }

    private IEnumerator LastBossExplosionStart()
    {        
        lastBossDisolve.StartDisovle();
        yield return new WaitForSeconds(0.4f);
        diagonallyMoveflg = false;
        transform.position = explosionViwePos.position;
        transform.rotation = explosionViwePos.rotation;
        panflg = true;
        yield return new WaitForSeconds(1.6f);
        panflg = false;

        yield return new WaitForSeconds(_radiulBlurStartDelay - 3f);

        // ラスボスのHPが0になるタイミングで放射ブラー起動
        radialBlur.EnableRadialBlur();
        yield return new WaitForSeconds(_radiulBlurEndDelay);

        // 爆発エフェクトが終了するタイミングで放射ブラーオフ
        radialBlur.DisableRadialBlur();
    }
}
