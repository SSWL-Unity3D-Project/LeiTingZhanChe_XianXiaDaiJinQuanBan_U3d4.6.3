using UnityEngine;
using System.Collections;
using System;

public class XKGameStageCtrl : MonoBehaviour
{
	public GameObject StageCtrlObj;
	public GameObject StageObj;
	public GameObject JinGongObj;
	public Texture[] StageUIArray;
	UITexture StageUI;
	int StageCount;
	static XKGameStageCtrl _Instance;
	public static XKGameStageCtrl GetInstance()
	{
		return _Instance;
	}
	// Use this for initialization
	internal void Init()
	{
		_Instance = this;
		StageUI = StageObj.GetComponent<UITexture>();
		HiddeStageObj();
	}

	[Range(0.01f, 5f)]public float TimeStage = 1f;
	public void MoveIntoStageUI()
	{
		if (StageCtrlObj.activeSelf) {
			return;
		}
		TweenPosition twPos = StageObj.GetComponent<TweenPosition>();
		if (twPos != null) {
			twPos.enabled = false;
			DestroyObject(twPos);
		}
		
		StageUI.mainTexture = StageUIArray[StageCount];
		twPos = StageObj.AddComponent<TweenPosition>();
		twPos.from = new Vector3(900f, 0f, 0f);
		twPos.to = new Vector3(0f, 0f, 0f);
		twPos.duration = TimeStage;
		transform.localPosition = twPos.from;
		StageObj.SetActive(true);
		StageCtrlObj.SetActive(true);
		twPos.PlayForward();
		StageCount++;
		if (StageCount >= StageUIArray.Length) {
			StageCount = 0;
		}

		XKGlobalData.GetInstance().PlayAudioStage1();
		StopCoroutine(ShowJinGongUI());
		StartCoroutine(ShowJinGongUI());
	}

	IEnumerator ShowJinGongUI()
	{
		yield return new WaitForSeconds(1.5f);
		JinGongObj.SetActive(true);
		XKGlobalData.GetInstance().PlayAudioStage2();
		yield return new WaitForSeconds(1f);
		JinGongObj.SetActive(false);
		MoveOutStageUI();
	}

	void MoveOutStageUI()
	{
		TweenPosition twPos = StageObj.GetComponent<TweenPosition>();
		if (twPos != null) {
			twPos.enabled = false;
			DestroyObject(twPos);
		}
		
		twPos = StageObj.AddComponent<TweenPosition>();
		twPos.from = new Vector3(0f, 0f, 0f);
		twPos.to = new Vector3(-900f, 0f, 0f);
		twPos.duration = TimeStage;
		StageObj.SetActive(true);
		twPos.PlayForward();
		EventDelegate.Add(twPos.onFinished, delegate{
			HiddeStageObj();
		});
	}

	void HiddeStageObj()
	{
		StageCtrlObj.SetActive(false);
		StageObj.SetActive(false);
		JinGongObj.SetActive(false);
	}

    bool IsRemoveSelf = false;
    internal void RemoveSelf()
    {
        if (IsRemoveSelf == false)
        {
            IsRemoveSelf = true;
            _Instance = null;
            Destroy(gameObject);
        }
    }
}