using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpwanerScript : MonoBehaviour
{
    [SerializeField] private GameObject[] _asteroidPrefab;
    [SerializeField] private float _spawnDuration = 4f;
    [Header("生成した隕石のスピードをスポナーから指定"), Space(10)]
    [SerializeField] private bool _isAsteroidSpeedOverride = false;
    [SerializeField] private float _spawnedAsteroidSpeed = 50f;

    [Header("デバッグ用"), Space(10)]
    [SerializeField] private bool _isDgb = false;
    [SerializeField] private KeyCode _dgb_AsteroidSpawnButton = KeyCode.Alpha1;

    private float spawnTimeRemain;

    void Start()
    {
        spawnTimeRemain += _spawnDuration;
        transform.LookAt(Vector3.zero);
        Debug.Log(gameObject.name + "の隕石プレハブの数:" + _asteroidPrefab.Length);
    }

    void Update()
    {
        #region デバッグ用コマンド
        if (!_isDgb) return;

        if (Input.GetKeyDown(_dgb_AsteroidSpawnButton)) {
            Dbg_AsteroidSpawn();
        }
        #endregion
    }

    private void FixedUpdate()
    {
        spawnTimeRemain -= Time.deltaTime;
        if(spawnTimeRemain <= 0f) {
            AsteroidSpawn();
        }
    }

    private void AsteroidSpawn()
    {
        spawnTimeRemain += _spawnDuration;
        int spawnNum = Random.Range(0, _asteroidPrefab.Length - 1);

        if (_isAsteroidSpeedOverride) {
            GameObject asteroid = Instantiate(_asteroidPrefab[spawnNum], transform.position, Quaternion.identity);
            asteroid.GetComponent<AsteroidScript>().ChangeSpeed(_spawnedAsteroidSpeed);

        } else {
            Instantiate(_asteroidPrefab[spawnNum], transform.position, Quaternion.identity);
        }

    }

    #region デバッグ用関数

    private void Dbg_AsteroidSpawn()
    {
        spawnTimeRemain = 0f;
    }

    #endregion
}
