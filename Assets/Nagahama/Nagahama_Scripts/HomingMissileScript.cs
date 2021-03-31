using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissileScript : MonoBehaviour
{
    [SerializeField] private SoundPlayer _soundPlayer;
    [SerializeField] private AudioClip _se_Explosion;

    [Space(10)]
    [SerializeField] private GameObject _explosionEffect;

    [Space(10)]
    [SerializeField] private float _destroyTime = 10f;
    [SerializeField] private float _lockAtSpeed = 10f;

    private Transform targetTransform;

    private Vector3 velocity;
    private Vector3 position;
    private Vector3 lastTargetPosition;
    private Vector3 lastAcceleration;
    private Vector3 halfwayPoint;
    private float impactTime = 1.5f;
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

    void Update()
    {
       
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

    private void HomingMove()
    {
        Vector3 acceleration = Vector3.zero;

        Vector3 vec = targetTransform.position - position;
        acceleration += (vec - velocity * impactTime) * 2f / (impactTime * impactTime);

        impactTime -= Time.deltaTime;
        if (impactTime < 0f) {
            targetTransform.GetComponent<TargetCollider>().ReceiveDamage(giveDamage);
            StopCoroutine(nameof(AutoDestroy));
            SelfDestroy();
            return;
        }

        velocity += acceleration * Time.deltaTime;
        position += velocity * Time.deltaTime;

        Vector3 diff = transform.position - position;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(diff).normalized, Time.deltaTime * _lockAtSpeed);
        transform.position = position;
        
        lastTargetPosition = vec * 10000f;
        lastAcceleration = acceleration;
    }

    private void NonTargetMove()
    {
        velocity += lastAcceleration * Time.fixedDeltaTime;
        position += velocity * Time.fixedDeltaTime;

        Vector3 diff = transform.position - position;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(diff).normalized, Time.fixedDeltaTime * _lockAtSpeed);
        transform.position = position;
    }

    private void TargetDestroyedCheck()
    {
        if(targetTransform == null) {
            //SelfDestroy();
        }
    }

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
