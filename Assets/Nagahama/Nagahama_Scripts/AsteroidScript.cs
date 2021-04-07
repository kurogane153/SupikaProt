using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidScript : MonoBehaviour
{
    [SerializeField] private GameObject _explosionEffect;   // 隕石消滅時爆発エフェクト

    [Space(10)]
    [SerializeField] private float _destroyTime = 10f;      // 自動消滅までの時間
    [SerializeField] private int _hp = 6;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private bool _isRotation = false;      // 浮遊中に隕石が自転するか
    [SerializeField] private Vector3 _rotationAxis;         // 自転の回転軸
    [SerializeField] private float _rotationSpeed = 5f;     // 回転速度

    [SerializeField] private Vector3 targetPosition = Vector3.zero;
    [SerializeField] private int _asteroidnumber = 0;


    private bool isMovePause;   // 惑星への接近を停止するか

    public bool IsMovePause
    {
        get { return isMovePause; }
        set { isMovePause = value; }
    }

    #region デバッグ用変数
    [HideInInspector]
    public int _dgb_hp;

    #endregion

    // 速度と接近対象位置を変更
    public void ChangeParam(float speed, Vector3 pos)
    {
        ChangeSpeed(speed);
        SetTargetPosition(pos);
    }

    // 速度を変更
    public void ChangeSpeed(float speed)
    {
        _speed = speed;
    }

    // 隕石にダメージを与える
    public void ReceiveDamage(int damage)
    {
        _hp -= damage;

        if (_hp <= 0) {
            SelfDestroy();
            Instantiate(_explosionEffect, transform.position, Quaternion.identity);

            GameClearOver_Process.GameClearCount++;

            if (_asteroidnumber == AsteroidWaveManager._asteroidnum)
            {
                AsteroidWaveManager._instansflg = true;
                AsteroidWaveManager._instantiatePosition = this.transform.position;
            }
        }
    }
    
    // 接近対象位置を変更
    public void SetTargetPosition(Vector3 pos)
    {
        targetPosition = pos;
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
        if (isMovePause) return;

        // 地球に接近
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime);

        if (_isRotation) {
            // 自転
            Quaternion rot = Quaternion.AngleAxis(_rotationSpeed, _rotationAxis);
            transform.rotation = transform.rotation * rot;
        }

    }
  
    // 時間経過で自動消滅させる
    private IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(_destroyTime);
        TargetCollider[] targetColliders = GetComponentsInChildren<TargetCollider>();
        foreach (var tarcol in targetColliders) {
            RectInAsteroidContainer.Instance.targetColliders.Remove(tarcol);
        }

        if (ReticleController.Instance._userSuperAimAssistSystemFlags) {
            ReticleController.Instance.SelectFirstButton();
        }
        
        Destroy(gameObject);
    }

    // 消滅時の処理まとめ
    private void SelfDestroy()
    {
        StopCoroutine(nameof(AutoDestroy));
        TargetCollider[] targetColliders = GetComponentsInChildren<TargetCollider>();
        foreach(var tarcol in targetColliders) {
            RectInAsteroidContainer.Instance.targetColliders.Remove(tarcol);
        }
        Destroy(gameObject);
        if (ReticleController.Instance._userSuperAimAssistSystemFlags) {
            ReticleController.Instance.SelectFirstButton();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }    

    private void Dbg()
    {
        _dgb_hp = _hp;
    }
}
