using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissileScript : MonoBehaviour
{
    [SerializeField] private SoundPlayer _soundPlayer;
    [SerializeField] private AudioClip _se_Explosion;

    [Space(10)]
    [SerializeField] private float _destroyTime = 10f;

    private Transform targetPoint;
    private Rigidbody rb;

    private Vector3 velocity;
    private Vector3 position;
    private Vector3 halfwayPoint;
    private float impactTime = 1.5f;

    void Start()
    {
        if (rb == null) {
            rb = GetComponent<Rigidbody>();
            position = transform.position;
        }

        if (_soundPlayer == null) {
            if ((_soundPlayer = GetComponentInChildren<SoundPlayer>()) == null) {
                Debug.LogError(gameObject.name + "の" + nameof(_soundPlayer) + "が空です");
            }
            Debug.Log(gameObject.name + "は、子要素にアタッチされているAudioSourceを自動的に" + nameof(_soundPlayer) + "にアタッチしました");
        }

        StartCoroutine(nameof(AutoDestroy));
        Debug.Log(gameObject.name + "の自動消滅まで：" + _destroyTime + "秒");
    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
        HomingMove();
    }

    public void LaunchMissile(Transform targetP, Transform halfwaypoint, float impacttime, Vector3 velo)
    {
        if (rb == null) {
            rb = GetComponent<Rigidbody>();
            position = transform.position;
            velocity = velo.normalized * 100f;
        }
        targetPoint = targetP;
        impactTime = impacttime;
    }

    private void HomingMove()
    {
        Vector3 acceleration = Vector3.zero;

        Vector3 vec = targetPoint.position - position;
        acceleration += (vec - velocity * impactTime) * 2f / (impactTime * impactTime);

        impactTime -= Time.deltaTime;
        if (impactTime < 0f) {
            return;
        }

        velocity += acceleration * Time.deltaTime;
        position += velocity * Time.deltaTime;
        transform.position = position;
    }

    #region Coroutine
    private IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(_destroyTime);
        Destroy(gameObject);
    }
    #endregion

    #region OnCollision
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Asteroid")) {
            StopCoroutine(nameof(AutoDestroy));
            _soundPlayer.gameObject.transform.parent = null;
            Destroy(_soundPlayer.gameObject, 3f);
            _soundPlayer.PlaySE(_se_Explosion);
            Destroy(gameObject);
            Debug.Log(gameObject.name + "が隕石に当たって消滅した");
        }
    }
    #endregion
}
