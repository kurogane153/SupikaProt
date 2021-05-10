using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// シーン遷移時のフェードイン・アウトを制御するためのクラス .
/// </summary>
public class FadeManager : MonoBehaviour
{

	#region Singleton

	private static FadeManager instance;

	public static FadeManager Instance
	{
		get
		{
			if (instance == null) {
				instance = (FadeManager)FindObjectOfType(typeof(FadeManager));

				if (instance == null) {
					Debug.LogError(typeof(FadeManager) + "is nothing");
				}
			}

			return instance;
		}
	}

	#endregion Singleton

	/// <summary>
	/// デバッグモード
	/// </summary>
	public bool DebugMode = false;
	/// <summary>フェード中の透明度</summary>
	private float fadeAlpha = 0;
	/// <summary>フェード中かどうか</summary>
	private bool isFading = false;
	/// <summary>フェード色</summary>
	public Color fadeColor = Color.black;

	private bool fadeFlg;


	private void Awake()
	{
		if (this != Instance) {
			Destroy(gameObject);
			return;
		}

		DontDestroyOnLoad(gameObject);
	}

	public void OnGUI()
	{

		// Fade
		if (isFading) {
			//色と透明度を更新して白テクスチャを描画
			fadeColor.a = fadeAlpha;
			GUI.color = fadeColor;
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
		}

		if (DebugMode) {
			if (!isFading) {
				//Scene一覧を作成
				//(UnityEditor名前空間を使わないと自動取得できなかったので決めうちで作成)
				List<string> scenes = new List<string>();
				scenes.Add("SampleScene");
				//scenes.Add ("SomeScene1");
				//scenes.Add ("SomeScene2");


				//Sceneが一つもない
				if (scenes.Count == 0) {
					GUI.Box(new Rect(10, 10, 200, 50), "Fade Manager(Debug Mode)");
					GUI.Label(new Rect(20, 35, 180, 20), "Scene not found.");
					return;
				}


				GUI.Box(new Rect(10, 10, 300, 50 + scenes.Count * 25), "Fade Manager(Debug Mode)");
				GUI.Label(new Rect(20, 30, 280, 20), "Current Scene : " + SceneManager.GetActiveScene().name);

				int i = 0;
				foreach (string sceneName in scenes) {
					if (GUI.Button(new Rect(20, 55 + i * 25, 100, 20), "Load Level")) {
						LoadScene(0, 1.0f);
					}
					GUI.Label(new Rect(125, 55 + i * 25, 1000, 20), sceneName);
					i++;
				}
			}
		}
	}

	/// <summary>
	/// 画面遷移
	/// </summary>
	/// <param name='scene'>シーンインデックス</param>
	/// <param name='interval'>暗転にかかる時間(秒)</param>
	public void LoadScene(int scene, float interval)
	{
		if (fadeFlg) return;
		StartCoroutine(TransScene(scene, interval));
		BGMFadeout bgmFadeout = FindObjectOfType<BGMFadeout>();
		bgmFadeout.FadeoutStart(interval);
		fadeFlg = true;
	}

	/// <summary>
	/// シーン遷移用コルーチン
	/// </summary>
	/// <param name='scene'>シーンインデックス</param>
	/// <param name='interval'>暗転にかかる時間(秒)</param>
	private IEnumerator TransScene(int scene, float interval)
	{
		//だんだん暗く
		isFading = true;
		float time = 0;
		while (time <= interval) {
			fadeAlpha = Mathf.Lerp(0f, 1f, time / interval);
			time += Time.deltaTime;
			yield return 0;
		}

		//シーン切替
		SceneManager.LoadScene(scene);

		//一気に明るく
		time = 0;
		while (time <= interval) {
			fadeAlpha = Mathf.Lerp(1f, 0f, time / interval);
			time += interval;
			yield return 0;
		}

		isFading = false;
		fadeFlg = false;
	}
}
