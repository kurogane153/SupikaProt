using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidScript : MonoBehaviour
{
    [SerializeField] private float _destroyTime = 10f;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private bool _isRotation = false;
    [SerializeField] private Vector3 _rotationAxis;
    [SerializeField] private float _rotationSpeed = 5f;

    public void ChangeSpeed(float speed)
    {
        _speed = speed;
    }

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
        transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, _speed * Time.deltaTime);

        if (_isRotation) {
            Quaternion rot = Quaternion.AngleAxis(_rotationSpeed, _rotationAxis);
            transform.rotation = transform.rotation * rot;
        }
        
    }

    private IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(_destroyTime);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet")) {
            StopCoroutine(nameof(AutoDestroy));
            
            Debug.Log(gameObject.name + "が弾に当たって消滅した");
        }
    }
}
