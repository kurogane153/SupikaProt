using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : SingletonMonoBehaviour<ScoreManager>
{
    private int score;
    public int Score
    {
        get { return score; }
        set { score = value; }
    }

    private int combo;
    public int Combo
    {
        get { return combo; }
        set { combo = value; }
    }

    private bool isKillCameraEnable;
    public bool IsKillCameraEnable
    {
        get { return isKillCameraEnable; }
        set { isKillCameraEnable = value; }
    }

    public void AddScore(int point)
    {
        score += point * (int)Mathf.Pow(combo, 2);
    }

    /// <summary>
    /// ヒエラルキー上にあるCanvasを取得しておく
    /// </summary>
    private Canvas canvas;
    public Canvas GetCanvas()
    {
        return canvas;
    }

    public Camera killCamera;


    void Start()
    {
        canvas = GameObject.FindGameObjectWithTag("DeletePointCanvas").GetComponent<Canvas>();
    }

}