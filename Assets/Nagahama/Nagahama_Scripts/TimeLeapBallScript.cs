using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLeapBallScript : MonoBehaviour
{
    [SerializeField] private float _destroyTime = 10f;

    void Start()
    {
        StartCoroutine(nameof(AutoDestroy));
        Debug.Log(gameObject.name + "の自動消滅まで：" + _destroyTime + "秒");
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    public void LaunchBall(Vector3 direction, float power)
    {
        GetComponent<Rigidbody>().AddForce(direction * power, ForceMode.Impulse);
    }

    private IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(_destroyTime);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Asteroid")) {
            StopCoroutine(nameof(AutoDestroy));
            Destroy(gameObject);
            Debug.Log(gameObject.name + "が隕石に当たって消滅した");
        }
    }
}
