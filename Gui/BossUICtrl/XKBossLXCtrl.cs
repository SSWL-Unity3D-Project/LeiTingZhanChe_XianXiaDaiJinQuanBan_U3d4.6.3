using UnityEngine;
using System.Collections;

/**
 * boss来袭UI控制.
 */
public class XKBossLXCtrl : MonoBehaviour
{
	static XKBossLXCtrl _Instance;
	public static XKBossLXCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		_Instance = this;
		HiddenBossLaiXi();
	}

	float TimeLastBossLX;
	void Update()
	{
		if (Time.realtimeSinceStartup - TimeLastBossLX < 2f) {
			return;
		}
		HiddenBossLaiXi();
	}

	public void StartPlayBossLaiXi()
	{
		Debug.Log("Unity:"+"StartPlayBossLaiXi...");
		//BossZuDangCtrl.GetInstance().SetIsActiveBossZuDang(true);

		TweenAlpha twAlpha = GetComponent<TweenAlpha>();
		if (twAlpha != null) {
			DestroyObject(twAlpha);
		}

		twAlpha = gameObject.AddComponent<TweenAlpha>();
		twAlpha.from = 1f;
		twAlpha.to = 0f;
		twAlpha.duration = 1f;
		EventDelegate.Add(twAlpha.onFinished, delegate{
			OnEndToMinAlpha();
		});
		TimeLastBossLX = Time.realtimeSinceStartup;

		XKGlobalData.GetInstance().PlayAudioBossLaiXi();
		gameObject.SetActive(true);
	}

	void OnEndToMinAlpha()
	{
		TweenAlpha twAlpha = GetComponent<TweenAlpha>();
		if (twAlpha != null) {
			DestroyObject(twAlpha);
		}
		
		twAlpha = gameObject.AddComponent<TweenAlpha>();
		twAlpha.from = 1f;
		twAlpha.to = 0f;
		twAlpha.duration = 1f;
		EventDelegate.Add(twAlpha.onFinished, delegate{
			HiddenBossLaiXi();
		});
	}

	void HiddenBossLaiXi()
	{
		//XKGlobalData.GetInstance().StopAudioBossLaiXi();
		TweenAlpha twAlpha = GetComponent<TweenAlpha>();
		if (twAlpha != null) {
			DestroyObject(twAlpha);
		}
		gameObject.SetActive(false);
	}
}