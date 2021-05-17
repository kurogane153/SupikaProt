using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsteroidScript : MonoBehaviour
{
    [Header("サウンド系")]
    [SerializeField] private SoundPlayer _soundPlayer;
    [SerializeField] private AudioClip _se_Explosion;
    [SerializeField] private AudioClip _se_ExplosionStart1;
    [SerializeField] private AudioClip _se_ExplosionStart2;

    [SerializeField] private ParticleSystem _explosionEffect;   // 隕石消滅時爆発エフェクト
    [SerializeField] private ParticleSystem[] _childParticles;  // 子要素にしてある軌跡などのパーティクル

    [Space(10)]
    [SerializeField] private float _destroyTime = 10f;      // 自動消滅までの時間
    [SerializeField] private int _hp = 6;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private bool _isRotation = false;      // 浮遊中に隕石が自転するか
    [SerializeField] private bool _startLookAtTarget;       // 生成時ターゲットに向くか
    [SerializeField] private Vector3 _rotationAxis;         // 自転の回転軸
    [SerializeField] private float _rotationSpeed = 5f;     // 回転速度
    [SerializeField] private float _explosionEffectDelaySec;   // 爆発エフェクトのディレイ

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

    public void ChangeRotation(bool flg)
    {
        _isRotation = flg;
    }

    public int GetAsteroidNumber()
    {
        return _asteroidnumber;
    }

    public int GetAsteroidHp()
    {
        return _hp;
    }

    // 隕石にダメージを与える
    public void ReceiveDamage(int damage)
    {
        _hp -= damage;

        if (_hp <= 0) {
            if(0 < _explosionEffectDelaySec) {
                StartCoroutine(nameof(DelaySelfDestroy));
            } else {
                _explosionEffect.Play();
                _explosionEffect.transform.parent = null;

                foreach(var particle in _childParticles) {
                    particle.Stop();

                    particle.transform.parent = null;
                }

                SelfDestroy();
                
            }
            if (SceneManager.GetActiveScene().name == "KuroganeScene")
            {
                RoberiaManager.Instance.RoberiaAsteroidDestroyCount(1);
            }
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

        if (_startLookAtTarget) {
            transform.LookAt(targetPosition);
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

    private IEnumerator DelaySelfDestroy()
    {
        _soundPlayer.gameObject.transform.parent = null;

        _soundPlayer.PlaySE(_se_ExplosionStart1);
        _soundPlayer.PlaySE(_se_ExplosionStart2);

        foreach (var particle in _childParticles) {
            var main = particle.main;
            main.loop = false;
            particle.Stop();
            particle.transform.parent = null;
        }

        yield return new WaitForSeconds(_explosionEffectDelaySec);
        _explosionEffect.Play();
        _explosionEffect.transform.parent = null;

        _soundPlayer.PlaySE(_se_Explosion, 3f);

        SelfDestroy();        
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }    

    private void Dbg()
    {
        _dgb_hp = _hp;
    }
}