using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpwanerScript : MonoBehaviour
{
    [SerializeField] private GameObject[] _asteroidPrefab;
    [SerializeField] private float _spawnDuration = 4f;

    private float spawnTimeRemain;

    void Start()
    {
        spawnTimeRemain += _spawnDuration;
        transform.LookAt(Vector3.zero);
        Debug.Log(_asteroidPrefab.Length);
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        spawnTimeRemain -= Time.deltaTime;
        if(spawnTimeRemain <= 0f) {
            spawnTimeRemain += _spawnDuration;
            Instantiate(_asteroidPrefab[Random.Range(0, _asteroidPrefab.Length - 1)], transform.position, Quaternion.identity);
        }
    }
}
