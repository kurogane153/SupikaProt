using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingPlayerScript : MonoBehaviour
{
    [System.Serializable]
    public class MissileShotSettings
    {

        [SerializeField, Tooltip("ミサイルの発射時初速")]
        public float[] _missileShotPower;

        [SerializeField, Tooltip("次回発射までのクールタイム")]
        public float _missileShotDelay;

        [SerializeField, Tooltip("複数ターゲット発射時、次ターゲット発射までの待機時間")]
        public float _multiTargetMissileDelay;

        [SerializeField, Tooltip("アニメーショントリガーの名前")]
        public string _animationTriggerName;

        [SerializeField, Tooltip("アニメーションの長さ")]
        public float _animWaitTime;

        [SerializeField, Tooltip("ミサイルの発射方向")]
        public Transform[] _halfwayPoints;

        [SerializeField, Tooltip("着弾までの時間")]
        public float[] _impactTimes;

        [SerializeField, Tooltip("発射してから生成までの待機時間")]
        public float[] _instantiateTimes;
    }

    [SerializeField] private EndingCameraScript _endCam;

    public bool moveflg = false;
    public Quaternion angleAxis;
    public Transform orbitOrigin;
    public GameObject logo;
    public float _moveStartWaitTime; 

    [Header("サウンド系")]
    [SerializeField] private SoundPlayer _soundPlayer;
    [SerializeField] private AudioClip _se_MissileLaunch;

    [Header("ロベリア")]
    [SerializeField] private Transform[] _target;

    [Header("ミサイルの設定"), Space(10)]
    [SerializeField] private Transform _launchPoint;
    [SerializeField] private GameObject _missilePrefab;
    [SerializeField] private int _missileDamage = 1;

    [Header("下段にそのグループ内の何発目のミサイルに追従するか指定できる")]
    [Header("上段に何回目に発射されたミサイルのグループにキルカメラが追従するか")]
    [SerializeField, Tooltip("何回目に生成されたミサイルのグループにカメラが追従するか")] private int _followKillCamMultiTargetMissilesCount;
    [SerializeField, Tooltip("キルカメラ時何番目に生成されたミサイルをカメラが追従するか")] private int _followKillCamMissileNum;

    [SerializeField, Space(10)] private MissileShotSettings _missileShotSettings;


    void Start()
    {
        StartCoroutine(nameof(MultiTargetMissileInstantiate));
    }

    void Update()
    {
        if (moveflg) {
            angleAxis = Quaternion.AngleAxis(360 / 25 * Time.deltaTime, new Vector3(0,1,0));
            Vector3 newPos = transform.position;

            newPos -= orbitOrigin.position;
            newPos = angleAxis * newPos;
            newPos += orbitOrigin.position;

            transform.position = newPos;

            transform.rotation = transform.rotation * angleAxis;
        }
    }

    private IEnumerator MultiTargetMissileInstantiate()
    {
        yield return new WaitForSeconds(1.5f);
        int i = 0;
        foreach (var hp in _missileShotSettings._halfwayPoints) {
            
            StartCoroutine(MissileInstantiate(i++));
        }

        yield return new WaitForSeconds(_moveStartWaitTime);
        moveflg = true;
        yield return new WaitForSeconds(2.5f);
        logo.SetActive(true);

    }

    private IEnumerator MissileInstantiate(int index)
    {
        yield return new WaitForSeconds(_missileShotSettings._instantiateTimes[index]);
        GameObject missile = MissileShot(_missileShotSettings._halfwayPoints[index], _missileShotSettings._impactTimes[index], index);

    }

    /// <summary>
    /// 複数呼び出すときのターゲット指定版
    /// </summary>
    /// <param name="target">ターゲットのTransform</param>
    /// <param name="halfwaypoint">ミサイルが一度通過する中間地点</param>
    /// <param name="impacttime">着弾するまでの時間</param>
    private GameObject MissileShot(Transform halfwaypoint, float impacttime, int index)
    {
        GameObject missile = Instantiate(_missilePrefab, _launchPoint.position, Quaternion.identity);
        HomingMissileScript homingMissileScript = missile.GetComponent<HomingMissileScript>();

        homingMissileScript.LaunchMissile(_target[index], halfwaypoint, impacttime, _launchPoint.position - halfwaypoint.position, _missileShotSettings._missileShotPower[index], _missileDamage);

        _soundPlayer.PlaySE(_se_MissileLaunch);

        if(index == 0) {
            _endCam.target = missile.transform;
            _endCam.rotflg = true;
        }

        return missile;
    }
}
