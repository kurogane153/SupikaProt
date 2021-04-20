﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManagerScript : MonoBehaviour
{
    protected static BGMManagerScript instance;

    public static BGMManagerScript Instance
    {
        get
        {
            if (instance == null) {
                instance = (BGMManagerScript)FindObjectOfType(typeof(BGMManagerScript));

                if (instance == null) {
                    Debug.LogError("BGMManagerScript Instance Error");
                }
            }

            return instance;
        }
    }

    // 音量
    public SoundVolume volume = new SoundVolume();
    private float startVolume;

    // === AudioSource ===
    // BGM
    private AudioSource BGMsource;

    // === AudioClip ===
    // BGM
    public AudioClip[] BGM;

    void Awake()
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("BGMManager");
        if (obj.Length > 1) {
            // 既に存在しているなら削除
            Destroy(gameObject);
        } else {
            // 音管理はシーン遷移では破棄させない
            DontDestroyOnLoad(gameObject);
        }

        // 全てのAudioSourceコンポーネントを追加する

        // BGM AudioSource
        BGMsource = gameObject.GetComponent<AudioSource>();
        // BGMはループを有効にする
        BGMsource.loop = true;

        // 開始時の音量を保存
        startVolume = BGMsource.volume;

    }

    private void Start()
    {
        OptionDataManagerScript.Instance.optionValueChanges.AddListener(ChangeBGMVolume);
        
        ChangeBGMVolume();
    }

    private void ChangeBGMVolume()
    {
        BGMsource.volume = startVolume * OptionDataManagerScript.Instance.optionData._bgmVolume;
        Debug.Log("BGMマネージャー処理完了");
    }

    void Update()
    {
        // ミュート設定
        BGMsource.mute = volume.Mute;

        // ボリューム設定
        //BGMsource.volume = volume.BGM;
        
    }



    // ***** BGM再生 *****
    // BGM再生
    public void PlayBGM(int index)
    {
        if (0 > index || BGM.Length <= index) {
            return;
        }
        // 同じBGMの場合は何もしない
        if (BGMsource.clip == BGM[index]) {
            return;
        }
        BGMsource.Stop();
        BGMsource.clip = BGM[index];
        BGMsource.Play();
    }

    // BGM停止
    public void StopBGM()
    {
        BGMsource.Stop();
        BGMsource.clip = null;
    }

    private void OnDestroy()
    {
        if(OptionDataManagerScript.Instance != null) {
            OptionDataManagerScript.Instance.optionValueChanges.RemoveListener(ChangeBGMVolume);
        }
        
    }

}

// 音量クラス
[System.Serializable]
public class SoundVolume
{
    public float BGM = 1.0f;
    public float Voice = 1.0f;
    public float SE = 1.0f;
    public bool Mute = false;

    public void Init()
    {
        BGM = 1.0f;
        Voice = 1.0f;
        SE = 1.0f;
        Mute = false;
    }
}
