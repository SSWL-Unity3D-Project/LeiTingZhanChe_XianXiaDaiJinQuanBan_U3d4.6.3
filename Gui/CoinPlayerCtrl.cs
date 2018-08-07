#define USE_AUTO_START_GAME
using UnityEngine;

public class CoinPlayerCtrl : MonoBehaviour
{
	public PlayerEnum PlayerSt = PlayerEnum.Null;
	public UISprite CoinSpriteA; //ShiWei
	public UISprite CoinSpriteB; //GeWei
	public UISprite NeedCoinSpriteA; //ShiWei
	public UISprite NeedCoinSpriteB; //GeWei
	public GameObject InsertCoinObj;
	public GameObject StartBtObj;
	public GameObject CoinGroup;
	public GameObject FreeMode;
	public GameObject ZhunBeiZhanDou;
	static CoinPlayerCtrl _InstanceOne;
	public static CoinPlayerCtrl GetInstanceOne()
	{
		return _InstanceOne;
	}

	static CoinPlayerCtrl _InstanceTwo;
	public static CoinPlayerCtrl GetInstanceTwo()
	{
		return _InstanceTwo;
	}

	static CoinPlayerCtrl _InstanceThree;
	public static CoinPlayerCtrl GetInstanceThree()
	{
		return _InstanceThree;
	}

	static CoinPlayerCtrl _InstanceFour;
	public static CoinPlayerCtrl GetInstanceFour()
	{
		return _InstanceFour;
	}

	// Use this for initialization
	void Start()
	{
		switch (PlayerSt) {
		case PlayerEnum.PlayerOne:
			_InstanceOne = this;
			XKGlobalData.SetCoinPlayerOne(XKGlobalData.CoinPlayerOne);
			InputEventCtrl.GetInstance().ClickStartBtOneEvent += ClickStartBtOneEvent;
			break;
			
		case PlayerEnum.PlayerTwo:
			_InstanceTwo = this;
			XKGlobalData.SetCoinPlayerTwo(XKGlobalData.CoinPlayerTwo);
			InputEventCtrl.GetInstance().ClickStartBtTwoEvent += ClickStartBtTwoEvent;
			break;
			
		case PlayerEnum.PlayerThree:
			_InstanceThree = this;
			XKGlobalData.SetCoinPlayerThree(XKGlobalData.CoinPlayerThree);
			InputEventCtrl.GetInstance().ClickStartBtThreeEvent += ClickStartBtThreeEvent;
			break;
			
		case PlayerEnum.PlayerFour:
			_InstanceFour = this;
			XKGlobalData.SetCoinPlayerFour(XKGlobalData.CoinPlayerFour);
			InputEventCtrl.GetInstance().ClickStartBtFourEvent += ClickStartBtFourEvent;
			break;
		}
		SetGameNeedCoin(XKGlobalData.GameNeedCoin);
		SetActiveFreeMode(XKGlobalData.IsFreeMode);
		InsertCoinObj.SetActive(false);
		StartBtObj.SetActive(false);
		if (ZhunBeiZhanDou != null) {
			ZhunBeiZhanDou.SetActive(false);
		}

		switch(GameTypeCtrl.AppTypeStatic) {
		case AppGameType.LianJiServer:
			gameObject.SetActive(false);
			break;
		}
	}

	void Update()
	{
		if (JiFenJieMianCtrl.GetInstance() != null && JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()) {
			if (InsertCoinObj.activeSelf) {
				InsertCoinObj.SetActive(false);
			}

			if (StartBtObj.activeSelf) {
				StartBtObj.SetActive(false);
			}
			return;
		}

		CheckPlayerOneCoinCur();
		CheckPlayerTwoCoinCur();
		CheckPlayerThreeCoinCur();
		CheckPlayerFourCoinCur();
	}

	public void HiddenPlayerCoin()
	{
		gameObject.SetActive(false);
	}

	public void ShwoPlayerCoin()
	{
		gameObject.SetActive(true);
	}

	public void SetActiveFreeMode(bool isActive)
	{
		if (isActive && InsertCoinObj.activeSelf) {
			InsertCoinObj.SetActive(false);
		}
		FreeMode.SetActive(isActive);
		CoinGroup.SetActive(!isActive);
	}

	void ClickStartBtOneEvent(ButtonState state)
	{
		if (XKGlobalData.GameVersionPlayer != 0) {
			return;
		}

		if (XkGameCtrl.IsActivePlayerOne) {
			return;
		}

		if (!StartBtObj.activeSelf) {
			return;
		}

		if (GameOverCtrl.IsShowGameOver) {
			return;
		}
		XKGlobalData.GetInstance().PlayStartBtAudio();
		SubCoinPlayerOne();
		StartBtObj.SetActive(false);
		XkGameCtrl.SetActivePlayerOne(true);
		ActiveZhanDouObj();
	}

	void ClickStartBtTwoEvent(ButtonState state)
	{
		if (XKGlobalData.GameVersionPlayer != 0) {
			return;
		}

		if (XkGameCtrl.IsActivePlayerTwo) {
			return;
		}
		
		if (!StartBtObj.activeSelf) {
			return;
		}
		
		if (GameOverCtrl.IsShowGameOver) {
			return;
		}
		XKGlobalData.GetInstance().PlayStartBtAudio();
		SubCoinPlayerTwo();
		StartBtObj.SetActive(false);
		XkGameCtrl.SetActivePlayerTwo(true);
		ActiveZhanDouObj();
	}
	
	void ClickStartBtThreeEvent(ButtonState state)
	{
		if (XkGameCtrl.IsActivePlayerThree) {
			return;
		}
		
		if (!StartBtObj.activeSelf) {
			return;
		}
		
		if (GameOverCtrl.IsShowGameOver) {
			return;
		}
		XKGlobalData.GetInstance().PlayStartBtAudio();
		SubCoinPlayerThree();
		StartBtObj.SetActive(false);
		XkGameCtrl.SetActivePlayerThree(true);
		ActiveZhanDouObj();

		if (XKGlobalData.GameVersionPlayer != 0) {
			XKGlobalData.CoinPlayerOne = XKGlobalData.CoinPlayerThree;
		}
	}
	
	void ClickStartBtFourEvent(ButtonState state)
	{
		if (XkGameCtrl.IsActivePlayerFour) {
			return;
		}
		
		if (!StartBtObj.activeSelf) {
			return;
		}
		
		if (GameOverCtrl.IsShowGameOver) {
			return;
		}
		XKGlobalData.GetInstance().PlayStartBtAudio();
		SubCoinPlayerFour();
		StartBtObj.SetActive(false);
		XkGameCtrl.SetActivePlayerFour(true);
		ActiveZhanDouObj();
		
		if (XKGlobalData.GameVersionPlayer != 0) {
			XKGlobalData.CoinPlayerTwo = XKGlobalData.CoinPlayerFour;
		}
	}

	void SubCoinPlayerOne()
	{
		XKGlobalData.CoinPlayerOne -= XKGlobalData.GameNeedCoin;
		SetPlayerCoin(XKGlobalData.CoinPlayerOne);
		pcvr.GetInstance().SubPlayerCoin(PlayerEnum.PlayerOne, XKGlobalData.GameNeedCoin);
	}
	
	void SubCoinPlayerTwo()
	{
		XKGlobalData.CoinPlayerTwo -= XKGlobalData.GameNeedCoin;
		SetPlayerCoin(XKGlobalData.CoinPlayerTwo);
		pcvr.GetInstance().SubPlayerCoin(PlayerEnum.PlayerTwo, XKGlobalData.GameNeedCoin);
	}
	
	void SubCoinPlayerThree()
	{
		XKGlobalData.CoinPlayerThree -= XKGlobalData.GameNeedCoin;
		SetPlayerCoin(XKGlobalData.CoinPlayerThree);
		pcvr.GetInstance().SubPlayerCoin(PlayerEnum.PlayerThree, XKGlobalData.GameNeedCoin);
	}
	
	void SubCoinPlayerFour()
	{
		XKGlobalData.CoinPlayerFour -= XKGlobalData.GameNeedCoin;
		SetPlayerCoin(XKGlobalData.CoinPlayerFour);
		pcvr.GetInstance().SubPlayerCoin(PlayerEnum.PlayerFour, XKGlobalData.GameNeedCoin);
	}

	void CheckPlayerOneCoinCur()
	{
		if (PlayerSt != PlayerEnum.PlayerOne) {
			return;
		}

		if (XkGameCtrl.IsActivePlayerOne) {
			return;
		}

		if (!XKGlobalData.IsFreeMode) {
			if (XKGlobalData.CoinPlayerOne < XKGlobalData.GameNeedCoin && !InsertCoinObj.activeSelf) {
				InsertCoinObj.SetActive(true); //Active Insert Coin
				StartBtObj.SetActive(false);
			}
			else if (XKGlobalData.CoinPlayerOne >= XKGlobalData.GameNeedCoin && (InsertCoinObj.activeSelf || !StartBtObj.activeSelf)) {
				InsertCoinObj.SetActive(false); //Hidden Insert Coin
				StartBtObj.SetActive(true);
#if USE_AUTO_START_GAME
                //运营模式.
                //玩家币值如果大于启动币数则自动开始.
                if (XkGameCtrl.GetInstance() != null
                    && XkGameCtrl.GetInstance().GetPlayerIsPlayDaoJiShiUI(PlayerEnum.PlayerOne))
                {
                    InputEventCtrl.GetInstance().ClickStartBtOne(ButtonState.DOWN);
                    InputEventCtrl.GetInstance().ClickStartBtOne(ButtonState.UP);
                }
#endif
            }
		}
		else {
			if (!StartBtObj.activeSelf) {
				StartBtObj.SetActive(true);
			}
		}
	}

	void CheckPlayerTwoCoinCur()
	{
		if (PlayerSt != PlayerEnum.PlayerTwo) {
			return;
		}
		
		if (XkGameCtrl.IsActivePlayerTwo) {
			return;
		}
		
		if (!XKGlobalData.IsFreeMode) {
			if (XKGlobalData.CoinPlayerTwo < XKGlobalData.GameNeedCoin && !InsertCoinObj.activeSelf) {
				InsertCoinObj.SetActive(true); //Active Insert Coin
				StartBtObj.SetActive(false);
			}
			else if (XKGlobalData.CoinPlayerTwo >= XKGlobalData.GameNeedCoin && (InsertCoinObj.activeSelf || !StartBtObj.activeSelf)) {
				InsertCoinObj.SetActive(false); //Hidden Insert Coin
				StartBtObj.SetActive(true);
#if USE_AUTO_START_GAME
                //运营模式.
                //玩家币值如果大于启动币数则自动开始.
                if (XkGameCtrl.GetInstance() != null
                    && XkGameCtrl.GetInstance().GetPlayerIsPlayDaoJiShiUI(PlayerEnum.PlayerTwo))
                {
                    InputEventCtrl.GetInstance().ClickStartBtTwo(ButtonState.DOWN);
                    InputEventCtrl.GetInstance().ClickStartBtTwo(ButtonState.UP);
                }
#endif
            }
		}
		else {
			if (!StartBtObj.activeSelf) {
				StartBtObj.SetActive(true);
			}
		}
	}
	
	void CheckPlayerThreeCoinCur()
	{
		if (PlayerSt != PlayerEnum.PlayerThree) {
			return;
		}
		
		if (XkGameCtrl.IsActivePlayerThree) {
			return;
		}
		
		if (!XKGlobalData.IsFreeMode) {
			if (XKGlobalData.CoinPlayerThree < XKGlobalData.GameNeedCoin && !InsertCoinObj.activeSelf) {
				InsertCoinObj.SetActive(true); //Active Insert Coin
				StartBtObj.SetActive(false);
			}
			else if (XKGlobalData.CoinPlayerThree >= XKGlobalData.GameNeedCoin && (InsertCoinObj.activeSelf || !StartBtObj.activeSelf)) {
				InsertCoinObj.SetActive(false); //Hidden Insert Coin
				StartBtObj.SetActive(true);
#if USE_AUTO_START_GAME
                //运营模式.
                //玩家币值如果大于启动币数则自动开始.
                if (XkGameCtrl.GetInstance() != null
                    && XkGameCtrl.GetInstance().GetPlayerIsPlayDaoJiShiUI(PlayerEnum.PlayerThree))
                {
                    InputEventCtrl.GetInstance().ClickStartBtThree(ButtonState.DOWN);
                    InputEventCtrl.GetInstance().ClickStartBtThree(ButtonState.UP);
                }
#endif
            }
		}
		else {
			if (!StartBtObj.activeSelf) {
				StartBtObj.SetActive(true);
			}
		}
	}
	
	void CheckPlayerFourCoinCur()
	{
		if (PlayerSt != PlayerEnum.PlayerFour) {
			return;
		}
		
		if (XkGameCtrl.IsActivePlayerFour) {
			return;
		}
		
		if (!XKGlobalData.IsFreeMode) {
			if (XKGlobalData.CoinPlayerFour < XKGlobalData.GameNeedCoin && !InsertCoinObj.activeSelf) {
				InsertCoinObj.SetActive(true); //Active Insert Coin
				StartBtObj.SetActive(false);
			}
			else if (XKGlobalData.CoinPlayerFour >= XKGlobalData.GameNeedCoin && (InsertCoinObj.activeSelf || !StartBtObj.activeSelf)) {
				InsertCoinObj.SetActive(false); //Hidden Insert Coin
				StartBtObj.SetActive(true);
#if USE_AUTO_START_GAME
                //运营模式.
                //玩家币值如果大于启动币数则自动开始.
                if (XkGameCtrl.GetInstance() != null
                    && XkGameCtrl.GetInstance().GetPlayerIsPlayDaoJiShiUI(PlayerEnum.PlayerFour))
                {
                    InputEventCtrl.GetInstance().ClickStartBtFour(ButtonState.DOWN);
                    InputEventCtrl.GetInstance().ClickStartBtFour(ButtonState.UP);
                }
#endif
            }
		}
		else {
			if (!StartBtObj.activeSelf) {
				StartBtObj.SetActive(true);
			}
		}
	}

	public void SetPlayerCoin(int coin)
	{
		XKGlobalData.GetInstance();
		if (XKGlobalData.GameVersionPlayer != 0) {
			if (PlayerSt == PlayerEnum.PlayerOne || PlayerSt == PlayerEnum.PlayerTwo) {
				return;
			}
		}
		SetPlayerCoinSprite(coin);
	}

	void SetPlayerCoinSprite(int num)
	{
		//Debug.Log("Unity:"+"SetPlayerCoinSprite -> coin "+num+", playerIndex "+PlayerSt);
		if(num > 99)
		{
			CoinSpriteA.spriteName = "p1_9";
			CoinSpriteB.spriteName = "p1_9";
		}
		else
		{
			string playerCoinStr = "p1_";
			int coinShiWei = (int)((float)num/10.0f);
			CoinSpriteA.spriteName = playerCoinStr + coinShiWei.ToString();
			CoinSpriteB.spriteName = playerCoinStr + (num%10).ToString();
		}
	}

	public void SetGameNeedCoin(int coin)
	{
		SetGameNeedCoinSprite(coin);
	}

	void SetGameNeedCoinSprite(int num)
	{
		string playerCoinStr = "p1_";
		NeedCoinSpriteA.spriteName = playerCoinStr + (num/10).ToString();
		NeedCoinSpriteB.spriteName = playerCoinStr + (num%10).ToString();
	}

	void ActiveZhanDouObj()
	{
		if (ZhunBeiZhanDou == null) {
			return;
		}

		ZhanDouCtrl ZhanDouScript = ZhunBeiZhanDou.GetComponent<ZhanDouCtrl>();
		if (ZhanDouScript == null) {
			ZhanDouScript = ZhunBeiZhanDou.AddComponent<ZhanDouCtrl>();
		}
		//ZhanDouScript.ShowZhanDouObj();
	}
}