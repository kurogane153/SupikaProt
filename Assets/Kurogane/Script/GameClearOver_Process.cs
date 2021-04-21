using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameClearOver_Process : MonoBehaviour
{

    private int GameOverCount = 0;
    private int GameOverCountSpika = 0;
    public static int GameClearCount = 0;
    public static bool isclear = false;
    public bool Spikafig = false;

    public GameObject GameOverCountText = null; // Textオブジェクト
    public GameObject GameOverCountTextSpika = null; // Textオブジェクト
    public GameObject GameClearCountText = null; // Textオブジェクト

    #region 長浜追加フィールド
    private Vector3 impactPos;  // 隕石が衝突した位置

    [Header("衝突イベントカメラ")]
    public KillCameraScript ConflictEventCamera;

    [Header("イベントカメラが地球からどの程度離れるか")]
    public float EventCameraDistance = 750f;

    [SerializeField] private SoundPlayer _soundPlayer;
    [SerializeField] private AudioClip _se_Explosion;

    [Header("テラエクスプロージョン")]
    [SerializeField] private GameObject _teraExplosion;
    [Header("世界崩壊マテリアル")]
    [SerializeField] private Material _hellMaterial;

    #endregion

    [Header("ゲームオーバーまでの隕石の個数")]
    public int GameOverAsteroid = 3;

    [Header("ゲームクリアまでの隕石の個数")]
    public int GameClearAsteroid = 20;

    [Header("ゲームオーバーのリロード時間")]
    public float GameOverReloadTime = 3.5f;

   

    void Start()
    {
        GameClearCount = 0;

        if (_soundPlayer == null) {
            if ((_soundPlayer = GetComponentInChildren<SoundPlayer>()) == null) {
                Debug.LogError(gameObject.name + "の" + nameof(_soundPlayer) + "が空です");
            }
            Debug.Log(gameObject.name + "は、子要素にアタッチされているAudioSourceを自動的に" + nameof(_soundPlayer) + "にアタッチしました");
        }
    }

    void Update()
    {

        if (GameOverCount >= GameOverAsteroid)
        { 
            Invoke("DelayMethod", GameOverReloadTime);
        }

        if (GameOverCountSpika >= GameOverAsteroid)
        {
            Invoke("DelayMethod", GameOverReloadTime);
        }

        if (GameClearCount >= GameClearAsteroid)
        {
            isclear = true;
            Invoke("DelayMethod", 3.0f);
        }

        GameOverCountText.GetComponent<Text>().text = "地球滅亡まで残り:" + (GameOverAsteroid - GameOverCount);
        GameOverCountTextSpika.GetComponent<Text>().text = "コロニー滅亡まで残り:" + (GameOverAsteroid - GameOverCountSpika);
        GameClearCountText.GetComponent<Text>().text = "防衛成功まで残り:" + (GameClearAsteroid - GameClearCount);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Asteroid")
        {
            HitGameOverCount(other);
            // 長浜変更点↓
            // 衝突した隕石の座標を取得、地球から隕石へのベクトルを作り
            // そのベクトルを長くして衝突イベントカメラが来てほしい座標を作る
            // 作った座標と地球のgameObjectを引数で渡しながら衝突イベントカメラを起動している。
            

            other.GetComponent<AsteroidScript>().ReceiveDamage(9999);
            _soundPlayer.PlaySE(_se_Explosion);

            
        }
        Debug.Log(GameOverCountSpika);
    }

    void HitGameOverCount(Collider asteroid)
    {
        

        if (Spikafig)
        {
            if(asteroid.GetComponent<AsteroidScript>().GetAsteroidNumber() == AsteroidWaveManager._asteroidnum)
            {
            }
            else
            {
                impactPos = asteroid.transform.position;
                Vector3 desiredPos = (impactPos - transform.position).normalized * EventCameraDistance;
                GameOverCountSpika++;

                if (GameOverCountSpika >= GameOverAsteroid) {
                    GameOverProcess();
                    ConflictEventCamera.ConflictEventCameraActive(gameObject, desiredPos, GameOverReloadTime);
                } else {
                    ConflictEventCamera.ConflictEventCameraActive(gameObject, desiredPos);
                }
            }  
        }
        else
        {
            impactPos = asteroid.transform.position;
            Vector3 desiredPos = (impactPos - transform.position).normalized * EventCameraDistance;
            GameOverCount++;

            if (GameOverCount >= GameOverAsteroid) {
                GameOverProcess();
                ConflictEventCamera.ConflictEventCameraActive(gameObject, desiredPos, GameOverReloadTime);
            } else {
                ConflictEventCamera.ConflictEventCameraActive(gameObject, desiredPos);
            }
        }
    }

    void GameOverProcess()
    {
        Vector3 explosionpos = (impactPos - transform.position);
        Instantiate(_teraExplosion, explosionpos, Quaternion.identity);
        Invoke("ChangeMaterial", 2.0f);
        BGMManagerScript.Instance.StopBGM();
    }

    void DelayMethod()
    {
        SceneManager.LoadScene("Result");
    }

    void ChangeMaterial()
    {
        GetComponent<MeshRenderer>().material = _hellMaterial;
    }

    public int GetGameClearCount()
    {
        return GameClearCount;
    }

    public void SetGameClearCount(int count)
    {
        GameClearCount = count;
    }
}
