using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidScript : MonoBehaviour
{
    [SerializeField] private float _destroyTime = 10f;
    [SerializeField] private int _hp = 6;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private bool _isRotation = false;
    [SerializeField] private Vector3 _rotationAxis;
    [SerializeField] private float _rotationSpeed = 5f;

    #region デバッグ用変数
    [Watch, HideInInspector]
    public int _dgb_hp;

    #endregion


    public void ChangeSpeed(float speed)
    {
        _speed = speed;
    }

    public void ReceiveDamage(int damage)
    {
        _hp -= damage;

        if (_hp <= 0) {
            SelfDestroy();
        }
    }

    void Start()
    {
        StartCoroutine(nameof(AutoDestroy));
        Debug.Log(gameObject.name + "の自動消滅まで：" + _destroyTime + "秒");

        
    }

    void Update()
    {
        Dbg();
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

    private void SelfDestroy()
    {
        StopCoroutine(nameof(AutoDestroy));
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet")) {
            //SelfDestroy();
            //Debug.Log(gameObject.name + "が弾に当たって消滅した");
        }
    }

    private void Dbg()
    {
        _dgb_hp = _hp;
    }
}
