using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyGenerator3 : MonoBehaviour
{
    [Header("Set Asteroid Prefab")]
    //敵プレハブ
    public GameObject enemyPrefab;

    [Header("Set Interval Min and Max")]
    //時間間隔の最小値
    [Range(1f, 5f)]
    public float minTime = 2f;
    //時間間隔の最大値
    [Range(5f, 20f)]
    public float maxTime = 5f;

    [Header("隕石の速さ")]
    //隕石の速さ
    public float _spawnedAsteroidSpeed = 50f;

    [Header("X座標の最大値と最小値")]
    //X座標の最小値
    [Range(-100f, 0f)]
    public float xMinPosition = -10f;
    //X座標の最大値
    [Range(0f, 100f)]
    public float xMaxPosition = 10f;

    [Header("Y座標の最大値と最小値")]
    //Y座標の最小値
    [Range(-100f, 0f)]
    public float yMinPosition = 0f;
    //Y座標の最大値
    [Range(0f, 100f)]
    public float yMaxPosition = 10f;

    [Header("Z座標の最大値と最小値")]
    //Z座標の最小値
    [Range(-100f, 0f)]
    public float zMinPosition = 0f;
    //Z座標の最大値
    [Range(0f, 100f)]
    public float zMaxPosition = 0f;

    //敵生成時間間隔
    private float interval;
    //経過時間
    private float time = 0f;

    //カメラに表示されているか
    private bool isRendered = false;
    private Vector3 SpikaPos = new Vector3 (-925, 0, 0);

    //メインカメラに付いているタグ名
    private const string MAIN_CAMERA_TAG_NAME = "MainCamera";

    void Start()
    {
        //時間間隔を決定する
        interval = GetRandomTime();
    }

    void Update()
    {
        //時間計測
        time += Time.deltaTime;

        //経過時間が生成時間になったとき(生成時間より大きくなったとき)
        if (time > interval)
        {
            if (isRendered)
            {
                //enemyをインスタンス化する(生成する)
                GameObject asteroid = Instantiate(enemyPrefab);
                //生成した敵の位置をランダムに設定する
                asteroid.transform.position = GetRandomPosition();
                asteroid.GetComponent<AsteroidScript>().ChangeParam(_spawnedAsteroidSpeed, SpikaPos);
                //経過時間を初期化して再度時間計測を始める
                time = 0f;
                //次に発生する時間間隔を決定する
                interval = GetRandomTime();
                isRendered = false;
            }
        }

    }

    //ランダムな時間を生成する関数
    private float GetRandomTime()
    {
        return Random.Range(minTime, maxTime);
    }

    //ランダムな位置を生成する関数
    private Vector3 GetRandomPosition()
    {
        //それぞれの座標をランダムに生成する
        float x = Random.Range(xMinPosition + this.transform.position.x, xMaxPosition + this.transform.position.x);
        float y = Random.Range(yMinPosition + this.transform.position.y, yMaxPosition + this.transform.position.y);
        float z = Random.Range(zMinPosition + this.transform.position.z, zMaxPosition + this.transform.position.z);

        //Vector3型のPositionを返す
        return new Vector3(x, y, z);
    }

    private void OnWillRenderObject()
    {
        if (Camera.current.tag == MAIN_CAMERA_TAG_NAME)
        {
            isRendered = true;
        }
    }
}
