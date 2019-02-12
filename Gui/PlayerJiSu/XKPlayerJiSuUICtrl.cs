using UnityEngine;
using System.Collections;

public class XKPlayerJiSuUICtrl : MonoBehaviour
{
	public PlayerEnum PlayerSt;
	UISprite JiSuSprite;
	static XKPlayerJiSuUICtrl _InstanceOne;
	static XKPlayerJiSuUICtrl _InstanceTwo;
	static XKPlayerJiSuUICtrl _InstanceThree;
	static XKPlayerJiSuUICtrl _InstanceFour;
	public static XKPlayerJiSuUICtrl GetInstanceJiSu(PlayerEnum indexPlayer)
	{
		XKPlayerJiSuUICtrl huoLiOpenScript = null;
		switch (indexPlayer) {
		case PlayerEnum.PlayerOne:
			huoLiOpenScript = _InstanceOne;
			break;
		case PlayerEnum.PlayerTwo:
			huoLiOpenScript = _InstanceTwo;
			break;
		case PlayerEnum.PlayerThree:
			huoLiOpenScript = _InstanceThree;
			break;
		case PlayerEnum.PlayerFour:
			huoLiOpenScript = _InstanceFour;
			break;
		}
		return huoLiOpenScript;
	}
	
	// Use this for initialization
	void Start()
	{
		JiSuSprite = GetComponent<UISprite>();
		JiSuSprite.fillDirection = UISprite.FillDirection.Vertical;
		switch (PlayerSt) {
		case PlayerEnum.PlayerOne:
			_InstanceOne = this;
			break;
			
		case PlayerEnum.PlayerTwo:
			_InstanceTwo = this;
			break;
			
		case PlayerEnum.PlayerThree:
			_InstanceThree = this;
			break;
			
		case PlayerEnum.PlayerFour:
			_InstanceFour = this;
			break;
		}
		HiddenJiSuUI(1);
	}
	
	// Update is called once per frame
	void FixedUpdate()
	{
		CheckJiSuUISprite();
	}
	
	float TimeStart;
	void CheckJiSuUISprite()
	{
		if (!XkGameCtrl.GetIsActivePlayer(PlayerSt)) {
			HiddenJiSuUI();
			return;
		}

		float keyHD = -JiSuTime;
		float fillVal = ((Time.realtimeSinceStartup - TimeStart) / keyHD) + 1f;
		fillVal = fillVal < 0f ? 0f : fillVal;
		fillVal = fillVal > 1f ? 1f : fillVal;
		JiSuSprite.fillAmount = fillVal;
		if (fillVal <= 0f) {
			HiddenJiSuUI();
		}
	}
	
	float JiSuTime;
	public void ShowJiSuUI(float timeVal = 1f)
	{
		JiSuTime = timeVal;
		TimeStart = Time.realtimeSinceStartup;
		JiSuSprite.fillAmount = 1f;
		gameObject.SetActive(true);
	}
	
	public void HiddenJiSuUI(int key = 0)
	{
		if (key == 0) {
			XKPlayerMoveCtrl playerMoveScript = XKPlayerMoveCtrl.GetXKPlayerMoveCom(PlayerSt);
			playerMoveScript.ResetPlayerJiSuState();
		}
		gameObject.SetActive(false);
	}
}