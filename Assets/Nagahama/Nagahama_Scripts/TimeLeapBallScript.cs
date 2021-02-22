using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLeapBallScript : MonoBehaviour
{
    [SerializeField] private float _destroyTime = 10f;

    void Start()
    {
        StartCoroutine(nameof(AutoDestroy));
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
}
