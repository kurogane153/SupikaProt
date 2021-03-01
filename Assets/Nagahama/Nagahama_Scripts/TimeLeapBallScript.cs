using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLeapBallScript : MonoBehaviour
{
    [SerializeField] private float _destroyTime = 10f;

    private Transform earthTransform;
    private Transform targetPoint;
    private Rigidbody rb;
    private Vector3 direction;
    private Quaternion angleAxis;

    void Start()
    {
        if (rb == null) {
            rb = GetComponent<Rigidbody>();
        }

        StartCoroutine(nameof(AutoDestroy));
        Debug.Log(gameObject.name + "の自動消滅まで：" + _destroyTime + "秒");
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //BallMovement();
        BallMovePhys();
    }

    public void LaunchBall(Vector3 dir, float pow, Quaternion angleaxis, Transform targetP)
    {
        if(rb == null) {
            rb = GetComponent<Rigidbody>();
        }
        targetPoint = targetP;
        //rb.AddForce(dir * pow, ForceMode.Impulse);
        
    }

    private void BallMovement()
    {
        Vector3 newPos = transform.position;

        newPos -= earthTransform.position;
        newPos = angleAxis * newPos;
        newPos += earthTransform.position;

        transform.position = newPos;
    }

    private void BallMovePhys()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.TransformDirection(new Vector3(0, 0, 1)) * 10000f, Time.deltaTime * 1500);        
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
