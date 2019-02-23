//#define USE_PC_INPUT_TEST
using UnityEngine;

/// <summary>
/// 监听pc键盘鼠标按键消息组件.
/// </summary>
public class InputEventCtrl : MonoBehaviour
{
	/// <summary>
	/// 玩家方向信息.
	/// </summary>
	public static float[] PlayerFX = new float[4];
	/**
	 * PlayerFXTmp[0] == 1 -> 1P左按下.
	 * PlayerFXTmp[0] == 0 -> 1P左弹起.
	 * PlayerFXTmp[1] == 1 -> 1P右按下.
	 * PlayerFXTmp[1] == 0 -> 1P右弹起.
	 * ...
	 * ...
	 * PlayerFXTmp[6] == 1 -> 4P左按下.
	 * PlayerFXTmp[6] == 0 -> 4P左弹起.
	 * PlayerFXTmp[7] == 1 -> 4P右按下.
	 * PlayerFXTmp[7] == 0 -> 4P右弹起.
	 */
	static float[] PlayerFXTmp = new float[8];
	/// <summary>
	/// 玩家油门信息.
	/// </summary>
	public static float[] PlayerYM = new float[4];
	/**
	 * PlayerYMTmp[0] == 1 -> 1P上按下.
	 * PlayerYMTmp[0] == 0 -> 1P上弹起.
	 * PlayerYMTmp[1] == 1 -> 1P下按下.
	 * PlayerYMTmp[1] == 0 -> 1P下弹起.
	 * ...
	 * ...
	 * PlayerYMTmp[6] == 1 -> 4P上按下.
	 * PlayerYMTmp[6] == 0 -> 4P上弹起.
	 * PlayerYMTmp[7] == 1 -> 4P下按下.
	 * PlayerYMTmp[7] == 0 -> 4P下弹起.
	 */
	static float[] PlayerYMTmp = new float[8];
	/// <summary>
	/// 玩家刹车信息.
	/// </summary>
	public static float[] PlayerSC = new float[4];
	static private InputEventCtrl Instance = null;
	static public InputEventCtrl GetInstance()
	{
		if(Instance == null)
		{
            PcvrComInputEvent pcvrComInput = PcvrComInputEvent.GetInstance();
            GameObject obj = pcvrComInput.gameObject;
            Instance = obj.AddComponent<InputEventCtrl>();
			XKGlobalData.GetInstance();
			SetPanelCtrl.GetInstance();
		}
		return Instance;
    }

    void Start()
    {
        //响应pcvr的按键消息.
        PcvrComInputEvent.GetInstance().ClickPcvrBtEvent07 += ClickSetEnterBt;
        PcvrComInputEvent.GetInstance().ClickPcvrBtEvent08 += ClickSetMoveBt;

        PcvrComInputEvent.GetInstance().ClickPcvrBtEvent01 += ClickFangXiangLBtP1;
        PcvrComInputEvent.GetInstance().ClickPcvrBtEvent14 += ClickFangXiangRBtP1;
        PcvrComInputEvent.GetInstance().ClickPcvrBtEvent12 += ClickFangXiangUBtP1;
        PcvrComInputEvent.GetInstance().ClickPcvrBtEvent13 += ClickFangXiangDBtP1;
        PcvrComInputEvent.GetInstance().ClickPcvrBtEvent23 += ClickStartBtOne;
        PcvrComInputEvent.GetInstance().ClickPcvrBtEvent04 += ClickFireBtOne;
        PcvrComInputEvent.GetInstance().ClickPcvrBtEvent09 += ClickDaoDanBtOne;

        PcvrComInputEvent.GetInstance().ClickPcvrBtEvent02 += ClickFangXiangLBtP2;
        PcvrComInputEvent.GetInstance().ClickPcvrBtEvent18 += ClickFangXiangRBtP2;
        PcvrComInputEvent.GetInstance().ClickPcvrBtEvent16 += ClickFangXiangUBtP2;
        PcvrComInputEvent.GetInstance().ClickPcvrBtEvent17 += ClickFangXiangDBtP2;
        PcvrComInputEvent.GetInstance().ClickPcvrBtEvent24 += ClickStartBtTwo;
        PcvrComInputEvent.GetInstance().ClickPcvrBtEvent05 += ClickFireBtTwo;
        PcvrComInputEvent.GetInstance().ClickPcvrBtEvent10 += ClickDaoDanBtTwo;

        PcvrComInputEvent.GetInstance().ClickPcvrBtEvent03 += ClickFangXiangLBtP3;
        PcvrComInputEvent.GetInstance().ClickPcvrBtEvent20 += ClickFangXiangRBtP3;
        PcvrComInputEvent.GetInstance().ClickPcvrBtEvent15 += ClickFangXiangUBtP3;
        PcvrComInputEvent.GetInstance().ClickPcvrBtEvent19 += ClickFangXiangDBtP3;
        PcvrComInputEvent.GetInstance().ClickPcvrBtEvent25 += ClickStartBtThree;
        PcvrComInputEvent.GetInstance().ClickPcvrBtEvent06 += ClickFireBtThree;
        PcvrComInputEvent.GetInstance().ClickPcvrBtEvent11 += ClickDaoDanBtThree;
        
        PcvrComInputEvent.GetInstance().OnPlayerInsertCoinEvent += OnPlayerInsertCoin;
    }

    #region Player Coin Event
    /// <summary>
    /// 玩家投币时间响应.
    /// coin -> 玩家的总投币数.
    /// </summary>
    public void OnPlayerInsertCoin(pcvrTXManage.PlayerCoinEnum indexPlayerCoin, int coin)
    {
        int indexVal = (int)indexPlayerCoin;
        if (indexVal < 0 || indexVal > 2)
        {
            Debug.LogWarning("OnPlayerInsertCoin -> indexVal was wrong! indexVal ==== " + indexVal);
            return;
        }
        PlayerEnum indexPlayer = (PlayerEnum)(indexVal + 1);
        XKGlobalData.SetCoinPlayerInfo(indexPlayer, coin);
    }
    #endregion

    /// <summary>
    /// 按键响应事件.
    /// </summary>
    public delegate void EventHandel(pcvr.ButtonState val);

    #region Click Button Envent
    public void OnClickGameStartBt(int indexPlayer)
    {
        switch (indexPlayer)
        {
            case 0:
                {
                    ClickStartBtOne(pcvr.ButtonState.DOWN);
                    ClickStartBtOne(pcvr.ButtonState.UP);
                    break;
                }
            case 1:
                {
                    ClickStartBtTwo(pcvr.ButtonState.DOWN);
                    ClickStartBtTwo(pcvr.ButtonState.UP);
                    break;
                }
            case 2:
                {
                    ClickStartBtThree(pcvr.ButtonState.DOWN);
                    ClickStartBtThree(pcvr.ButtonState.UP);
                    break;
                }
            case 3:
                {
                    ClickStartBtFour(pcvr.ButtonState.DOWN);
                    ClickStartBtFour(pcvr.ButtonState.UP);
                    break;
                }
        }
    }

    public event EventHandel ClickStartBtOneEvent;
	public void ClickStartBtOne(pcvr.ButtonState val)
	{
		if(ClickStartBtOneEvent != null)
		{
			ClickStartBtOneEvent( val );
			//pcvr.StartLightStateP1 = LedState.Mie;
		}

		if (XKGlobalData.GameVersionPlayer != 0) {
			ClickStartBtThree(val);
		}
	}
	
	public event EventHandel ClickStartBtTwoEvent;
	public void ClickStartBtTwo(pcvr.ButtonState val)
	{
		if(ClickStartBtTwoEvent != null)
		{
			ClickStartBtTwoEvent( val );
			//pcvr.StartLightStateP2 = LedState.Mie;
		}
		
		if (XKGlobalData.GameVersionPlayer != 0) {
			ClickStartBtFour(val);
		}
	}
	
	public event EventHandel ClickStartBtThreeEvent;
	public void ClickStartBtThree(pcvr.ButtonState val)
	{
		if(ClickStartBtThreeEvent != null)
		{
			ClickStartBtThreeEvent( val );
			//pcvr.StartLightStateP2 = LedState.Mie;
		}
	}
	
	public event EventHandel ClickStartBtFourEvent;
	public void ClickStartBtFour(pcvr.ButtonState val)
	{
		if(ClickStartBtFourEvent != null)
		{
			ClickStartBtFourEvent( val );
			//pcvr.StartLightStateP2 = LedState.Mie;
		}
	}

	public event EventHandel ClickSetEnterBtEvent;
	public void ClickSetEnterBt(pcvr.ButtonState val)
	{
		if(ClickSetEnterBtEvent != null)
		{
			ClickSetEnterBtEvent( val );
		}

		if (val == pcvr.ButtonState.DOWN) {
			XKGlobalData.PlayAudioSetEnter();
		}
	}
	
	public event EventHandel ClickSetMoveBtEvent;
	public void ClickSetMoveBt(pcvr.ButtonState val)
	{
		if(ClickSetMoveBtEvent != null)
		{
			ClickSetMoveBtEvent( val );
		}

		if (val == pcvr.ButtonState.DOWN) {
			XKGlobalData.PlayAudioSetMove();
		}
	}
	
    public void OnClickFireBt(int index, pcvr.ButtonState val)
    {
        if (val == pcvr.ButtonState.DOWN)
        {
            PlayerEnum indexPlayer = (PlayerEnum)(index + 1);
            HuDunCtrl huDunCom = HuDunCtrl.GetInstance(indexPlayer);
            if (huDunCom != null && huDunCom.IsCanResetHuDunTime == true)
            {
                huDunCom.ResetHunDunTimeInfo();
            }
        }

        switch (index)
        {
            case 0:
                {
                    ClickFireBtOne(val);
                    break;
                }
            case 1:
                {
                    ClickFireBtTwo(val);
                    break;
                }
            case 2:
                {
                    ClickFireBtThree(val);
                    break;
                }
            case 3:
                {
                    ClickFireBtFour(val);
                    break;
                }
        }
    }

	public event EventHandel ClickFireBtOneEvent;
	public void ClickFireBtOne(pcvr.ButtonState val)
	{
		if (XKGlobalData.GameVersionPlayer == 0) {
			if(ClickFireBtOneEvent != null)
			{
				ClickFireBtOneEvent( val );
			}
            
            if (XkGameCtrl.GetIsActivePlayer(PlayerEnum.PlayerOne) == false
                || SSUIRoot.GetInstance().GetIsActiveCaiPiaoBuZuPanel(PlayerEnum.PlayerOne) == true)
            {
                ClickStartBtOne(val);
            }
        }
		else {
			ClickFireBtThree( val );
		}
	}

	public event EventHandel ClickFireBtTwoEvent;
	public void ClickFireBtTwo(pcvr.ButtonState val)
	{
		if (XKGlobalData.GameVersionPlayer == 0) {
			if(ClickFireBtTwoEvent != null)
			{
				ClickFireBtTwoEvent( val );
			}

            if (XkGameCtrl.GetIsActivePlayer(PlayerEnum.PlayerTwo) == false
                || SSUIRoot.GetInstance().GetIsActiveCaiPiaoBuZuPanel(PlayerEnum.PlayerTwo) == true)
            {
                ClickStartBtTwo(val);
            }
        }
		else {
			ClickFireBtFour( val );
		}
	}

	public event EventHandel ClickFireBtThreeEvent;
	public void ClickFireBtThree(pcvr.ButtonState val)
	{
		if(ClickFireBtThreeEvent != null)
		{
			ClickFireBtThreeEvent( val );

            if (XkGameCtrl.GetIsActivePlayer(PlayerEnum.PlayerThree) == false
                || SSUIRoot.GetInstance().GetIsActiveCaiPiaoBuZuPanel(PlayerEnum.PlayerThree) == true)
            {
                ClickStartBtThree(val);
            }
        }
	}

	public event EventHandel ClickFireBtFourEvent;
	public void ClickFireBtFour(pcvr.ButtonState val)
	{
		if(ClickFireBtFourEvent != null)
		{
			ClickFireBtFourEvent( val );

            if (XkGameCtrl.GetIsActivePlayer(PlayerEnum.PlayerFour) == false
                || SSUIRoot.GetInstance().GetIsActiveCaiPiaoBuZuPanel(PlayerEnum.PlayerFour) == true)
            {
                ClickStartBtFour(val);
            }
        }
	}
    
    public void OnClickDaoDanBt(int index, pcvr.ButtonState val)
    {
        if (val == pcvr.ButtonState.DOWN)
        {
            PlayerEnum indexPlayer = (PlayerEnum)(index + 1);
            HuDunCtrl huDunCom = HuDunCtrl.GetInstance(indexPlayer);
            if (huDunCom != null && huDunCom.IsCanResetHuDunTime == true)
            {
                huDunCom.ResetHunDunTimeInfo();
            }
        }

        switch (index)
        {
            case 0:
                {
                    ClickDaoDanBtOne(val);
                    break;
                }
            case 1:
                {
                    ClickDaoDanBtTwo(val);
                    break;
                }
            case 2:
                {
                    ClickDaoDanBtThree(val);
                    break;
                }
            case 3:
                {
                    ClickDaoDanBtFour(val);
                    break;
                }
        }
    }

    public event EventHandel ClickDaoDanBtOneEvent;
	public void ClickDaoDanBtOne(pcvr.ButtonState val)
	{
		if (XKGlobalData.GameVersionPlayer == 0) {
			if(ClickDaoDanBtOneEvent != null)
			{
				ClickDaoDanBtOneEvent( val );
			}
		}
		else {
			ClickDaoDanBtThree(val);
		}
	}
	
	public event EventHandel ClickDaoDanBtTwoEvent;
	public void ClickDaoDanBtTwo(pcvr.ButtonState val)
	{
		if (XKGlobalData.GameVersionPlayer == 0) {
			if(ClickDaoDanBtTwoEvent != null)
			{
				ClickDaoDanBtTwoEvent( val );
			}
		}
		else {
			ClickDaoDanBtFour(val);
		}
	}
	
	public event EventHandel ClickDaoDanBtThreeEvent;
	public void ClickDaoDanBtThree(pcvr.ButtonState val)
	{
		if(ClickDaoDanBtThreeEvent != null)
		{
			ClickDaoDanBtThreeEvent( val );
		}
	}
	
	public event EventHandel ClickDaoDanBtFourEvent;
	public void ClickDaoDanBtFour(pcvr.ButtonState val)
	{
		if(ClickDaoDanBtFourEvent != null)
		{
			ClickDaoDanBtFourEvent( val );
		}
	}

    /// <summary>
    /// 清除所有玩家的方向信息.
    /// </summary>
    public void ClearAllPlayerDirBtInfo()
    {
        for (int i = 0; i < 4; i++)
        {
            OnClickFangXiangUBt(i, pcvr.ButtonState.UP);
            OnClickFangXiangDBt(i, pcvr.ButtonState.UP);
            OnClickFangXiangLBt(i, pcvr.ButtonState.UP);
            OnClickFangXiangRBt(i, pcvr.ButtonState.UP);
        }
    }
    
    /// <summary>
    /// 向左运动.
    /// </summary>
    public void OnClickFangXiangLBt(int index, pcvr.ButtonState val)
    {
        if (val == pcvr.ButtonState.DOWN)
        {
            PlayerEnum indexPlayer = (PlayerEnum)(index + 1);
            HuDunCtrl huDunCom = HuDunCtrl.GetInstance(indexPlayer);
            if (huDunCom != null && huDunCom.IsCanResetHuDunTime == true)
            {
                huDunCom.ResetHunDunTimeInfo();
            }
        }

        switch (index)
        {
            case 0:
                {
                    ClickFangXiangLBtP1(val);
                    break;
                }
            case 1:
                {
                    ClickFangXiangLBtP2(val);
                    break;
                }
            case 2:
                {
                    ClickFangXiangLBtP3(val);
                    break;
                }
            case 3:
                {
                    ClickFangXiangLBtP4(val);
                    break;
                }
        }
    }

    /// <summary>
    /// 向右运动.
    /// </summary>
    public void OnClickFangXiangRBt(int index, pcvr.ButtonState val)
    {
        if (val == pcvr.ButtonState.DOWN)
        {
            PlayerEnum indexPlayer = (PlayerEnum)(index + 1);
            HuDunCtrl huDunCom = HuDunCtrl.GetInstance(indexPlayer);
            if (huDunCom != null && huDunCom.IsCanResetHuDunTime == true)
            {
                huDunCom.ResetHunDunTimeInfo();
            }
        }

        switch (index)
        {
            case 0:
                {
                    ClickFangXiangRBtP1(val);
                    break;
                }
            case 1:
                {
                    ClickFangXiangRBtP2(val);
                    break;
                }
            case 2:
                {
                    ClickFangXiangRBtP3(val);
                    break;
                }
            case 3:
                {
                    ClickFangXiangRBtP4(val);
                    break;
                }
        }
    }

    /// <summary>
    /// 向上运动.
    /// </summary>
    public void OnClickFangXiangUBt(int index, pcvr.ButtonState val)
    {
        if (val == pcvr.ButtonState.DOWN)
        {
            PlayerEnum indexPlayer = (PlayerEnum)(index + 1);
            HuDunCtrl huDunCom = HuDunCtrl.GetInstance(indexPlayer);
            if (huDunCom != null && huDunCom.IsCanResetHuDunTime == true)
            {
                huDunCom.ResetHunDunTimeInfo();
            }
        }

        switch (index)
        {
            case 0:
                {
                    ClickFangXiangUBtP1(val);
                    break;
                }
            case 1:
                {
                    ClickFangXiangUBtP2(val);
                    break;
                }
            case 2:
                {
                    ClickFangXiangUBtP3(val);
                    break;
                }
            case 3:
                {
                    ClickFangXiangUBtP4(val);
                    break;
                }
        }
    }

    /// <summary>
    /// 向下运动.
    /// </summary>
    public void OnClickFangXiangDBt(int index, pcvr.ButtonState val)
    {
        switch (index)
        {
            case 0:
                {
                    ClickFangXiangDBtP1(val);
                    break;
                }
            case 1:
                {
                    ClickFangXiangDBtP2(val);
                    break;
                }
            case 2:
                {
                    ClickFangXiangDBtP3(val);
                    break;
                }
            case 3:
                {
                    ClickFangXiangDBtP4(val);
                    break;
                }
        }
    }

    /**
	 * 方向左响应P1.
	 */
    public void ClickFangXiangLBtP1(pcvr.ButtonState val)
	{
		switch (val) {
		case pcvr.ButtonState.DOWN:
			PlayerFXTmp[0] = 1f;
			PlayerFX[0] = -1f;
			break;
		case pcvr.ButtonState.UP:
			PlayerFXTmp[0] = 0f;
			PlayerFX[0] = PlayerFXTmp[1] == 0f ? 0f : 1f;
			break;
		}
	}
	/**
	 * 方向右响应P1.
	 */
	public void ClickFangXiangRBtP1(pcvr.ButtonState val)
	{
		switch (val) {
		case pcvr.ButtonState.DOWN:
			PlayerFXTmp[1] = 1f;
			PlayerFX[0] = 1f;
			break;
		case pcvr.ButtonState.UP:
			PlayerFXTmp[1] = 0f;
			PlayerFX[0] = PlayerFXTmp[0] == 0f ? 0f : -1f;
			break;
		}
	}
	/**
	 * 方向上响应P1.
	 */
	public void ClickFangXiangUBtP1(pcvr.ButtonState val)
	{
		switch (val) {
		case pcvr.ButtonState.DOWN:
			PlayerYMTmp[0] = 1f;
			PlayerYM[0] = 1f;
			break;
		case pcvr.ButtonState.UP:
			PlayerYMTmp[0] = 0f;
			PlayerYM[0] = PlayerYMTmp[1] == 0f ? 0f : -1f;
			break;
		}
	}
	/**
	 * 方向下响应P1.
	 */
	public void ClickFangXiangDBtP1(pcvr.ButtonState val)
	{
		switch (val) {
		case pcvr.ButtonState.DOWN:
			PlayerYMTmp[1] = 1f;
			PlayerYM[0] = -1f;
			break;
		case pcvr.ButtonState.UP:
			PlayerYMTmp[1] = 0f;
			PlayerYM[0] = PlayerYMTmp[0] == 0f ? 0f : 1f;
			break;
		}
	}
	/**
	 * 方向左响应P2.
	 */
	public void ClickFangXiangLBtP2(pcvr.ButtonState val)
	{
		switch (val) {
		case pcvr.ButtonState.DOWN:
			PlayerFXTmp[2] = 1f;
			PlayerFX[1] = -1f;
			break;
		case pcvr.ButtonState.UP:
			PlayerFXTmp[2] = 0f;
			PlayerFX[1] = PlayerFXTmp[3] == 0f ? 0f : 1f;
			break;
		}
	}
	/**
	 * 方向右响应P2.
	 */
	public void ClickFangXiangRBtP2(pcvr.ButtonState val)
	{
		switch (val) {
		case pcvr.ButtonState.DOWN:
			PlayerFXTmp[3] = 1f;
			PlayerFX[1] = 1f;
			break;
		case pcvr.ButtonState.UP:
			PlayerFXTmp[3] = 0f;
			PlayerFX[1] = PlayerFXTmp[2] == 0f ? 0f : -1f;
			break;
		}
	}
	/**
	 * 方向上响应P2.
	 */
	public void ClickFangXiangUBtP2(pcvr.ButtonState val)
	{
		switch (val) {
		case pcvr.ButtonState.DOWN:
			PlayerYMTmp[2] = 1f;
			PlayerYM[1] = 1f;
			break;
		case pcvr.ButtonState.UP:
			PlayerYMTmp[2] = 0f;
			PlayerYM[1] = PlayerYMTmp[3] == 0f ? 0f : -1f;
			break;
		}
	}
	/**
	 * 方向下响应P2.
	 */
	public void ClickFangXiangDBtP2(pcvr.ButtonState val)
	{
		switch (val) {
		case pcvr.ButtonState.DOWN:
			PlayerYMTmp[3] = 1f;
			PlayerYM[1] = -1f;
			break;
		case pcvr.ButtonState.UP:
			PlayerYMTmp[3] = 0f;
			PlayerYM[1] = PlayerYMTmp[2] == 0f ? 0f : 1f;
			break;
		}
	}
	/**
	 * 方向左响应P3.
	 */
	public void ClickFangXiangLBtP3(pcvr.ButtonState val)
	{
		switch (val) {
		case pcvr.ButtonState.DOWN:
			PlayerFXTmp[4] = 1f;
			PlayerFX[2] = -1f;
			break;
		case pcvr.ButtonState.UP:
			PlayerFXTmp[4] = 0f;
			PlayerFX[2] = PlayerFXTmp[5] == 0f ? 0f : 1f;
			break;
		}
	}
	/**
	 * 方向右响应P3.
	 */
	public void ClickFangXiangRBtP3(pcvr.ButtonState val)
	{
		switch (val) {
		case pcvr.ButtonState.DOWN:
			PlayerFXTmp[5] = 1f;
			PlayerFX[2] = 1f;
			break;
		case pcvr.ButtonState.UP:
			PlayerFXTmp[5] = 0f;
			PlayerFX[2] = PlayerFXTmp[4] == 0f ? 0f : -1f;
			break;
		}
	}
	/**
	 * 方向上响应P3.
	 */
	public void ClickFangXiangUBtP3(pcvr.ButtonState val)
	{
		switch (val) {
		case pcvr.ButtonState.DOWN:
			PlayerYMTmp[4] = 1f;
			PlayerYM[2] = 1f;
			break;
		case pcvr.ButtonState.UP:
			PlayerYMTmp[4] = 0f;
			PlayerYM[2] = PlayerYMTmp[5] == 0f ? 0f : -1f;
			break;
		}
	}
	/**
	 * 方向下响应P3.
	 */
	public void ClickFangXiangDBtP3(pcvr.ButtonState val)
	{
		switch (val) {
		case pcvr.ButtonState.DOWN:
			PlayerYMTmp[5] = 1f;
			PlayerYM[2] = -1f;
			break;
		case pcvr.ButtonState.UP:
			PlayerYMTmp[5] = 0f;
			PlayerYM[2] = PlayerYMTmp[4] == 0f ? 0f : 1f;
			break;
		}
	}
	/**
	 * 方向左响应P4.
	 */
	public void ClickFangXiangLBtP4(pcvr.ButtonState val)
	{
		switch (val) {
		case pcvr.ButtonState.DOWN:
			PlayerFXTmp[6] = 1;
			PlayerFX[3] = -1f;
			break;
		case pcvr.ButtonState.UP:
			PlayerFXTmp[6] = 0f;
			PlayerFX[3] = PlayerFXTmp[7] == 0f ? 0f : 1f;
			break;
		}
	}
	/**
	 * 方向右响应P4.
	 */
	public void ClickFangXiangRBtP4(pcvr.ButtonState val)
	{
		switch (val) {
		case pcvr.ButtonState.DOWN:
			PlayerFXTmp[7] = 1f;
			PlayerFX[3] = 1f;
			break;
		case pcvr.ButtonState.UP:
			PlayerFXTmp[7] = 0f;
			PlayerFX[3] = PlayerFXTmp[6] == 0f ? 0f : -1f;
			break;
		}
	}
	/**
	 * 方向上响应P4.
	 */
	public void ClickFangXiangUBtP4(pcvr.ButtonState val)
	{
		switch (val) {
		case pcvr.ButtonState.DOWN:
			PlayerYMTmp[6] = 1f;
			PlayerYM[3] = 1f;
			break;
		case pcvr.ButtonState.UP:
			PlayerYMTmp[6] = 0f;
			PlayerYM[3] = PlayerYMTmp[7] == 0f ? 0f : -1f;
			break;
		}
	}
	/**
	 * 方向下响应P4.
	 */
	public void ClickFangXiangDBtP4(pcvr.ButtonState val)
	{
		switch (val) {
		case pcvr.ButtonState.DOWN:
			PlayerYMTmp[7] = 1f;
			PlayerYM[3] = -1f;
			break;
		case pcvr.ButtonState.UP:
			PlayerYMTmp[7] = 0f;
			PlayerYM[3] = PlayerYMTmp[6] == 0f ? 0f : 1f;
			break;
		}
	}
    #endregion

    /// <summary>
    /// 用PC输入测试硬件IO.
    /// </summary>
    public static bool IsUsePcInputTest = false;
    void Update()
	{
#if !USE_PC_INPUT_TEST
        if (pcvr.bIsHardWare)
        {
			return;
		}
#else
        IsUsePcInputTest = true;
#endif

#if UNITY_EDITOR
        //发布出来的游戏不允许用键盘进行投币.
        if (Input.GetKeyUp(KeyCode.T))
        {
            if (pcvr.IsHongDDShouBing)
            {
                //红点点微信二维码游戏.
                if (XkGameCtrl.GetIsActivePlayer(PlayerEnum.PlayerOne) == false)
                {
                    //该机位没有被激活.
                    if (pcvr.GetInstance().m_HongDDGamePadInterface != null)
                    {
                        //清理玩家微信数据.
                        pcvr.GetInstance().m_HongDDGamePadInterface.RemoveGamePlayerData(PlayerEnum.PlayerOne);
                    }
                }
            }

            int coinVal = XKGlobalData.CoinPlayerOne + 1;
			XKGlobalData.SetCoinPlayerOne(coinVal);
            if (XKGlobalData.GetInstance().m_GameWXPayDataManage != null)
            {
                XKGlobalData.GetInstance().m_GameWXPayDataManage.WriteGamePayRevenueInfo(1);
            }
        }

		if (Input.GetKeyUp(KeyCode.Y))
        {
            if (pcvr.IsHongDDShouBing)
            {
                //红点点微信二维码游戏.
                if (XkGameCtrl.GetIsActivePlayer(PlayerEnum.PlayerTwo) == false)
                {
                    //该机位没有被激活.
                    if (pcvr.GetInstance().m_HongDDGamePadInterface != null)
                    {
                        //清理玩家微信数据.
                        pcvr.GetInstance().m_HongDDGamePadInterface.RemoveGamePlayerData(PlayerEnum.PlayerTwo);
                    }
                }
            }

            int coinVal = XKGlobalData.CoinPlayerTwo + 1;
			XKGlobalData.SetCoinPlayerTwo(coinVal);
            if (XKGlobalData.GetInstance().m_GameWXPayDataManage != null)
            {
                XKGlobalData.GetInstance().m_GameWXPayDataManage.WriteGamePayRevenueInfo(1);
            }
        }
		
		if (Input.GetKeyUp(KeyCode.U))
        {
            if (pcvr.IsHongDDShouBing)
            {
                //红点点微信二维码游戏.
                if (XkGameCtrl.GetIsActivePlayer(PlayerEnum.PlayerThree) == false)
                {
                    //该机位没有被激活.
                    if (pcvr.GetInstance().m_HongDDGamePadInterface != null)
                    {
                        //清理玩家微信数据.
                        pcvr.GetInstance().m_HongDDGamePadInterface.RemoveGamePlayerData(PlayerEnum.PlayerThree);
                    }
                }
            }

            if (XKGlobalData.GameVersionPlayer == 0) {
				int coinVal = XKGlobalData.CoinPlayerThree + 1;
				XKGlobalData.SetCoinPlayerThree(coinVal);
                if (XKGlobalData.GetInstance().m_GameWXPayDataManage != null)
                {
                    XKGlobalData.GetInstance().m_GameWXPayDataManage.WriteGamePayRevenueInfo(1);
                }
            }
		}
#endif

        //if (Input.GetKeyUp(KeyCode.I)) {
        //	if (XKGlobalData.GameVersionPlayer == 0) {
        //		int coinVal = XKGlobalData.CoinPlayerFour + 1;
        //		XKGlobalData.SetCoinPlayerFour(coinVal);
        //	}
        //}

        //StartBt PlayerOne
        if (Input.GetKeyUp(KeyCode.G)) {
			ClickStartBtOne( pcvr.ButtonState.UP );
		}
		
		if (Input.GetKeyDown(KeyCode.G)) {
			ClickStartBtOne( pcvr.ButtonState.DOWN );
		}
		
		//StartBt PlayerTwo
		if (Input.GetKeyUp(KeyCode.H)) {
			ClickStartBtTwo( pcvr.ButtonState.UP );
		}
		
		if (Input.GetKeyDown(KeyCode.H)) {
			ClickStartBtTwo( pcvr.ButtonState.DOWN );
		}
		
		//StartBt PlayerThree
		if (Input.GetKeyUp(KeyCode.J)) {
			ClickStartBtThree( pcvr.ButtonState.UP );
		}
		
		if (Input.GetKeyDown(KeyCode.J)) {
			ClickStartBtThree( pcvr.ButtonState.DOWN );
		}
		
		//StartBt PlayerFour
		//if (Input.GetKeyUp(KeyCode.K)) {
		//	ClickStartBtFour( pcvr.ButtonState.UP );
		//}
		
		//if (Input.GetKeyDown(KeyCode.K)) {
		//	ClickStartBtFour( pcvr.ButtonState.DOWN );
		//}

        if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer)
        {
            //没有玩家激活游戏.
        }
        else
        {
            //player_1.
            if (Input.GetKeyDown(KeyCode.A))
            {
                //ClickFangXiangLBtP1(pcvr.ButtonState.DOWN);
                OnClickFangXiangLBt(0, pcvr.ButtonState.DOWN);
                if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null
                    && XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_TestBaoJiangData != null
                    && XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_TestBaoJiangData.IsOpenTest == true
                    && XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_TestBaoJiangData.IsTestPlayerAmmo == true)
                {
                    //测试玩家子弹,使玩家运动到一起.
                    OnClickFangXiangLBt(1, pcvr.ButtonState.DOWN);
                    OnClickFangXiangLBt(2, pcvr.ButtonState.DOWN);
                }
            }

            if (Input.GetKeyUp(KeyCode.A))
            {
                //ClickFangXiangLBtP1(pcvr.ButtonState.UP);
                OnClickFangXiangLBt(0, pcvr.ButtonState.UP);
                if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null
                    && XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_TestBaoJiangData != null
                    && XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_TestBaoJiangData.IsOpenTest == true
                    && XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_TestBaoJiangData.IsTestPlayerAmmo == true)
                {
                    //测试玩家子弹,使玩家运动到一起.
                    OnClickFangXiangLBt(1, pcvr.ButtonState.UP);
                    OnClickFangXiangLBt(2, pcvr.ButtonState.UP);
                }
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                //ClickFangXiangRBtP1(pcvr.ButtonState.DOWN);
                OnClickFangXiangRBt(0, pcvr.ButtonState.DOWN);
                if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null
                    && XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_TestBaoJiangData != null
                    && XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_TestBaoJiangData.IsOpenTest == true
                    && XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_TestBaoJiangData.IsTestPlayerAmmo == true)
                {
                    //测试玩家子弹,使玩家运动到一起.
                    OnClickFangXiangRBt(1, pcvr.ButtonState.DOWN);
                    OnClickFangXiangRBt(2, pcvr.ButtonState.DOWN);
                }
            }

            if (Input.GetKeyUp(KeyCode.D))
            {
                //ClickFangXiangRBtP1(pcvr.ButtonState.UP);
                OnClickFangXiangRBt(0, pcvr.ButtonState.UP);
                if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null
                    && XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_TestBaoJiangData != null
                    && XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_TestBaoJiangData.IsOpenTest == true
                    && XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_TestBaoJiangData.IsTestPlayerAmmo == true)
                {
                    //测试玩家子弹,使玩家运动到一起.
                    OnClickFangXiangRBt(1, pcvr.ButtonState.UP);
                    OnClickFangXiangRBt(2, pcvr.ButtonState.UP);
                }
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                //ClickFangXiangUBtP1(pcvr.ButtonState.DOWN);
                OnClickFangXiangUBt(0, pcvr.ButtonState.DOWN);
                if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null
                    && XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_TestBaoJiangData != null
                    && XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_TestBaoJiangData.IsOpenTest == true
                    && XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_TestBaoJiangData.IsTestPlayerAmmo == true)
                {
                    //测试玩家子弹,使玩家运动到一起.
                    OnClickFangXiangUBt(1, pcvr.ButtonState.DOWN);
                    OnClickFangXiangUBt(2, pcvr.ButtonState.DOWN);
                }
            }

            if (Input.GetKeyUp(KeyCode.W))
            {
                //ClickFangXiangUBtP1(pcvr.ButtonState.UP);
                OnClickFangXiangUBt(0, pcvr.ButtonState.UP);
                if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null
                    && XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_TestBaoJiangData != null
                    && XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_TestBaoJiangData.IsOpenTest == true
                    && XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_TestBaoJiangData.IsTestPlayerAmmo == true)
                {
                    //测试玩家子弹,使玩家运动到一起.
                    OnClickFangXiangUBt(1, pcvr.ButtonState.UP);
                    OnClickFangXiangUBt(2, pcvr.ButtonState.UP);
                }
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                //ClickFangXiangDBtP1(pcvr.ButtonState.DOWN);
                OnClickFangXiangDBt(0, pcvr.ButtonState.DOWN);
                if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null
                    && XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_TestBaoJiangData != null
                    && XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_TestBaoJiangData.IsOpenTest == true
                    && XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_TestBaoJiangData.IsTestPlayerAmmo == true)
                {
                    //测试玩家子弹,使玩家运动到一起.
                    OnClickFangXiangDBt(1, pcvr.ButtonState.DOWN);
                    OnClickFangXiangDBt(2, pcvr.ButtonState.DOWN);
                }
            }

            if (Input.GetKeyUp(KeyCode.S))
            {
                //ClickFangXiangDBtP1(pcvr.ButtonState.UP);
                OnClickFangXiangDBt(0, pcvr.ButtonState.UP);
                if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null
                    && XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_TestBaoJiangData != null
                    && XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_TestBaoJiangData.IsOpenTest == true
                    && XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_TestBaoJiangData.IsTestPlayerAmmo == true)
                {
                    //测试玩家子弹,使玩家运动到一起.
                    OnClickFangXiangDBt(1, pcvr.ButtonState.UP);
                    OnClickFangXiangDBt(2, pcvr.ButtonState.UP);
                }
            }

            //player_2.
            if (Input.GetKeyDown(KeyCode.F))
            {
                //ClickFangXiangLBtP2(pcvr.ButtonState.DOWN);
                OnClickFangXiangLBt(1, pcvr.ButtonState.DOWN);
            }

            if (Input.GetKeyUp(KeyCode.F))
            {
                //ClickFangXiangLBtP2(pcvr.ButtonState.UP);
                OnClickFangXiangLBt(1, pcvr.ButtonState.UP);
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                //ClickFangXiangRBtP2(pcvr.ButtonState.DOWN);
                OnClickFangXiangRBt(1, pcvr.ButtonState.DOWN);
            }

            if (Input.GetKeyUp(KeyCode.H))
            {
                //ClickFangXiangRBtP2(pcvr.ButtonState.UP);
                OnClickFangXiangRBt(1, pcvr.ButtonState.UP);
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                //ClickFangXiangUBtP2(pcvr.ButtonState.DOWN);
                OnClickFangXiangUBt(1, pcvr.ButtonState.DOWN);
            }

            if (Input.GetKeyUp(KeyCode.T))
            {
                //ClickFangXiangUBtP2(pcvr.ButtonState.UP);
                OnClickFangXiangUBt(1, pcvr.ButtonState.UP);
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                //ClickFangXiangDBtP2(pcvr.ButtonState.DOWN);
                OnClickFangXiangDBt(1, pcvr.ButtonState.DOWN);
            }

            if (Input.GetKeyUp(KeyCode.G))
            {
                //ClickFangXiangDBtP2(pcvr.ButtonState.UP);
                OnClickFangXiangDBt(1, pcvr.ButtonState.UP);
            }

            //player_3.
            if (Input.GetKeyDown(KeyCode.J))
            {
                //ClickFangXiangLBtP3(pcvr.ButtonState.DOWN);
                OnClickFangXiangLBt(2, pcvr.ButtonState.DOWN);
            }

            if (Input.GetKeyUp(KeyCode.J))
            {
                //ClickFangXiangLBtP3(pcvr.ButtonState.UP);
                OnClickFangXiangLBt(2, pcvr.ButtonState.UP);
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                //ClickFangXiangRBtP3(pcvr.ButtonState.DOWN);
                OnClickFangXiangRBt(2, pcvr.ButtonState.DOWN);
            }

            if (Input.GetKeyUp(KeyCode.L))
            {
                //ClickFangXiangRBtP3(pcvr.ButtonState.UP);
                OnClickFangXiangRBt(2, pcvr.ButtonState.UP);
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                //ClickFangXiangUBtP3(pcvr.ButtonState.DOWN);
                OnClickFangXiangUBt(2, pcvr.ButtonState.DOWN);
            }

            if (Input.GetKeyUp(KeyCode.I))
            {
                //ClickFangXiangUBtP3(pcvr.ButtonState.UP);
                OnClickFangXiangUBt(2, pcvr.ButtonState.UP);
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                //ClickFangXiangDBtP3(pcvr.ButtonState.DOWN);
                OnClickFangXiangDBt(2, pcvr.ButtonState.DOWN);
            }

            if (Input.GetKeyUp(KeyCode.K))
            {
                //ClickFangXiangDBtP3(pcvr.ButtonState.UP);
                OnClickFangXiangDBt(2, pcvr.ButtonState.UP);
            }

            //player_4.
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                //ClickFangXiangLBtP4(pcvr.ButtonState.DOWN);
                OnClickFangXiangLBt(3, pcvr.ButtonState.DOWN);
            }

            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                //ClickFangXiangLBtP4(pcvr.ButtonState.UP);
                OnClickFangXiangLBt(3, pcvr.ButtonState.UP);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                //ClickFangXiangRBtP4(pcvr.ButtonState.DOWN);
                OnClickFangXiangRBt(3, pcvr.ButtonState.DOWN);
            }

            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                //ClickFangXiangRBtP4(pcvr.ButtonState.UP);
                OnClickFangXiangRBt(3, pcvr.ButtonState.UP);
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                //ClickFangXiangUBtP4(pcvr.ButtonState.DOWN);
                OnClickFangXiangUBt(3, pcvr.ButtonState.DOWN);
            }

            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                //ClickFangXiangUBtP4(pcvr.ButtonState.UP);
                OnClickFangXiangUBt(3, pcvr.ButtonState.UP);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                //ClickFangXiangDBtP4(pcvr.ButtonState.DOWN);
                OnClickFangXiangDBt(3, pcvr.ButtonState.DOWN);
            }

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                //ClickFangXiangDBtP4(pcvr.ButtonState.UP);
                OnClickFangXiangDBt(3, pcvr.ButtonState.UP);
            }
        }

		//setPanel enter button
		if (Input.GetKeyUp(KeyCode.F4)) {
			ClickSetEnterBt( pcvr.ButtonState.UP );
		}
		
		if (Input.GetKeyDown(KeyCode.F4)) {
			ClickSetEnterBt( pcvr.ButtonState.DOWN );
		}
		
		//setPanel move button
		if (Input.GetKeyUp(KeyCode.F5)) {
			ClickSetMoveBt( pcvr.ButtonState.UP );
		}
		
		if (Input.GetKeyDown(KeyCode.F5)) {
			ClickSetMoveBt( pcvr.ButtonState.DOWN );
		}

		//Fire button
		if (Input.GetKeyUp(KeyCode.Mouse0)) {
            //OnClickFireBt(0, pcvr.ButtonState.UP);
            //OnClickFireBt(1, pcvr.ButtonState.UP);
            //OnClickFireBt(2, pcvr.ButtonState.UP);
            //OnClickFireBt(3, pcvr.ButtonState.UP);
            OnClickDaoDanBt(0, pcvr.ButtonState.UP);
            OnClickDaoDanBt(1, pcvr.ButtonState.UP);
            OnClickDaoDanBt(2, pcvr.ButtonState.UP);
            OnClickDaoDanBt(3, pcvr.ButtonState.UP);
        }
		
		if (Input.GetKeyDown(KeyCode.Mouse0)) {
            //OnClickFireBt(0, pcvr.ButtonState.DOWN);
            //OnClickFireBt(1, pcvr.ButtonState.DOWN);
            //OnClickFireBt(2, pcvr.ButtonState.DOWN);
            //OnClickFireBt(3, pcvr.ButtonState.DOWN);
            OnClickDaoDanBt(0, pcvr.ButtonState.DOWN);
            OnClickDaoDanBt(1, pcvr.ButtonState.DOWN);
            OnClickDaoDanBt(2, pcvr.ButtonState.DOWN);
            OnClickDaoDanBt(3, pcvr.ButtonState.DOWN);
        }

		if (Input.GetKeyUp(KeyCode.Mouse1)) {
            OnClickDaoDanBt(0, pcvr.ButtonState.UP);
            OnClickDaoDanBt(1, pcvr.ButtonState.UP);
            OnClickDaoDanBt(2, pcvr.ButtonState.UP);
            OnClickDaoDanBt(3, pcvr.ButtonState.UP);
        }
		
		if (Input.GetKeyDown(KeyCode.Mouse1)) {
            OnClickDaoDanBt(0, pcvr.ButtonState.DOWN);
            OnClickDaoDanBt(1, pcvr.ButtonState.DOWN);
            OnClickDaoDanBt(2, pcvr.ButtonState.DOWN);
            OnClickDaoDanBt(3, pcvr.ButtonState.DOWN);
        }

        //test
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    PcvrComInputEvent.GetInstance().ClickPcvrBt07(pcvr.ButtonState.DOWN);
        //}

        //if (Input.GetKeyUp(KeyCode.P))
        //{
        //    PcvrComInputEvent.GetInstance().ClickPcvrBt07(pcvr.ButtonState.UP);
        //}
        //test
    }
}