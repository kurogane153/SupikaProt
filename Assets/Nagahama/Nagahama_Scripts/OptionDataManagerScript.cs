using System;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class OptionDataManagerScript : MonoBehaviour
{
    #region Singleton
    protected static OptionDataManagerScript instance;

    public static OptionDataManagerScript Instance
    {
        get
        {
            if (instance == null) {
                instance = (OptionDataManagerScript)FindObjectOfType(typeof(OptionDataManagerScript));

                if (instance == null) {
                    //Debug.LogError("OptionDataManagerScript Instance Error");
                }
            }

            return instance;
        }
    }
    #endregion

    public UnityEvent optionValueChanges;
    public OptionData optionData;

    string filePath;

    void Awake()
    {
        if (this != Instance) {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        optionValueChanges = new UnityEvent();
        filePath = Application.persistentDataPath + "/" + ".savedata.json";
        optionData = new OptionData();
        Debug.Log(filePath);
        Load();
    }

    public void Save()
    {
        optionValueChanges.Invoke();
        JsonSave();
    }

    public void Load()
    {
        if (!File.Exists(filePath)) return;
        var streamReader = new StreamReader(filePath);
        string data = streamReader.ReadToEnd();
        streamReader.Close();
        optionData = JsonUtility.FromJson<OptionData>(data);
    }

    private void JsonSave()
    {
        string json = JsonUtility.ToJson(optionData);
        StreamWriter streamWriter = new StreamWriter(filePath);
        streamWriter.Write(json);
        streamWriter.Flush();
        streamWriter.Close();
    }

}

[Serializable]
public class OptionData
{
    [SerializeField, Range(0f, 2f)] public float _bgmVolume = 1f;           // BGM音量
    [SerializeField, Range(0f, 2f)] public float _seVolume = 1f;            // SE音量
    [SerializeField, Range(0f, 2f)] public float _aimSensivity_X = 1f;      // エイム感度X軸
    [SerializeField, Range(0f, 2f)] public float _aimSensivity_Y = 1f;      // エイム感度Y軸
    [SerializeField] public bool _useAimAssistFlag = true;                  // エイムアシスト使うか
    [SerializeField, Range(0f, 2f)] public float _aimAssistIntensity = 1f;  // エイムアシスト強度
    [SerializeField] public bool _tutorialPlayedFlag = false;               // チュートリアルをプレイしたことがあるか
    [SerializeField] public int _highScore = 0;                             // スコアアタックモードのハイスコア
}
