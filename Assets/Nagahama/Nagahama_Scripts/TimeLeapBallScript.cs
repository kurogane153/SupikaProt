using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLeapBallScript : MonoBehaviour
{
    [SerializeField] private SoundPlayer _soundPlayer;
    [SerializeField] private AudioClip _se_Explosion;

    [Space(10)]
    [SerializeField] private float _destroyTime = 10f;

    private Transform targetPoint;
    private Rigidbody rb;

    void Start()
    {
        if (rb == null) {
            rb = GetComponent<Rigidbody>();
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
        BallMovePhys();
    }

    public void LaunchBall(Transform targetP)
    {
        if(rb == null) {
            rb = GetComponent<Rigidbody>();
        }
        targetPoint = targetP;
    }

    private void BallMovePhys()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.TransformDirection(new Vector3(0, 0, 1)) * 10000f, Time.deltaTime * 1500);        
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
