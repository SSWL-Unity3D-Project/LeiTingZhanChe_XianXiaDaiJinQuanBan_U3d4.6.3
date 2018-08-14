using UnityEngine;
using System.Collections;

public enum TiaoGuoBtState
{
	Cartoon,
	JiFenPanel
}

public class TiaoGuoBtCtrl : MonoBehaviour {
	public TiaoGuoBtState BtState = TiaoGuoBtState.JiFenPanel;
	GameObject TiaoGuoBtObj;
	static TiaoGuoBtCtrl InstanceCartoon;
	public static TiaoGuoBtCtrl GetInstanceCartoon()
	{
		return InstanceCartoon;
	}
	
	static TiaoGuoBtCtrl InstanceJiFen;
	public static TiaoGuoBtCtrl GetInstanceJiFen()
	{
		return InstanceJiFen;
	}

	// Use this for initialization
	void Start()
	{
		TiaoGuoBtObj = gameObject;
		InputEventCtrl.GetInstance().ClickStartBtOneEvent += ClickStartBtOneEvent;
		InputEventCtrl.GetInstance().ClickStartBtTwoEvent += ClickStartBtTwoEvent;
		if (BtState != TiaoGuoBtState.Cartoon) {
			InstanceJiFen = this;
		}
		else {
			InstanceCartoon = this;
			ClickStartBtOneEvent(pcvr.ButtonState.UP);
		}
		TiaoGuoBtObj.SetActive(false);
	}

	public void ShowTiaoGuoBt()
	{
		if (XkGameCtrl.GetInstance().IsCartoonShootTest) {
			return;
		}

		if (TiaoGuoBtObj.activeSelf) {
			return;
		}
		TiaoGuoBtObj.SetActive(true);
	}
	
	public void HiddenTiaoGuoBt()
	{
		if (!TiaoGuoBtObj.activeSelf) {
			return;
		}
		TiaoGuoBtObj.SetActive(false);
	}

	void ClickStartBtOneEvent(pcvr.ButtonState state)
	{
		if (!TiaoGuoBtObj.activeSelf) {
			return;
		}

		if (state == pcvr.ButtonState.DOWN) {
			return;
		}
		OnClickTiaoGuoBt();
	}
	
	void ClickStartBtTwoEvent(pcvr.ButtonState state)
	{
		if (!TiaoGuoBtObj.activeSelf) {
			return;
		}
		
		if (state == pcvr.ButtonState.DOWN) {
			return;
		}
		OnClickTiaoGuoBt();
	}

	void OnClickTiaoGuoBt()
	{
		//XKGlobalData.GetInstance().PlayStartBtAudio();
		TiaoGuoBtObj.SetActive(false);
		Invoke("DelayCloseStartCartoon", 0.2f);
	}

	void DelayCloseStartCartoon()
	{
		//XKTriggerEndCartoon.GetInstance().CloseStartCartoon();
	}
}