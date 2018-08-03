using System;
using UnityEngine;

public class InputEventCtrl : MonoBehaviour
{
	public static bool IsClickFireBtOneDown;
	public static bool IsClickFireBtTwoDown;
	public static bool IsClickFireBtThreeDown;
	public static bool IsClickFireBtFourDown;
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
			GameObject obj = new GameObject("_InputEventCtrl");
			Instance = obj.AddComponent<InputEventCtrl>();
			pcvr.GetInstance();
			XKGlobalData.GetInstance();
			SetPanelCtrl.GetInstance();
		}
		return Instance;
	}

    void Start()
    {
#if UNITY_ANDROID
        Invoke("DelayClickSetMoveBt", 3f);
#endif
    }

    void DelayClickSetMoveBt()
    {
        if (!XKGameFPSCtrl.IsShowGameFPS)
        {
            ClickSetMoveBt(ButtonState.DOWN); //test
            ClickSetMoveBt(ButtonState.UP); //test
        }
    }

#region Click Button Envent
    public delegate void EventHandel(ButtonState val);
    public event EventHandel ClickTVYaoKongExitBtEvent;
    public void ClickTVYaoKongExitBt(ButtonState val)
    {
        if (ClickTVYaoKongExitBtEvent != null)
        {
            ClickTVYaoKongExitBtEvent(val);
        }
    }

    public event EventHandel ClickTVYaoKongEnterBtEvent;
    public void ClickTVYaoKongEnterBt(ButtonState val)
    {
        if (ClickTVYaoKongEnterBtEvent != null)
        {
            ClickTVYaoKongEnterBtEvent(val);
        }
    }

    public event EventHandel ClickTVYaoKongLeftBtEvent;
    public void ClickTVYaoKongLeftBt(ButtonState val)
    {
        if (ClickTVYaoKongLeftBtEvent != null)
        {
            ClickTVYaoKongLeftBtEvent(val);
        }
    }

    public event EventHandel ClickTVYaoKongRightBtEvent;
    public void ClickTVYaoKongRightBt(ButtonState val)
    {
        if (ClickTVYaoKongRightBtEvent != null)
        {
            ClickTVYaoKongRightBtEvent(val);
        }
    }

    public event EventHandel ClickTVYaoKongUpBtEvent;
    public void ClickTVYaoKongUpBt(ButtonState val)
    {
        if (ClickTVYaoKongUpBtEvent != null)
        {
            ClickTVYaoKongUpBtEvent(val);
        }
    }

    public event EventHandel ClickTVYaoKongDownBtEvent;
    public void ClickTVYaoKongDownBt(ButtonState val)
    {
        if (ClickTVYaoKongDownBtEvent != null)
        {
            ClickTVYaoKongDownBtEvent(val);
        }
    }

    public void OnClickGameStartBt(int indexPlayer)
    {
        switch (indexPlayer)
        {
            case 0:
                {
                    ClickStartBtOne(ButtonState.DOWN);
                    ClickStartBtOne(ButtonState.UP);
                    break;
                }
            case 1:
                {
                    ClickStartBtTwo(ButtonState.DOWN);
                    ClickStartBtTwo(ButtonState.UP);
                    break;
                }
            case 2:
                {
                    ClickStartBtThree(ButtonState.DOWN);
                    ClickStartBtThree(ButtonState.UP);
                    break;
                }
            case 3:
                {
                    ClickStartBtFour(ButtonState.DOWN);
                    ClickStartBtFour(ButtonState.UP);
                    break;
                }
        }
    }

    public event EventHandel ClickStartBtOneEvent;
	public void ClickStartBtOne(ButtonState val)
	{
		if(ClickStartBtOneEvent != null)
		{
			ClickStartBtOneEvent( val );
			//pcvr.StartLightStateP1 = LedState.Mie;
		}
		pcvr.SetIsPlayerActivePcvr();

		if (XKGlobalData.GameVersionPlayer != 0) {
			ClickStartBtThree(val);
		}
	}
	
	public event EventHandel ClickStartBtTwoEvent;
	public void ClickStartBtTwo(ButtonState val)
	{
		if(ClickStartBtTwoEvent != null)
		{
			ClickStartBtTwoEvent( val );
			//pcvr.StartLightStateP2 = LedState.Mie;
		}
		pcvr.SetIsPlayerActivePcvr();
		
		if (XKGlobalData.GameVersionPlayer != 0) {
			ClickStartBtFour(val);
		}
	}
	
	public event EventHandel ClickStartBtThreeEvent;
	public void ClickStartBtThree(ButtonState val)
	{
		if(ClickStartBtThreeEvent != null)
		{
			ClickStartBtThreeEvent( val );
			//pcvr.StartLightStateP2 = LedState.Mie;
		}
		pcvr.SetIsPlayerActivePcvr();
	}
	
	public event EventHandel ClickStartBtFourEvent;
	public void ClickStartBtFour(ButtonState val)
	{
		if(ClickStartBtFourEvent != null)
		{
			ClickStartBtFourEvent( val );
			//pcvr.StartLightStateP2 = LedState.Mie;
		}
		pcvr.SetIsPlayerActivePcvr();
	}

	public event EventHandel ClickSetEnterBtEvent;
	public void ClickSetEnterBt(ButtonState val)
	{
#if !UNITY_EDITOR
		SetEnterBtSt = val;
#endif
		if(ClickSetEnterBtEvent != null)
		{
			ClickSetEnterBtEvent( val );
		}

		if (val == ButtonState.DOWN) {
			XKGlobalData.PlayAudioSetEnter();
#if !UNITY_EDITOR
			TimeSetEnterMoveBt = Time.time;
#endif
		}
		pcvr.SetIsPlayerActivePcvr();
	}
	
	public event EventHandel ClickSetMoveBtEvent;
	public void ClickSetMoveBt(ButtonState val)
	{
		if(ClickSetMoveBtEvent != null)
		{
			ClickSetMoveBtEvent( val );
		}

		if (val == ButtonState.DOWN) {
			XKGlobalData.PlayAudioSetMove();
		}
		pcvr.SetIsPlayerActivePcvr();
	}
	
    public void OnClickFireBt(int index, ButtonState val)
    {
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
	public void ClickFireBtOne(ButtonState val)
	{
		if (XKGlobalData.GameVersionPlayer == 0) {
			if(ClickFireBtOneEvent != null)
			{
				ClickFireBtOneEvent( val );
			}
			pcvr.SetIsPlayerActivePcvr();
		}
		else {
			ClickFireBtThree( val );
		}
	}

	public event EventHandel ClickFireBtTwoEvent;
	public void ClickFireBtTwo(ButtonState val)
	{
		if (XKGlobalData.GameVersionPlayer == 0) {
			if(ClickFireBtTwoEvent != null)
			{
				ClickFireBtTwoEvent( val );
			}
			pcvr.SetIsPlayerActivePcvr();
		}
		else {
			ClickFireBtFour( val );
		}
	}

	public event EventHandel ClickFireBtThreeEvent;
	public void ClickFireBtThree(ButtonState val)
	{
		if(ClickFireBtThreeEvent != null)
		{
			ClickFireBtThreeEvent( val );
		}
		pcvr.SetIsPlayerActivePcvr();
	}

	public event EventHandel ClickFireBtFourEvent;
	public void ClickFireBtFour(ButtonState val)
	{
		if(ClickFireBtFourEvent != null)
		{
			ClickFireBtFourEvent( val );
		}
		pcvr.SetIsPlayerActivePcvr();
	}
    
    public void OnClickDaoDanBt(int index, ButtonState val)
    {
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
	public void ClickDaoDanBtOne(ButtonState val)
	{
		if (XKGlobalData.GameVersionPlayer == 0) {
			if(ClickDaoDanBtOneEvent != null)
			{
				ClickDaoDanBtOneEvent( val );
			}
			pcvr.SetIsPlayerActivePcvr();

			if (val == ButtonState.DOWN) {
				pcvr.OpenZuoYiQiNang(PlayerEnum.PlayerOne);
			}

			//if (SetPanelUiRoot.GetInstance() == null
			//    && !HardwareCheckCtrl.IsTestHardWare
			//    && ClickStartBtOneEvent != null) {
			//	ClickStartBtOneEvent( val );
			//}
		}
		else {
			ClickDaoDanBtThree(val);
		}
	}
	
	public event EventHandel ClickDaoDanBtTwoEvent;
	public void ClickDaoDanBtTwo(ButtonState val)
	{
		if (XKGlobalData.GameVersionPlayer == 0) {
			if(ClickDaoDanBtTwoEvent != null)
			{
				ClickDaoDanBtTwoEvent( val );
			}
			pcvr.SetIsPlayerActivePcvr();
			
			if (val == ButtonState.DOWN) {
				pcvr.OpenZuoYiQiNang(PlayerEnum.PlayerTwo);
			}
			
			//if (SetPanelUiRoot.GetInstance() == null
			//    && !HardwareCheckCtrl.IsTestHardWare
			//    && ClickStartBtTwoEvent != null) {
			//	ClickStartBtTwoEvent( val );
			//}
		}
		else {
			ClickDaoDanBtFour(val);
		}
	}
	
	public event EventHandel ClickDaoDanBtThreeEvent;
	public void ClickDaoDanBtThree(ButtonState val)
	{
		if(ClickDaoDanBtThreeEvent != null)
		{
			ClickDaoDanBtThreeEvent( val );
		}
		pcvr.SetIsPlayerActivePcvr();
		
		if (val == ButtonState.DOWN) {
			pcvr.OpenZuoYiQiNang(PlayerEnum.PlayerThree);
		}
		
		//if (SetPanelUiRoot.GetInstance() == null
		//    && !HardwareCheckCtrl.IsTestHardWare
		//    && ClickStartBtThreeEvent != null) {
		//	ClickStartBtThreeEvent( val );
		//}
	}
	
	public event EventHandel ClickDaoDanBtFourEvent;
	public void ClickDaoDanBtFour(ButtonState val)
	{
		if(ClickDaoDanBtFourEvent != null)
		{
			ClickDaoDanBtFourEvent( val );
		}
		pcvr.SetIsPlayerActivePcvr();
		
		if (val == ButtonState.DOWN) {
			pcvr.OpenZuoYiQiNang(PlayerEnum.PlayerFour);
		}
		
		//if (SetPanelUiRoot.GetInstance() == null
		//    && !HardwareCheckCtrl.IsTestHardWare
		//    && ClickStartBtFourEvent != null) {
		//	ClickStartBtFourEvent( val );
		//}
	}

	public event EventHandel ClickStopDongGanBtOneEvent;
	public void ClickStopDongGanBtOne(ButtonState val)
	{
		if(ClickStopDongGanBtOneEvent != null)
		{
			ClickStopDongGanBtOneEvent( val );
		}

		//if (val == ButtonState.DOWN) {
		//	DongGanUICtrl.ShowDongGanInfo(PlayerEnum.PlayerOne);
		//}
		pcvr.SetIsPlayerActivePcvr();
	}
	
	public event EventHandel ClickStopDongGanBtTwoEvent;
	public void ClickStopDongGanBtTwo(ButtonState val)
	{
		if(ClickStopDongGanBtTwoEvent != null)
		{
			ClickStopDongGanBtTwoEvent( val );
		}
		
		//if (val == ButtonState.DOWN) {
		//	DongGanUICtrl.ShowDongGanInfo(PlayerEnum.PlayerTwo);
		//}
		pcvr.SetIsPlayerActivePcvr();
	}
	
	public event EventHandel ClickStopDongGanBtThreeEvent;
	public void ClickStopDongGanBtThree(ButtonState val)
	{
		if(ClickStopDongGanBtThreeEvent != null)
		{
			ClickStopDongGanBtThreeEvent( val );
		}
		
		//if (val == ButtonState.DOWN) {
		//	DongGanUICtrl.ShowDongGanInfo(PlayerEnum.PlayerThree);
		//}
		pcvr.SetIsPlayerActivePcvr();
	}
	
	public event EventHandel ClickStopDongGanBtFourEvent;
	public void ClickStopDongGanBtFour(ButtonState val)
	{
		if(ClickStopDongGanBtFourEvent != null)
		{
			ClickStopDongGanBtFourEvent( val );
		}
		
		//if (val == ButtonState.DOWN) {
		//	DongGanUICtrl.ShowDongGanInfo(PlayerEnum.PlayerFour);
		//}
		pcvr.SetIsPlayerActivePcvr();
	}

    /// <summary>
    /// 向左运动.
    /// </summary>
    public void OnClickFangXiangLBt(int indexPlayer, ButtonState val)
    {
        switch (indexPlayer)
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
    public void OnClickFangXiangRBt(int indexPlayer, ButtonState val)
    {
        switch (indexPlayer)
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
    public void OnClickFangXiangUBt(int indexPlayer, ButtonState val)
    {
        switch (indexPlayer)
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
    public void OnClickFangXiangDBt(int indexPlayer, ButtonState val)
    {
        switch (indexPlayer)
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
    public void ClickFangXiangLBtP1(ButtonState val)
	{
		switch (val) {
		case ButtonState.DOWN:
			PlayerFXTmp[0] = 1f;
			PlayerFX[0] = -1f;
			break;
		case ButtonState.UP:
			PlayerFXTmp[0] = 0f;
			PlayerFX[0] = PlayerFXTmp[1] == 0f ? 0f : 1f;
			break;
		}
	}
	/**
	 * 方向右响应P1.
	 */
	public void ClickFangXiangRBtP1(ButtonState val)
	{
		switch (val) {
		case ButtonState.DOWN:
			PlayerFXTmp[1] = 1f;
			PlayerFX[0] = 1f;
			break;
		case ButtonState.UP:
			PlayerFXTmp[1] = 0f;
			PlayerFX[0] = PlayerFXTmp[0] == 0f ? 0f : -1f;
			break;
		}
	}
	/**
	 * 方向上响应P1.
	 */
	public void ClickFangXiangUBtP1(ButtonState val)
	{
		switch (val) {
		case ButtonState.DOWN:
			PlayerYMTmp[0] = 1f;
			PlayerYM[0] = 1f;
			break;
		case ButtonState.UP:
			PlayerYMTmp[0] = 0f;
			PlayerYM[0] = PlayerYMTmp[1] == 0f ? 0f : -1f;
			break;
		}
	}
	/**
	 * 方向下响应P1.
	 */
	public void ClickFangXiangDBtP1(ButtonState val)
	{
		switch (val) {
		case ButtonState.DOWN:
			PlayerYMTmp[1] = 1f;
			PlayerYM[0] = -1f;
			break;
		case ButtonState.UP:
			PlayerYMTmp[1] = 0f;
			PlayerYM[0] = PlayerYMTmp[0] == 0f ? 0f : 1f;
			break;
		}
	}
	/**
	 * 方向左响应P2.
	 */
	public void ClickFangXiangLBtP2(ButtonState val)
	{
		switch (val) {
		case ButtonState.DOWN:
			PlayerFXTmp[2] = 1f;
			PlayerFX[1] = -1f;
			break;
		case ButtonState.UP:
			PlayerFXTmp[2] = 0f;
			PlayerFX[1] = PlayerFXTmp[3] == 0f ? 0f : 1f;
			break;
		}
	}
	/**
	 * 方向右响应P2.
	 */
	public void ClickFangXiangRBtP2(ButtonState val)
	{
		switch (val) {
		case ButtonState.DOWN:
			PlayerFXTmp[3] = 1f;
			PlayerFX[1] = 1f;
			break;
		case ButtonState.UP:
			PlayerFXTmp[3] = 0f;
			PlayerFX[1] = PlayerFXTmp[2] == 0f ? 0f : -1f;
			break;
		}
	}
	/**
	 * 方向上响应P2.
	 */
	public void ClickFangXiangUBtP2(ButtonState val)
	{
		switch (val) {
		case ButtonState.DOWN:
			PlayerYMTmp[2] = 1f;
			PlayerYM[1] = 1f;
			break;
		case ButtonState.UP:
			PlayerYMTmp[2] = 0f;
			PlayerYM[1] = PlayerYMTmp[3] == 0f ? 0f : -1f;
			break;
		}
	}
	/**
	 * 方向下响应P2.
	 */
	public void ClickFangXiangDBtP2(ButtonState val)
	{
		switch (val) {
		case ButtonState.DOWN:
			PlayerYMTmp[3] = 1f;
			PlayerYM[1] = -1f;
			break;
		case ButtonState.UP:
			PlayerYMTmp[3] = 0f;
			PlayerYM[1] = PlayerYMTmp[2] == 0f ? 0f : 1f;
			break;
		}
	}
	/**
	 * 方向左响应P3.
	 */
	public void ClickFangXiangLBtP3(ButtonState val)
	{
		switch (val) {
		case ButtonState.DOWN:
			PlayerFXTmp[4] = 1f;
			PlayerFX[2] = -1f;
			break;
		case ButtonState.UP:
			PlayerFXTmp[4] = 0f;
			PlayerFX[2] = PlayerFXTmp[5] == 0f ? 0f : 1f;
			break;
		}
	}
	/**
	 * 方向右响应P3.
	 */
	public void ClickFangXiangRBtP3(ButtonState val)
	{
		switch (val) {
		case ButtonState.DOWN:
			PlayerFXTmp[5] = 1f;
			PlayerFX[2] = 1f;
			break;
		case ButtonState.UP:
			PlayerFXTmp[5] = 0f;
			PlayerFX[2] = PlayerFXTmp[4] == 0f ? 0f : -1f;
			break;
		}
	}
	/**
	 * 方向上响应P3.
	 */
	public void ClickFangXiangUBtP3(ButtonState val)
	{
		switch (val) {
		case ButtonState.DOWN:
			PlayerYMTmp[4] = 1f;
			PlayerYM[2] = 1f;
			break;
		case ButtonState.UP:
			PlayerYMTmp[4] = 0f;
			PlayerYM[2] = PlayerYMTmp[5] == 0f ? 0f : -1f;
			break;
		}
	}
	/**
	 * 方向下响应P3.
	 */
	public void ClickFangXiangDBtP3(ButtonState val)
	{
		switch (val) {
		case ButtonState.DOWN:
			PlayerYMTmp[5] = 1f;
			PlayerYM[2] = -1f;
			break;
		case ButtonState.UP:
			PlayerYMTmp[5] = 0f;
			PlayerYM[2] = PlayerYMTmp[4] == 0f ? 0f : 1f;
			break;
		}
	}
	/**
	 * 方向左响应P4.
	 */
	public void ClickFangXiangLBtP4(ButtonState val)
	{
		switch (val) {
		case ButtonState.DOWN:
			PlayerFXTmp[6] = 1;
			PlayerFX[3] = -1f;
			break;
		case ButtonState.UP:
			PlayerFXTmp[6] = 0f;
			PlayerFX[3] = PlayerFXTmp[7] == 0f ? 0f : 1f;
			break;
		}
	}
	/**
	 * 方向右响应P4.
	 */
	public void ClickFangXiangRBtP4(ButtonState val)
	{
		switch (val) {
		case ButtonState.DOWN:
			PlayerFXTmp[7] = 1f;
			PlayerFX[3] = 1f;
			break;
		case ButtonState.UP:
			PlayerFXTmp[7] = 0f;
			PlayerFX[3] = PlayerFXTmp[6] == 0f ? 0f : -1f;
			break;
		}
	}
	/**
	 * 方向上响应P4.
	 */
	public void ClickFangXiangUBtP4(ButtonState val)
	{
		switch (val) {
		case ButtonState.DOWN:
			PlayerYMTmp[6] = 1f;
			PlayerYM[3] = 1f;
			break;
		case ButtonState.UP:
			PlayerYMTmp[6] = 0f;
			PlayerYM[3] = PlayerYMTmp[7] == 0f ? 0f : -1f;
			break;
		}
	}
	/**
	 * 方向下响应P4.
	 */
	public void ClickFangXiangDBtP4(ButtonState val)
	{
		switch (val) {
		case ButtonState.DOWN:
			PlayerYMTmp[7] = 1f;
			PlayerYM[3] = -1f;
			break;
		case ButtonState.UP:
			PlayerYMTmp[7] = 0f;
			PlayerYM[3] = PlayerYMTmp[6] == 0f ? 0f : 1f;
			break;
		}
	}
    #endregion

#if !UNITY_EDITOR
	float TimeSetEnterMoveBt;
	ButtonState SetEnterBtSt = ButtonState.UP;
#endif

    class KeyCodeTV
    {
        /// <summary>
        /// 遥控器确定键的键值.
        /// </summary>
        public static KeyCode PadEnter01 = (KeyCode)10;
        public static KeyCode PadEnter02 = (KeyCode)66;
    }
    void Update()
	{
//		#if !UNITY_EDITOR
//		if (SetEnterBtSt == ButtonState.DOWN && Time.time - TimeSetEnterMoveBt > 2f) {
//			HardwareCheckCtrl.OnRestartGame();
//		}
//		#endif

		if (pcvr.bIsHardWare && !TestTanKCom.IsTestTankCom && !pcvr.IsTestInput) {
			return;
		}
        
#if UNITY_ANDROID
        if (Input.GetKeyDown(KeyCode.G) || Input.GetKeyDown(KeyCode.K))
        {
            //遥控器的确定键消息.
            ClickTVYaoKongEnterBt(ButtonState.DOWN);
            ClickSetMoveBt(ButtonState.DOWN); //test
        }

        if (Input.GetKeyUp(KeyCode.G) || Input.GetKeyUp(KeyCode.K))
        {
            //遥控器的确定键消息.
            ClickTVYaoKongEnterBt(ButtonState.UP);
            ClickSetMoveBt(ButtonState.UP); //test
        }
#endif

        //(KeyCode)10 -> acbox虚拟机的遥控器确定键消息.
        if (Input.GetKeyDown(KeyCode.KeypadEnter)
            || Input.GetKeyDown(KeyCode.Return)
            || Input.GetKeyDown(KeyCodeTV.PadEnter01)
            || Input.GetKeyDown(KeyCodeTV.PadEnter02)
            || Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            //遥控器的确定键消息.
            ClickTVYaoKongEnterBt(ButtonState.DOWN);
        }

        if (Input.GetKeyUp(KeyCode.KeypadEnter)
            || Input.GetKeyUp(KeyCode.Return)
            || Input.GetKeyUp(KeyCodeTV.PadEnter01)
            || Input.GetKeyUp(KeyCodeTV.PadEnter02)
            || Input.GetKeyUp(KeyCode.JoystickButton0))
        {
            //遥控器的确定键消息.
            ClickTVYaoKongEnterBt(ButtonState.UP);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //接收遥控器的返回键/键盘上的Esc按键信息.
            ClickTVYaoKongExitBt(ButtonState.DOWN);
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            //接收遥控器的返回键/键盘上的Esc按键信息.
            ClickTVYaoKongExitBt(ButtonState.UP);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            //接收遥控器/键盘上的向左按键信息.
            ClickTVYaoKongLeftBt(ButtonState.DOWN);
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.Keypad4))
        {
            //接收遥控器/键盘上的向左按键信息.
            ClickTVYaoKongLeftBt(ButtonState.UP);
        }
        
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Keypad6))
        {
            //接收遥控器/键盘上的向右按键信息.
            ClickTVYaoKongRightBt(ButtonState.DOWN);
        }

        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.Keypad6))
        {
            //接收遥控器/键盘上的向右按键信息.
            ClickTVYaoKongRightBt(ButtonState.UP);
        }
        
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            //接收遥控器/键盘上的向上按键信息.
            ClickTVYaoKongUpBt(ButtonState.DOWN);
        }

        if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.Keypad2))
        {
            //接收遥控器/键盘上的向上按键信息.
            ClickTVYaoKongUpBt(ButtonState.UP);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Keypad8))
        {
            //接收遥控器/键盘上的向下按键信息.
            ClickTVYaoKongDownBt(ButtonState.DOWN);
        }

        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.Keypad8))
        {
            //接收遥控器/键盘上的向下按键信息.
            ClickTVYaoKongDownBt(ButtonState.UP);
        }

        if (pcvr.IsHongDDShouBing)
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.T)) {
			int coinVal = XKGlobalData.CoinPlayerOne + 1;
			XKGlobalData.SetCoinPlayerOne(coinVal);
		}

		if (Input.GetKeyUp(KeyCode.Y)) {
			int coinVal = XKGlobalData.CoinPlayerTwo + 1;
			XKGlobalData.SetCoinPlayerTwo(coinVal);
		}
		
		if (Input.GetKeyUp(KeyCode.U)) {
			if (XKGlobalData.GameVersionPlayer == 0) {
				int coinVal = XKGlobalData.CoinPlayerThree + 1;
				XKGlobalData.SetCoinPlayerThree(coinVal);
			}
		}
		
		//if (Input.GetKeyUp(KeyCode.I)) {
		//	if (XKGlobalData.GameVersionPlayer == 0) {
		//		int coinVal = XKGlobalData.CoinPlayerFour + 1;
		//		XKGlobalData.SetCoinPlayerFour(coinVal);
		//	}
		//}

		//StartBt PlayerOne
		if (Input.GetKeyUp(KeyCode.G)) {
			ClickStartBtOne( ButtonState.UP );
		}
		
		if (Input.GetKeyDown(KeyCode.G)) {
			ClickStartBtOne( ButtonState.DOWN );
		}
		
		//StartBt PlayerTwo
		if (Input.GetKeyUp(KeyCode.H)) {
			ClickStartBtTwo( ButtonState.UP );
		}
		
		if (Input.GetKeyDown(KeyCode.H)) {
			ClickStartBtTwo( ButtonState.DOWN );
		}
		
		//StartBt PlayerThree
		if (Input.GetKeyUp(KeyCode.J)) {
			ClickStartBtThree( ButtonState.UP );
		}
		
		if (Input.GetKeyDown(KeyCode.J)) {
			ClickStartBtThree( ButtonState.DOWN );
		}
		
		//StartBt PlayerFour
		//if (Input.GetKeyUp(KeyCode.K)) {
		//	ClickStartBtFour( ButtonState.UP );
		//}
		
		//if (Input.GetKeyDown(KeyCode.K)) {
		//	ClickStartBtFour( ButtonState.DOWN );
		//}

		//player_1.
		if (Input.GetKeyDown(KeyCode.A)) {
			ClickFangXiangLBtP1(ButtonState.DOWN);
		}
		
		if (Input.GetKeyUp(KeyCode.A)) {
			ClickFangXiangLBtP1(ButtonState.UP);
		}
		
		if (Input.GetKeyDown(KeyCode.D)) {
			ClickFangXiangRBtP1(ButtonState.DOWN);
		}
		
		if (Input.GetKeyUp(KeyCode.D)) {
			ClickFangXiangRBtP1(ButtonState.UP);
		}

		if (Input.GetKeyDown(KeyCode.W)) {
			ClickFangXiangUBtP1(ButtonState.DOWN);
		}
		
		if (Input.GetKeyUp(KeyCode.W)) {
			ClickFangXiangUBtP1(ButtonState.UP);
		}
		
		if (Input.GetKeyDown(KeyCode.S)) {
			ClickFangXiangDBtP1(ButtonState.DOWN);
		}
		
		if (Input.GetKeyUp(KeyCode.S)) {
			ClickFangXiangDBtP1(ButtonState.UP);
		}

		//player_2.
		if (!TestTanKCom.IsTestTankCom) {
			if (Input.GetKeyDown(KeyCode.F)) {
				ClickFangXiangLBtP2(ButtonState.DOWN);
			}
			
			if (Input.GetKeyUp(KeyCode.F)) {
				ClickFangXiangLBtP2(ButtonState.UP);
			}
			
			if (Input.GetKeyDown(KeyCode.H)) {
				ClickFangXiangRBtP2(ButtonState.DOWN);
			}
			
			if (Input.GetKeyUp(KeyCode.H)) {
				ClickFangXiangRBtP2(ButtonState.UP);
			}
			
			if (Input.GetKeyDown(KeyCode.T)) {
				ClickFangXiangUBtP2(ButtonState.DOWN);
			}
			
			if (Input.GetKeyUp(KeyCode.T)) {
				ClickFangXiangUBtP2(ButtonState.UP);
			}
			
			if (Input.GetKeyDown(KeyCode.G)) {
				ClickFangXiangDBtP2(ButtonState.DOWN);
			}
			
			if (Input.GetKeyUp(KeyCode.G)) {
				ClickFangXiangDBtP2(ButtonState.UP);
			}
		}
		else {
			PlayerYM[1] = TestTanKCom.YouMenStateP2;
			PlayerFX[1] = TestTanKCom.FangXiangStateP2;
		}
		
		//player_3.
		if (!TestTanKCom.IsTestTankCom) {
			if (Input.GetKeyDown(KeyCode.J)) {
				ClickFangXiangLBtP3(ButtonState.DOWN);
			}
			
			if (Input.GetKeyUp(KeyCode.J)) {
				ClickFangXiangLBtP3(ButtonState.UP);
			}
			
			if (Input.GetKeyDown(KeyCode.L)) {
				ClickFangXiangRBtP3(ButtonState.DOWN);
			}
			
			if (Input.GetKeyUp(KeyCode.L)) {
				ClickFangXiangRBtP3(ButtonState.UP);
			}
			
			if (Input.GetKeyDown(KeyCode.I)) {
				ClickFangXiangUBtP3(ButtonState.DOWN);
			}
			
			if (Input.GetKeyUp(KeyCode.I)) {
				ClickFangXiangUBtP3(ButtonState.UP);
			}
			
			if (Input.GetKeyDown(KeyCode.K)) {
				ClickFangXiangDBtP3(ButtonState.DOWN);
			}
			
			if (Input.GetKeyUp(KeyCode.K)) {
				ClickFangXiangDBtP3(ButtonState.UP);
			}
		}
		else {
			PlayerFX[2] = TestTanKCom.FangXiangStateP3;
			PlayerYM[2] = TestTanKCom.YouMenStateP3;
		}
		
		//player_4.
		if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			ClickFangXiangLBtP4(ButtonState.DOWN);
		}
		
		if (Input.GetKeyUp(KeyCode.LeftArrow)) {
			ClickFangXiangLBtP4(ButtonState.UP);
		}
		
		if (Input.GetKeyDown(KeyCode.RightArrow)) {
			ClickFangXiangRBtP4(ButtonState.DOWN);
		}
		
		if (Input.GetKeyUp(KeyCode.RightArrow)) {
			ClickFangXiangRBtP4(ButtonState.UP);
		}
		
		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			ClickFangXiangUBtP4(ButtonState.DOWN);
		}
		
		if (Input.GetKeyUp(KeyCode.UpArrow)) {
			ClickFangXiangUBtP4(ButtonState.UP);
		}

		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			ClickFangXiangDBtP4(ButtonState.DOWN);
		}
		
		if (Input.GetKeyUp(KeyCode.DownArrow)) {
			ClickFangXiangDBtP4(ButtonState.UP);
		}

		//setPanel enter button
		if (Input.GetKeyUp(KeyCode.F4)) {
			ClickSetEnterBt( ButtonState.UP );
		}
		
		if (Input.GetKeyDown(KeyCode.F4)) {
			ClickSetEnterBt( ButtonState.DOWN );
		}
		
		//setPanel move button
		if (Input.GetKeyUp(KeyCode.F5)) {
			ClickSetMoveBt( ButtonState.UP );
		}
		
		if (Input.GetKeyDown(KeyCode.F5)) {
			ClickSetMoveBt( ButtonState.DOWN );
		}

		//Fire button
		if (Input.GetKeyUp(KeyCode.Mouse0)) {
			IsClickFireBtOneDown = false;
			IsClickFireBtTwoDown = false;
			IsClickFireBtThreeDown = false;
			IsClickFireBtFourDown = false;
			ClickFireBtOne( ButtonState.UP );
			if (!TestTanKCom.IsTestTankCom) {
				ClickFireBtTwo( ButtonState.UP );
				ClickFireBtThree( ButtonState.UP );
			}
			ClickFireBtFour( ButtonState.UP );
		}
		
		if (Input.GetKeyDown(KeyCode.Mouse0)) {
			IsClickFireBtOneDown = true;
			IsClickFireBtTwoDown = true;
			IsClickFireBtThreeDown = true;
			IsClickFireBtFourDown = true;
			ClickFireBtOne( ButtonState.DOWN );
			if (!TestTanKCom.IsTestTankCom) {
				ClickFireBtTwo( ButtonState.DOWN );
				ClickFireBtThree( ButtonState.DOWN );
			}
			ClickFireBtFour( ButtonState.DOWN );
		}

		if (Input.GetKeyUp(KeyCode.Mouse1)) {
			ClickDaoDanBtOne( ButtonState.UP );
			if (!TestTanKCom.IsTestTankCom) {
				ClickDaoDanBtTwo( ButtonState.UP );
				ClickDaoDanBtThree( ButtonState.UP );
			}
			ClickDaoDanBtFour( ButtonState.UP );
		}
		
		if (Input.GetKeyDown(KeyCode.Mouse1)) {
			ClickDaoDanBtOne( ButtonState.DOWN );
			if (!TestTanKCom.IsTestTankCom) {
				ClickDaoDanBtTwo( ButtonState.DOWN );
				ClickDaoDanBtThree( ButtonState.DOWN );
			}
			ClickDaoDanBtFour( ButtonState.DOWN );
		}

		if (Input.GetKeyDown(KeyCode.C)) {
			ClickStopDongGanBtOne(ButtonState.DOWN);
			ClickStopDongGanBtTwo(ButtonState.DOWN);
			ClickStopDongGanBtThree(ButtonState.DOWN);
			ClickStopDongGanBtFour(ButtonState.DOWN);
		}
		
		if (Input.GetKeyUp(KeyCode.C)) {
			ClickStopDongGanBtOne(ButtonState.UP);
			ClickStopDongGanBtTwo(ButtonState.UP);
			ClickStopDongGanBtThree(ButtonState.UP);
			ClickStopDongGanBtFour(ButtonState.UP);
		}
	}
}

public enum ButtonState : int
{
	UP = 1,
	DOWN = -1
}