using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameClearOver_Process : MonoBehaviour
{
    
    public static int GameClearCount = 0;

    #region 長浜追加フィールド
    private Vector3 impactPos;  // 隕石が衝突した位置

    [Header("衝突イベントカメラ")]
    public KillCameraScript ConflictEventCamera;

    [Header("イベントカメラが地球からどの程度離れるか")]
    public float EventCameraDistance = 750f;

    [SerializeField] private SoundPlayer _soundPlayer;
    [SerializeField] private AudioClip _se_Explosion;
    [SerializeField] private AudioClip _se_Explosion2;

    [SerializeField] private GameObject _halo;

    [Header("ラスボスによるゲームオーバー鑑賞地点")]
    [SerializeField] private Transform _lastBossGameOverViewPos;

    [Header("大気圏突入エフェクト")]
    [SerializeField] private GameObject _atmosphericEntryEffect;

    [Header("ラスボスによるゲームオーバーエフェクト")]
    [SerializeField] private ParticleSystem _lastBossGameOverEffect;

    [Header("小エクスプロージョン")]
    [SerializeField] private GameObject _smallTeraExplosion;

    [Header("テラエクスプロージョン")]
    [SerializeField] private GameObject _teraExplosion;    

    [Header("世界崩壊マテリアル")]
    [SerializeField] private Material _hellMaterial;

    #endregion

    [Header("ゲームオーバーまでの隕石の個数")]
    public int GameOverAsteroid = 3;

    [Header("ゲームオーバー・ゲームクリアのリロード時間")]
    public float GameOverReloadTime = 3.5f;

    [Header("MinimapObject")]
    public GameObject _minimapobj;

    [Header("Set RoberiaAsteroid Prefab")]
    //隕石プレハブ
    public GameObject _roberiaPrefab;

    [Header("表示するテキストナンバー")]
    [SerializeField]
    public int[] _textNo;

    [Header("隕石衝突時のテキストの表示秒数")]
    public int drawingTime = 5;

    [Header("TextWindowManager")]
    [SerializeField]
    public TextWindowManager _textmanager;

    private int GameOverCount;

    private bool _doubleasteroidflg = false;

    void Start()
    {
        GameObject parentObject = gameObject.transform.parent.gameObject;
        if(parentObject.name == "Earth")
        {
            GameOverCount = GameClearOverManager._gameoverCount;
        }
        else
        {
            GameOverCount = GameClearOverManager._gameoverCountColony;
        }

        if(GameOverCount > 0)
        {
            _minimapobj.GetComponent<MinimapObjectScript>().ChangeColor(GameOverCount - 1);
        }

        parentObject = null;

        if (_soundPlayer == null) {
            if ((_soundPlayer = GetComponentInChildren<SoundPlayer>()) == null) {
                Debug.LogError(gameObject.name + "の" + nameof(_soundPlayer) + "が空です");
            }
            Debug.Log(gameObject.name + "は、子要素にアタッチされているAudioSourceを自動的に" + nameof(_soundPlayer) + "にアタッチしました");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Asteroid")
        {
            StartCoroutine( HitGameOverCount(other));
            // 長浜変更点↓
            // 衝突した隕石の座標を取得、地球から隕石へのベクトルを作り
            // そのベクトルを長くして衝突イベントカメラが来てほしい座標を作る
            // 作った座標と地球のgameObjectを引数で渡しながら衝突イベントカメラを起動している。
            
        }
        if (GameOverCount == 2)
        {
            _textmanager.TextWindowOn(8);
            Invoke("TextClose", drawingTime);
        }
    }

    void TextClose()
    {
        _textmanager.TextWindowOff();
    }

    IEnumerator HitGameOverCount(Collider asteroid)
    {
        AsteroidScript asteroidSC = asteroid.GetComponent<AsteroidScript>();
        impactPos = asteroid.transform.position;
        Vector3 desiredPos = (impactPos - transform.position).normalized * EventCameraDistance + transform.position;
        Vector3 explosionpos = (impactPos - transform.position) + transform.position;
        bool isLastBossImpact = asteroid.name == _roberiaPrefab.name;

        _soundPlayer.PlaySE(_se_Explosion2);

        if (asteroidSC.GetAsteroidNumber() == AsteroidWaveManager._asteroidnum) {
            _doubleasteroidflg = true;
            GameClearCount += 2;
        }

        GameOverCount++;
        if (isLastBossImpact) {
            GameOverCount = 3;
            if(_lastBossGameOverViewPos != null) {
                desiredPos = _lastBossGameOverViewPos.position;
            }
            _lastBossGameOverEffect.Play();
            GameClearOverManager.isLastBossGameOver = true;

        } else {
            Instantiate(_atmosphericEntryEffect, explosionpos, Quaternion.LookRotation((explosionpos - transform.position).normalized));
        }

        if (GameOverCount >= GameOverAsteroid) {
            ConflictEventCamera.ConflictEventCameraActive(explosionpos, desiredPos, GameOverReloadTime);
            Debug.Log("death");

        } else {
            ConflictEventCamera.ConflictEventCameraActive(explosionpos, desiredPos);
            Debug.Log("生き残り");
        }

        yield return new WaitForSeconds(2f);

        if (GameOverCount >= GameOverAsteroid) {            
            GameOverProcess(isLastBossImpact);
            Debug.Log("death");
        } else {
            Instantiate(_smallTeraExplosion, explosionpos, Quaternion.identity);
            Debug.Log("生き残り");
        }

        asteroidSC.ReceiveDamage(9999);
        _soundPlayer.PlaySE(_se_Explosion);

        if (isLastBossImpact) {
            yield return new WaitForSeconds(1.9f);
            GetComponent<MeshRenderer>().enabled = false;
            _halo.SetActive(false);
        }

        _minimapobj.GetComponent<MinimapObjectScript>().ChangeColor(GameOverCount - 1);
    }

    void GameOverProcess(bool isLastBoss)
    {
        Vector3 explosionpos = (impactPos - transform.position) + transform.position;

        if (!isLastBoss) {
            Instantiate(_teraExplosion, explosionpos, Quaternion.identity);
        }
        
        Invoke("ChangeMaterial", 2.0f);
        GameClearOverManager._gameoverCount = 0;
        GameClearOverManager._gameoverCountColony = 0;
        BGMManagerScript.Instance.StopBGM();
    }

    void ChangeMaterial()
    {
        GetComponent<MeshRenderer>().material = _hellMaterial;
    }

    public void SetGameClearCount(int count)
    {
        GameClearCount = count;
    }

    public int GetGameClearCount()
    {
        return GameClearCount;
    }

    public int GetGameOverCount()
    {
        return GameOverCount;
    }

    public int GetGameOverAsteroidCount()
    {
        return GameOverAsteroid;
    }

    public bool GetDoubleAsteroid()
    {
        return _doubleasteroidflg;
    }
    public void SetDoubleAsteroid(bool flg)
    {
        _doubleasteroidflg = flg;
    }
}
