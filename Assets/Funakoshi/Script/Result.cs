using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.EventSystems;

public class Result : MonoBehaviour
{
    [SerializeField] private PostProcessVolume _postProcessVolume;
    [SerializeField] GameObject _earth;
    [SerializeField] GameObject _hellearth;
    [SerializeField] private int _restartSceneIndex;
    [SerializeField] private int _titleSceneIndex;
    [SerializeField] private float _fadeTimeLoadScene;
    [SerializeField] private EventSystem _eventSystem;

    //カメラ取得
    GameObject ClearCameraObj;
    GameObject OverCameraObj;
    Camera cam;
    Camera cam2;
    public GameObject PlayerOff;

    public GameObject GameCleartext = null;
    public GameObject GameOvertext = null;
    private Text GameClear_text;
    private Text GameOver_text;
    private PostProcessProfile postProcessProfile;


    // Start is called before the first frame update
    void Start()
    {

        //ゲームクリアとゲームオーバーのカメラ
        ClearCameraObj = GameObject.Find("Main Camera");
        OverCameraObj = GameObject.Find("GameOverCamera");
        cam = ClearCameraObj.GetComponent<Camera>();
        cam2 = OverCameraObj.GetComponent<Camera>();

        GameClear_text = GameCleartext.GetComponent<Text>();
        GameOver_text = GameOvertext.GetComponent<Text>();
        GameCleartext.SetActive(false);
        GameOvertext.SetActive(false);

        ClearCameraObj.SetActive(false);
        OverCameraObj.SetActive(false);

        postProcessProfile = _postProcessVolume.sharedProfile;

        ColorGrading colorGrading = postProcessProfile.GetSetting<ColorGrading>();

        Color startColor = colorGrading.colorFilter.value;
        colorGrading.enabled.Override(true);
        colorGrading.colorFilter.overrideState = true;
        colorGrading.colorFilter.Override(startColor);

        //GameClearOverManager.isclear = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameClearOverManager.isclear == false)
        {
            _earth.SetActive(false);
            PlayerOff.SetActive(false);
            OverCameraObj.SetActive(true);
            GameOvertext.SetActive(true);
            BGMManagerScript.Instance.PlayBGM(4);
        }

        if(GameClearOverManager.isclear == true)
        {
            _hellearth.SetActive(false);
            ClearCameraObj.SetActive(true);
            GameCleartext.SetActive(true);
            BGMManagerScript.Instance.PlayBGM(3);
        }

        
    }

    public void PushRestart()
    {
        //Restartのボタンを押すと、メイン画面に戻る
        FadeManager.Instance.LoadScene(_restartSceneIndex, _fadeTimeLoadScene);
        _eventSystem.enabled = false;
    }

    public void PushBackToTitle()
    {
        //BackToTitleのボタンを押すと、タイトルに戻る
        FadeManager.Instance.LoadScene(_titleSceneIndex, _fadeTimeLoadScene);
        _eventSystem.enabled = false;
    }

    
}
