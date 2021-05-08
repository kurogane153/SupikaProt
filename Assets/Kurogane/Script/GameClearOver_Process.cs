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
            HitGameOverCount(other);
            // 長浜変更点↓
            // 衝突した隕石の座標を取得、地球から隕石へのベクトルを作り
            // そのベクトルを長くして衝突イベントカメラが来てほしい座標を作る
            // 作った座標と地球のgameObjectを引数で渡しながら衝突イベントカメラを起動している。
            other.GetComponent<AsteroidScript>().ReceiveDamage(9999);
            _soundPlayer.PlaySE(_se_Explosion);
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

    void HitGameOverCount(Collider asteroid)
    {
        if(asteroid.GetComponent<AsteroidScript>().GetAsteroidNumber() == AsteroidWaveManager._asteroidnum)
        {
            _doubleasteroidflg = true;
            GameClearCount += 2;
        }

        impactPos = asteroid.transform.position;
        Vector3 desiredPos = (impactPos - transform.position).normalized * EventCameraDistance + transform.position;
        GameOverCount++;
        if (asteroid.name == _roberiaPrefab.name)
        {
            GameOverCount = 3;
        }
        _minimapobj.GetComponent<MinimapObjectScript>().ChangeColor(GameOverCount - 1);

        if (GameOverCount >= GameOverAsteroid) {
            GameOverProcess();
            ConflictEventCamera.ConflictEventCameraActive(gameObject, desiredPos, GameOverReloadTime);
        } else {
            ConflictEventCamera.ConflictEventCameraActive(gameObject, desiredPos);
        }
    }

    void GameOverProcess()
    {
        Vector3 explosionpos = (impactPos - transform.position) + transform.position;
        Instantiate(_teraExplosion, explosionpos, Quaternion.identity);
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
