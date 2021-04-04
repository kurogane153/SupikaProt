using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 自動追尾ミサイル
public class HomingMissileScript : MonoBehaviour
{
    [Header("音関係のフィールド")]
    [SerializeField] private SoundPlayer _soundPlayer;
    [SerializeField] private AudioClip _se_Explosion;

    [Header("爆発エフェクト"), Space(10)]
    [SerializeField] private GameObject _explosionEffect;   // 消滅時爆発エフェクト

    [Space(10)]
    [SerializeField] private float _destroyTime = 10f;      // 自動消滅までの時間
    [SerializeField] private float _lockAtSpeed = 10f;      // ターゲットの方を向くスピード
    [SerializeField] private bool _onTargetDestroySelfDestroy;  // 対象が消滅したときに自分も消滅するか

    private Transform targetTransform;

    private Vector3 velocity;
    private Vector3 position;
    private Vector3 lastTargetPosition; // 最後に狙っていた座標
    private Vector3 lastAcceleration;   // 最後の加速度
    private Vector3 halfwayPoint;       // 飛行挙動の中間地点
    private float impactTime = 1.5f;    // 命中までの時間
    private int giveDamage;
   

    void Start()
    {
        position = transform.position;

        if (_soundPlayer == null) {
            if ((_soundPlayer = GetComponentInChildren<SoundPlayer>()) == null) {
                Debug.LogError(gameObject.name + "の" + nameof(_soundPlayer) + "が空です");
            }
            Debug.Log(gameObject.name + "は、子要素にアタッチされているAudioSourceを自動的に" + nameof(_soundPlayer) + "にアタッチしました");
        }

        StartCoroutine(nameof(AutoDestroy));
        Debug.Log(gameObject.name + "の自動消滅まで：" + _destroyTime + "秒");
    }

    private void FixedUpdate()
    {
        TargetDestroyedCheck();
        if (targetTransform != null) {
            HomingMove();
        } else {
            NonTargetMove();
        }
    }

    /// <summary>
    /// PlayerShotなどから呼ばれる
    /// </summary>
    /// <param name="targetP">対象</param>
    /// <param name="halfwaypoint">飛行挙動の中間地点</param>
    /// <param name="impacttime">命中までの時間</param>
    /// <param name="vec">発射した瞬間に飛んで行く方向</param>
    /// <param name="initpower">発射する力</param>
    /// <param name="damage">与ダメージ</param>
    public void LaunchMissile(Transform targetP, Transform halfwaypoint, float impacttime, Vector3 vec, float initpower, int damage)
    {
        position = transform.position;
        velocity = vec.normalized * initpower;
        targetTransform = targetP;
        impactTime = impacttime;
        giveDamage = damage;
        transform.rotation = Quaternion.LookRotation(vec * -1f).normalized;
        gameObject.SetActive(true);
    }

    // 対象に向かっていくまでの挙動
    private void HomingMove()
    {
        Vector3 acceleration = Vector3.zero;

        Vector3 vec = targetTransform.position - position;
        acceleration += (vec - velocity * impactTime) * 2f / (impactTime * impactTime);

        impactTime -= Time.deltaTime;

        // 命中までの時間になったらその位置についているので対象にダメージを与えて自分は消滅する
        if (impactTime < 0f) {
            targetTransform.GetComponent<TargetCollider>().ReceiveDamage(giveDamage);
            StopCoroutine(nameof(AutoDestroy));
            SelfDestroy();
            return;
        }

        velocity += acceleration * Time.deltaTime;
        position += velocity * Time.deltaTime;

        Vector3 diff = transform.position - position;

        // 徐々に対象への方向に回転する
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(diff).normalized, Time.deltaTime * _lockAtSpeed);
        transform.position = position;
        
        // 狙っていた座標と加速度を保存している
        lastTargetPosition = vec * 10000f;
        lastAcceleration = acceleration;
    }

    // 対象が消失したときの挙動
    private void NonTargetMove()
    {
        velocity += lastAcceleration * Time.fixedDeltaTime;
        position += velocity * Time.fixedDeltaTime;

        Vector3 diff = transform.position - position;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(diff).normalized, Time.fixedDeltaTime * _lockAtSpeed);
        transform.position = position;
    }

    // 対象が消滅していないか
    private void TargetDestroyedCheck()
    {
        if(targetTransform == null && _onTargetDestroySelfDestroy) {
            SelfDestroy();
        }
    }

    // 消滅時処理まとめ
    private void SelfDestroy()
    {
        _soundPlayer.gameObject.transform.parent = null;
        _soundPlayer.PlaySE(_se_Explosion);
        _soundPlayer.DestroyCall(3f);

        Instantiate(_explosionEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    #region Coroutine
    private IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(_destroyTime);
        SelfDestroy();
    }
    #endregion

    #region OnCollision
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Asteroid")) {
            //collision.gameObject.GetComponent<AsteroidScript>().ReceiveDamage(giveDamage);
            //StopCoroutine(nameof(AutoDestroy));
            //SelfDestroy();
            //Debug.Log(gameObject.name + "が隕石に当たって消滅した");
        }
    }
    #endregion
}
