using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class HardwareCheckCtrl : MonoBehaviour
{
	public UILabel[] BiZhiLable;
	public UILabel[] FangXiangLable;
	public UILabel[] YouMenLable;
	public UILabel[] QiangPosLable;
	public UILabel[] QiangZDLable;
	public UILabel AnJianLable;
	public UILabel StartLedP1;
	public UILabel StartLedP2;
	public UILabel StartLedP3;
	public UILabel StartLedP4;
	public UILabel StartLedP5;
	/**
	 * 0-3   --> 1P.
	 * 4-7   --> 2P.
	 * 8-11  --> 3P.
	 * 12-15 --> 4P.
	 */
	public UILabel[] QiNangLabel;
	public GameObject JiaMiCeShiObj;
	public bool IsJiaMiCeShi;
	int StartLedNumP1 = 1;
	int StartLedNumP2 = 1;
	int StartLedNumP3 = 1;
	int StartLedNumP4 = 1;
	int StartLedNumP5 = 1;
	public static bool IsTestHardWare;
	public static HardwareCheckCtrl Instance;

	// Use this for initialization
	void Start()
	{
		//pcvr.TKMoveSt = TKMoveSt;
		Screen.SetResolution(1280, 720, false);
		Instance = this;
		IsTestHardWare = true;
		JiaMiCeShiObj.SetActive(IsJiaMiCeShi);
		BiZhiLable[0].text = "0";
		BiZhiLable[1].text = "0";
		BiZhiLable[2].text = "0";
		BiZhiLable[3].text = "0";
		AnJianLable.text = "...";
		HandleHiddenObjInfo();
		SetZuoYiActiveShangP1(false);
		SetZuoYiActiveZhongP1(false);
		SetZuoYiActiveXiaP1(false);
		
		SetZuoYiActiveShangP2(false);
		SetZuoYiActiveZhongP2(false);
		SetZuoYiActiveXiaP2(false);

		SetZuoYiActiveShangP3(false);
		SetZuoYiActiveZhongP3(false);
		SetZuoYiActiveXiaP3(false);
		
		SetZuoYiActiveShangP4(false);
		SetZuoYiActiveZhongP4(false);
		SetZuoYiActiveXiaP4(false);
		HiddenYaoGanDuiGou();

		HardwareBtCtrl.StartLedP1 = StartLedP1;
		HardwareBtCtrl.StartLedP2 = StartLedP2;

		InputEventCtrl.GetInstance().ClickSetEnterBtEvent += ClickSetEnterBtEvent;
		InputEventCtrl.GetInstance().ClickSetMoveBtEvent += ClickSetMoveBtEvent;
		InputEventCtrl.GetInstance().ClickFireBtOneEvent += ClickFireBtOneEvent;
		InputEventCtrl.GetInstance().ClickFireBtTwoEvent += ClickFireBtTwoEvent;
		InputEventCtrl.GetInstance().ClickFireBtThreeEvent += ClickFireBtThreeEvent;
		InputEventCtrl.GetInstance().ClickFireBtFourEvent += ClickFireBtFourEvent;
		InputEventCtrl.GetInstance().ClickDaoDanBtOneEvent += ClickDaoDanBtOneEvent;
		InputEventCtrl.GetInstance().ClickDaoDanBtTwoEvent += ClickDaoDanBtTwoEvent;
		InputEventCtrl.GetInstance().ClickDaoDanBtThreeEvent += ClickDaoDanBtThreeEvent;
		InputEventCtrl.GetInstance().ClickDaoDanBtFourEvent += ClickDaoDanBtFourEvent;
		InputEventCtrl.GetInstance().ClickStartBtOneEvent += ClickStartP1BtEvent;
		InputEventCtrl.GetInstance().ClickStartBtTwoEvent += ClickStartP2BtEvent;
		InputEventCtrl.GetInstance().ClickStartBtThreeEvent += ClickStartP3BtEvent;
		InputEventCtrl.GetInstance().ClickStartBtFourEvent += ClickStartP4BtEvent;
		InputEventCtrl.GetInstance().ClickStopDongGanBtOneEvent += ClickStopDongGanBtOneEvent;
		InputEventCtrl.GetInstance().ClickStopDongGanBtTwoEvent += ClickStopDongGanBtTwoEvent;
		InputEventCtrl.GetInstance().ClickStopDongGanBtThreeEvent += ClickStopDongGanBtThreeEvent;
		InputEventCtrl.GetInstance().ClickStopDongGanBtFourEvent += ClickStopDongGanBtFourEvent;
	}
	
	// Update is called once per frame
	void Update()
	{
		UpdateYaoGanDuiGou();
		BiZhiLable[0].text = XKGlobalData.CoinPlayerOne.ToString();
		BiZhiLable[1].text = XKGlobalData.CoinPlayerTwo.ToString();
		BiZhiLable[2].text = XKGlobalData.CoinPlayerThree.ToString();
		BiZhiLable[3].text = XKGlobalData.CoinPlayerFour.ToString();
		if (!pcvr.bIsHardWare) {
			FangXiangLable[0].text = InputEventCtrl.PlayerFX[0].ToString();
			FangXiangLable[1].text = InputEventCtrl.PlayerFX[1].ToString();
			FangXiangLable[2].text = InputEventCtrl.PlayerFX[2].ToString();
			FangXiangLable[3].text = InputEventCtrl.PlayerFX[3].ToString();
			YouMenLable[0].text = InputEventCtrl.PlayerYM[0].ToString();
			YouMenLable[1].text = InputEventCtrl.PlayerYM[1].ToString();
			YouMenLable[2].text = InputEventCtrl.PlayerYM[2].ToString();
			YouMenLable[3].text = InputEventCtrl.PlayerYM[3].ToString();
		}
		else {
			//FangXiangLable[0].text = pcvr.SteerValCurAy[0].ToString();
			//FangXiangLable[1].text = pcvr.SteerValCurAy[1].ToString();
			//FangXiangLable[2].text = pcvr.SteerValCurAy[2].ToString();
			//FangXiangLable[3].text = pcvr.SteerValCurAy[3].ToString();
			//YouMenLable[0].text = pcvr.YouMenCurVal[0].ToString();
			//YouMenLable[1].text = pcvr.YouMenCurVal[1].ToString();
			//YouMenLable[2].text = pcvr.YouMenCurVal[2].ToString();
			//YouMenLable[3].text = pcvr.YouMenCurVal[3].ToString();
		}
	}

	/*void OnGUI()
	{
		string strA = "PlayerFX -> "+InputEventCtrl.PlayerFX[0].ToString("f1")
			+", "+InputEventCtrl.PlayerFX[1].ToString("f1")
			+", "+InputEventCtrl.PlayerFX[2].ToString("f1")
			+", "+InputEventCtrl.PlayerFX[3].ToString("f1")
			+", PlayerYM -> "+InputEventCtrl.PlayerYM[0].ToString("f1")
			+", "+InputEventCtrl.PlayerYM[1].ToString("f1")
			+", "+InputEventCtrl.PlayerYM[2].ToString("f1")
			+", "+InputEventCtrl.PlayerYM[3].ToString("f1");
		GUI.Label(new Rect(10f, 25f, Screen.width, 30f), strA);
	}*/

	void ClickSetEnterBtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "设置 Down";
		}
		else {
			AnJianLable.text = "设置 Up";
		}
	}

	void ClickSetMoveBtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "移动 Down";
		}
		else {
			AnJianLable.text = "移动 Up";
		}
	}

	void ClickStartP1BtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "1P开始 Down";
		}
		else {
			AnJianLable.text = "1P开始 Up";
		}
	}

	void ClickStartP2BtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "2P开始 Down";
		}
		else {
			AnJianLable.text = "2P开始 Up";
		}
	}
	
	void ClickStartP3BtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "3P开始 Down";
		}
		else {
			AnJianLable.text = "3P开始 Up";
		}
	}
	
	void ClickStartP4BtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "4P开始 Down";
		}
		else {
			AnJianLable.text = "4P开始 Up";
		}
	}
	
	void ClickStopDongGanBtOneEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "1P紧急 Down";
		}
		else {
			AnJianLable.text = "1P紧急 Up";
		}
	}

	void ClickStopDongGanBtTwoEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "2P紧急 Down";
		}
		else {
			AnJianLable.text = "2P紧急 Up";
		}
	}

	void ClickStopDongGanBtThreeEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "3P紧急 Down";
		}
		else {
			AnJianLable.text = "3P紧急 Up";
		}
	}
	
	void ClickStopDongGanBtFourEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "4P紧急 Down";
		}
		else {
			AnJianLable.text = "4P紧急 Up";
		}
	}

	void ClickFireBtOneEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "1P射击 Down";
		}
		else {
			AnJianLable.text = "1P射击 Up";
		}
	}

	void ClickFireBtTwoEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "2P射击 Down";
		}
		else {
			AnJianLable.text = "2P射击 Up";
		}
	}
	
	void ClickFireBtThreeEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "3P射击 Down";
		}
		else {
			AnJianLable.text = "3P射击 Up";
		}
	}
	
	void ClickFireBtFourEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "4P射击 Down";
		}
		else {
			AnJianLable.text = "4P射击 Up";
		}
	}
	
	void ClickDaoDanBtOneEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "1P导弹 Down";
		}
		else {
			AnJianLable.text = "1P导弹 Up";
		}
	}
	
	void ClickDaoDanBtTwoEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "2P导弹 Down";
		}
		else {
			AnJianLable.text = "2P导弹 Up";
		}
	}
	
	void ClickDaoDanBtThreeEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "3P导弹 Down";
		}
		else {
			AnJianLable.text = "3P导弹 Up";
		}
	}

	void ClickDaoDanBtFourEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "4P导弹 Down";
		}
		else {
			AnJianLable.text = "4P导弹 Up";
		}
	}

	public UILabel JiaMiJYMsg;
	public static bool IsOpenJiaMiJiaoYan;
	void CloseJiaMiJiaoYanFailed()
	{
		if (!IsInvoking("JiaMiJiaoYanFailed")) {
			return;
		}
		CancelInvoke("JiaMiJiaoYanFailed");
	}

	public void JiaMiJiaoYanFailed()
	{
		SetJiaMiJYMsg("", JiaMiJiaoYanEnum.Failed);
	}
	
	public void JiaMiJiaoYanSucceed()
	{
		SetJiaMiJYMsg("", JiaMiJiaoYanEnum.Succeed);
	}

	public void SetJiaMiJYMsg(string msg, JiaMiJiaoYanEnum key)
	{
		switch (key) {
		case JiaMiJiaoYanEnum.Succeed:
			CloseJiaMiJiaoYanFailed();
			JiaMiJYMsg.text = "校验成功";
			ResetJiaMiJYLabelInfo();
			ScreenLog.Log("校验成功");
			break;
			
		case JiaMiJiaoYanEnum.Failed:
			CloseJiaMiJiaoYanFailed();
			JiaMiJYMsg.text = "校验失败";
			ResetJiaMiJYLabelInfo();
			ScreenLog.Log("校验失败");
			break;
			
		default:
			JiaMiJYMsg.text = msg;
			ScreenLog.Log(msg);
			break;
		}
	}
	
	public static void CloseJiaMiJiaoYan()
	{
		if (!IsOpenJiaMiJiaoYan) {
			return;
		}
		IsOpenJiaMiJiaoYan = false;
	}

	void ResetJiaMiJYLabelInfo()
	{
		CloseJiaMiJiaoYan();
	}

	public void SubCoinPOne()
	{
		if (XKGlobalData.CoinPlayerOne < 1) {
			return;
		}
		XKGlobalData.CoinPlayerOne--;
		pcvr.GetInstance().SubPlayerCoin(PlayerEnum.PlayerOne, 1);
	}
	
	public void SubCoinPTwo()
	{
		if (XKGlobalData.CoinPlayerTwo < 1) {
			return;
		}
		XKGlobalData.CoinPlayerTwo--;
		pcvr.GetInstance().SubPlayerCoin(PlayerEnum.PlayerTwo, 1);
	}
	
	public void SubCoinPThree()
	{
		if (XKGlobalData.CoinPlayerThree < 1) {
			return;
		}
		XKGlobalData.CoinPlayerThree--;
		pcvr.GetInstance().SubPlayerCoin(PlayerEnum.PlayerThree, 1);
	}
	
	public void SubCoinPFour()
	{
		if (XKGlobalData.CoinPlayerFour < 1) {
			return;
		}
		XKGlobalData.CoinPlayerFour--;
		pcvr.GetInstance().SubPlayerCoin(PlayerEnum.PlayerFour, 1);
	}

	public void StartLedCheckP1()
	{
		StartLedNumP1++;
		switch (StartLedNumP1) {
		case 1:
			StartLedP1.text = "1P开始灯亮";
			//pcvr.StartLightStateP1 = LedState.Liang;
			break;
			
		case 2:
			StartLedP1.text = "1P开始灯闪";
			//pcvr.StartLightStateP1 = LedState.Shan;
			break;
			
		case 3:
			StartLedP1.text = "1P开始灯灭";
			//pcvr.StartLightStateP1 = LedState.Mie;
			StartLedNumP1 = 1;
			break;
		}
	}
	
	public void StartLedCheckP2()
	{
		StartLedNumP2++;
		switch (StartLedNumP2) {
		case 1:
			StartLedP2.text = "2P开始灯亮";
			//pcvr.StartLightStateP2 = LedState.Liang;
			break;
			
		case 2:
			StartLedP2.text = "2P开始灯闪";
			//pcvr.StartLightStateP2 = LedState.Shan;
			break;
			
		case 3:
			StartLedP2.text = "2P开始灯灭";
			//pcvr.StartLightStateP2 = LedState.Mie;
			StartLedNumP2 = 1;
			break;
		}
	}

	public void StartLedCheckP3()
	{
		StartLedNumP3++;
		switch (StartLedNumP3) {
		case 1:
			StartLedP3.text = "3P开始灯亮";
			//pcvr.StartLightStateP3 = LedState.Liang;
			break;
			
		case 2:
			StartLedP3.text = "3P开始灯闪";
			//pcvr.StartLightStateP3 = LedState.Shan;
			break;
			
		case 3:
			StartLedP3.text = "3P开始灯灭";
			//pcvr.StartLightStateP3 = LedState.Mie;
			StartLedNumP3 = 1;
			break;
		}
	}

	public void StartLedCheckP4()
	{
		StartLedNumP4++;
		switch (StartLedNumP4) {
		case 1:
			StartLedP4.text = "4P开始灯亮";
			//pcvr.StartLightStateP4 = LedState.Liang;
			break;
			
		case 2:
			StartLedP4.text = "4P开始灯闪";
			//pcvr.StartLightStateP4 = LedState.Shan;
			break;
			
		case 3:
			StartLedP4.text = "4P开始灯灭";
			//pcvr.StartLightStateP4 = LedState.Mie;
			StartLedNumP4 = 1;
			break;
		}
	}

	
	public void StartLedCheckP5()
	{
		StartLedNumP5++;
		switch (StartLedNumP5) {
		case 1:
			StartLedP5.text = "5P开始灯亮";
			//pcvr.StartLightStateP5 = LedState.Liang;
			break;
			
		case 2:
			StartLedP5.text = "5P开始灯闪";
			//pcvr.StartLightStateP5 = LedState.Shan;
			break;
			
		case 3:
			StartLedP5.text = "5P开始灯灭";
			//pcvr.StartLightStateP5 = LedState.Mie;
			StartLedNumP5 = 1;
			break;
		}
	}

	public void OnClickCloseAppBt()
	{
		Application.Quit();
	}
	
	public void OnClickRestartAppBt()
	{
		Application.Quit();
		RunCmd("start ComTest.exe");
	}

	public static void OnRestartGame()
	{
		if (IsTestHardWare) {
			return;
		}
		Application.Quit();
		RunCmd("start BlazeTanks.exe");
	}

	static void RunCmd(string command)
	{
		//實例一個Process類，啟動一個獨立進程    
		Process p = new Process();    //Process類有一個StartInfo屬性，這個是ProcessStartInfo類，    
		//包括了一些屬性和方法，下面我們用到了他的幾個屬性：   
		p.StartInfo.FileName = "cmd.exe";           //設定程序名   
		p.StartInfo.Arguments = "/c " + command;    //設定程式執行參數   
		p.StartInfo.UseShellExecute = false;        //關閉Shell的使用    p.StartInfo.RedirectStandardInput = true;   //重定向標準輸入    p.StartInfo.RedirectStandardOutput = true;  //重定向標準輸出   
		p.StartInfo.RedirectStandardError = true;   //重定向錯誤輸出    
		p.StartInfo.CreateNoWindow = true;          //設置不顯示窗口    
		p.Start();   //啟動
	}

	public void StartJiaoYanIO()
	{
		//pcvr.GetInstance().StartJiaoYanIO();
	}

	int GetQiNangIndexVal(int playerIndex, int qiNangIndex)
	{
		return qiNangIndex + (playerIndex - 1) * 4;
	}

	public void OnClickQiNangBtP1_1()
	{
		//int indexVal = GetQiNangIndexVal(1, 0);
		//QiNangLabel[indexVal].text = QiNangLabel[indexVal].text != "1气囊充气" ? "1气囊充气" : "1气囊放气";
		//pcvr.QiNangArray[indexVal] = (byte)(pcvr.QiNangArray[indexVal] != 1 ? 1 : 0);
	}

	public void OnClickQiNangBtP1_2()
	{
		//int indexVal = GetQiNangIndexVal(1, 1);
		//QiNangLabel[indexVal].text = QiNangLabel[indexVal].text != "2气囊充气" ? "2气囊充气" : "2气囊放气";
		//pcvr.QiNangArray[indexVal] = (byte)(pcvr.QiNangArray[indexVal] != 1 ? 1 : 0);
	}

	public void OnClickQiNangBtP1_3()
	{
		//int indexVal = GetQiNangIndexVal(1, 2);
		//QiNangLabel[indexVal].text = QiNangLabel[indexVal].text != "3气囊充气" ? "3气囊充气" : "3气囊放气";
		//pcvr.QiNangArray[indexVal] = (byte)(pcvr.QiNangArray[indexVal] != 1 ? 1 : 0);
	}

	public void OnClickQiNangBtP1_4()
	{
		//int indexVal = GetQiNangIndexVal(1, 3);
		//QiNangLabel[indexVal].text = QiNangLabel[indexVal].text != "4气囊充气" ? "4气囊充气" : "4气囊放气";
		//pcvr.QiNangArray[indexVal] = (byte)(pcvr.QiNangArray[indexVal] != 1 ? 1 : 0);
	}
	
	public void OnClickQiNangBtP1_5()
	{
		//int indexVal = 16;
		//QiNangLabel[indexVal].text = QiNangLabel[indexVal].text != "5气囊充气" ? "5气囊充气" : "5气囊放气";
		//pcvr.QiNangArray[indexVal] = (byte)(pcvr.QiNangArray[indexVal] != 1 ? 1 : 0);
	}
	
	public void OnClickQiNangBtP2_1()
	{
		//int indexVal = GetQiNangIndexVal(2, 0);
		//QiNangLabel[indexVal].text = QiNangLabel[indexVal].text != "1气囊充气" ? "1气囊充气" : "1气囊放气";
		//pcvr.QiNangArray[indexVal] = (byte)(pcvr.QiNangArray[indexVal] != 1 ? 1 : 0);
	}
	
	public void OnClickQiNangBtP2_2()
	{
		//int indexVal = GetQiNangIndexVal(2, 1);
		//QiNangLabel[indexVal].text = QiNangLabel[indexVal].text != "2气囊充气" ? "2气囊充气" : "2气囊放气";
		//pcvr.QiNangArray[indexVal] = (byte)(pcvr.QiNangArray[indexVal] != 1 ? 1 : 0);
	}
	
	public void OnClickQiNangBtP2_3()
	{
		//int indexVal = GetQiNangIndexVal(2, 2);
		//QiNangLabel[indexVal].text = QiNangLabel[indexVal].text != "3气囊充气" ? "3气囊充气" : "3气囊放气";
		//pcvr.QiNangArray[indexVal] = (byte)(pcvr.QiNangArray[indexVal] != 1 ? 1 : 0);
	}
	
	public void OnClickQiNangBtP2_4()
	{
		//int indexVal = GetQiNangIndexVal(2, 3);
		//QiNangLabel[indexVal].text = QiNangLabel[indexVal].text != "4气囊充气" ? "4气囊充气" : "4气囊放气";
		//pcvr.QiNangArray[indexVal] = (byte)(pcvr.QiNangArray[indexVal] != 1 ? 1 : 0);
	}
	
	public void OnClickQiNangBtP2_5()
	{
		//int indexVal = 17;
		//QiNangLabel[indexVal].text = QiNangLabel[indexVal].text != "5气囊充气" ? "5气囊充气" : "5气囊放气";
		//pcvr.QiNangArray[indexVal] = (byte)(pcvr.QiNangArray[indexVal] != 1 ? 1 : 0);
	}
	
	public void OnClickQiNangBtP3_1()
	{
		//int indexVal = GetQiNangIndexVal(3, 0);
		//QiNangLabel[indexVal].text = QiNangLabel[indexVal].text != "1气囊充气" ? "1气囊充气" : "1气囊放气";
		//pcvr.QiNangArray[indexVal] = (byte)(pcvr.QiNangArray[indexVal] != 1 ? 1 : 0);
	}
	
	public void OnClickQiNangBtP3_2()
	{
		//int indexVal = GetQiNangIndexVal(3, 1);
		//QiNangLabel[indexVal].text = QiNangLabel[indexVal].text != "2气囊充气" ? "2气囊充气" : "2气囊放气";
		//pcvr.QiNangArray[indexVal] = (byte)(pcvr.QiNangArray[indexVal] != 1 ? 1 : 0);
	}
	
	public void OnClickQiNangBtP3_3()
	{
		//int indexVal = GetQiNangIndexVal(3, 2);
		//QiNangLabel[indexVal].text = QiNangLabel[indexVal].text != "3气囊充气" ? "3气囊充气" : "3气囊放气";
		//pcvr.QiNangArray[indexVal] = (byte)(pcvr.QiNangArray[indexVal] != 1 ? 1 : 0);
	}
	
	public void OnClickQiNangBtP3_4()
	{
		//int indexVal = GetQiNangIndexVal(3, 3);
		//QiNangLabel[indexVal].text = QiNangLabel[indexVal].text != "4气囊充气" ? "4气囊充气" : "4气囊放气";
		//pcvr.QiNangArray[indexVal] = (byte)(pcvr.QiNangArray[indexVal] != 1 ? 1 : 0);
	}
	
	public void OnClickQiNangBtP3_5()
	{
		//int indexVal = 18;
		//QiNangLabel[indexVal].text = QiNangLabel[indexVal].text != "5气囊充气" ? "5气囊充气" : "5气囊放气";
		//pcvr.QiNangArray[indexVal] = (byte)(pcvr.QiNangArray[indexVal] != 1 ? 1 : 0);
	}
	
	public void OnClickQiNangBtP4_1()
	{
		//int indexVal = GetQiNangIndexVal(4, 0);
		//QiNangLabel[indexVal].text = QiNangLabel[indexVal].text != "1气囊充气" ? "1气囊充气" : "1气囊放气";
		//pcvr.QiNangArray[indexVal] = (byte)(pcvr.QiNangArray[indexVal] != 1 ? 1 : 0);
	}
	
	public void OnClickQiNangBtP4_2()
    {
        //	int indexVal = GetQiNangIndexVal(4, 1);
        //	QiNangLabel[indexVal].text = QiNangLabel[indexVal].text != "2气囊充气" ? "2气囊充气" : "2气囊放气";
        //	pcvr.QiNangArray[indexVal] = (byte)(pcvr.QiNangArray[indexVal] != 1 ? 1 : 0);
    }
	
	public void OnClickQiNangBtP4_3()
	{
		//int indexVal = GetQiNangIndexVal(4, 2);
		//QiNangLabel[indexVal].text = QiNangLabel[indexVal].text != "3气囊充气" ? "3气囊充气" : "3气囊放气";
		//pcvr.QiNangArray[indexVal] = (byte)(pcvr.QiNangArray[indexVal] != 1 ? 1 : 0);
	}
	
	public void OnClickQiNangBtP4_4()
	{
		//int indexVal = GetQiNangIndexVal(4, 3);
		//QiNangLabel[indexVal].text = QiNangLabel[indexVal].text != "4气囊充气" ? "4气囊充气" : "4气囊放气";
		//pcvr.QiNangArray[indexVal] = (byte)(pcvr.QiNangArray[indexVal] != 1 ? 1 : 0);
	}
	
	public void OnClickQiNangBtP4_5()
	{
		//int indexVal = 19;
		//QiNangLabel[indexVal].text = QiNangLabel[indexVal].text != "5气囊充气" ? "5气囊充气" : "5气囊放气";
		//pcvr.QiNangArray[indexVal] = (byte)(pcvr.QiNangArray[indexVal] != 1 ? 1 : 0);
	}

	int[] FangXiangDouDongSt = {0, 0, 0, 0};
	public UILabel[] FangXiangDouDongLB;
	public void SetFangXiangDouDongP1()
	{
		int playerIndex = 0;
		UnityEngine.Debug.Log("Unity:"+"SetFangXiangDouDong -> p"+playerIndex
		          +": FangXiangDouDongSt "+FangXiangDouDongSt[playerIndex]);
		switch (FangXiangDouDongSt[playerIndex]) {
		case 0:
			FangXiangDouDongSt[playerIndex] = 1;
			FangXiangDouDongLB[0].text = "1P方向关闭";
			//pcvr.GetInstance().ActiveFangXiangDouDong(PlayerEnum.PlayerOne, IsLoopDouDongFX);
			break;
		case 1:
			FangXiangDouDongSt[playerIndex] = 0;
			FangXiangDouDongLB[0].text = "1P方向抖动";
			//pcvr.GetInstance().CloseFangXiangPanPower(PlayerEnum.PlayerOne);
			break;
		}
	}
	
	public void SetFangXiangDouDongP2()
	{
		int playerIndex = 1;
		UnityEngine.Debug.Log("Unity:"+"SetFangXiangDouDong -> p"+playerIndex
		          +": FangXiangDouDongSt "+FangXiangDouDongSt[playerIndex]);
		switch (FangXiangDouDongSt[playerIndex]) {
		case 0:
			FangXiangDouDongSt[playerIndex] = 1;
			FangXiangDouDongLB[1].text = "2P方向关闭";
			//pcvr.GetInstance().ActiveFangXiangDouDong(PlayerEnum.PlayerTwo, IsLoopDouDongFX);
			break;
		case 1:
			FangXiangDouDongSt[playerIndex] = 0;
			FangXiangDouDongLB[1].text = "2P方向抖动";
			//pcvr.GetInstance().CloseFangXiangPanPower(PlayerEnum.PlayerTwo);
			break;
		}
	}

	public void SetFangXiangDouDongP3()
	{
		int playerIndex = 2;
		UnityEngine.Debug.Log("Unity:"+"SetFangXiangDouDong -> p"+playerIndex
		          +": FangXiangDouDongSt "+FangXiangDouDongSt[playerIndex]);
		switch (FangXiangDouDongSt[playerIndex]) {
		case 0:
			FangXiangDouDongSt[playerIndex] = 1;
			FangXiangDouDongLB[2].text = "3P方向关闭";
			//pcvr.GetInstance().ActiveFangXiangDouDong(PlayerEnum.PlayerThree, IsLoopDouDongFX);
			break;
		case 1:
			FangXiangDouDongSt[playerIndex] = 0;
			FangXiangDouDongLB[2].text = "3P方向抖动";
			//pcvr.GetInstance().CloseFangXiangPanPower(PlayerEnum.PlayerThree);
			break;
		}
	}

	public void SetFangXiangDouDongP4()
	{
		int playerIndex = 3;
		UnityEngine.Debug.Log("Unity:"+"SetFangXiangDouDong -> p"+playerIndex
		          +": FangXiangDouDongSt "+FangXiangDouDongSt[playerIndex]);
		switch (FangXiangDouDongSt[playerIndex]) {
		case 0:
			FangXiangDouDongSt[playerIndex] = 1;
			FangXiangDouDongLB[3].text = "4P方向关闭";
			//pcvr.GetInstance().ActiveFangXiangDouDong(PlayerEnum.PlayerFour, IsLoopDouDongFX);
			break;
		case 1:
			FangXiangDouDongSt[playerIndex] = 0;
			FangXiangDouDongLB[3].text = "4P方向抖动";
			//pcvr.GetInstance().CloseFangXiangPanPower(PlayerEnum.PlayerFour);
			break;
		}
	}

	bool IsLoopDouDongFX = true;
	public void OnClickLoopFangXiangDouDong()
	{
		IsLoopDouDongFX = !IsLoopDouDongFX;
		UnityEngine.Debug.Log("Unity:"+"OnClickLoopFangXiangDouDong -> IsLoopDouDongFX "+IsLoopDouDongFX);
	}

	public void OnClickZuoYiShangP1()
	{
		UnityEngine.Debug.Log("Unity:"+"OnClickZuoYiShangP1...");
		//pcvr.GetInstance().SetZuoYiDianJiSpeed(PlayerEnum.PlayerOne, 1);
	}

	public void OnClickZuoYiZhongP1()
	{
		UnityEngine.Debug.Log("Unity:"+"OnClickZuoYiZhongP1...");
		//pcvr.GetInstance().MakeZuoYiMoveCenter(PlayerEnum.PlayerOne);
	}
	
	public void OnClickZuoYiXiaP1()
	{
		UnityEngine.Debug.Log("Unity:"+"OnClickZuoYiXiaP1...");
		//pcvr.GetInstance().SetZuoYiDianJiSpeed(PlayerEnum.PlayerOne, -1);
	}

	public GameObject[] ZuoYiDuiGouP1;
	public void SetZuoYiActiveShangP1(bool isActive)
	{
		ZuoYiDuiGouP1[0].SetActive(isActive);
	}
	
	public void SetZuoYiActiveZhongP1(bool isActive)
	{
		ZuoYiDuiGouP1[1].SetActive(isActive);
	}

	public void SetZuoYiActiveXiaP1(bool isActive)
	{
		ZuoYiDuiGouP1[2].SetActive(isActive);
	}

	public void OnClickZuoYiShangP2()
	{
		UnityEngine.Debug.Log("Unity:"+"OnClickZuoYiShangP2...");
		//pcvr.GetInstance().SetZuoYiDianJiSpeed(PlayerEnum.PlayerTwo, 1);
	}
	
	public void OnClickZuoYiZhongP2()
	{
		UnityEngine.Debug.Log("Unity:"+"OnClickZuoYiZhongP2...");
		//pcvr.GetInstance().MakeZuoYiMoveCenter(PlayerEnum.PlayerTwo);
	}
	
	public void OnClickZuoYiXiaP2()
	{
		UnityEngine.Debug.Log("Unity:"+"OnClickZuoYiXiaP2...");
		//pcvr.GetInstance().SetZuoYiDianJiSpeed(PlayerEnum.PlayerTwo, -1);
	}

	public GameObject[] ZuoYiDuiGouP2;
	public void SetZuoYiActiveShangP2(bool isActive)
	{
		ZuoYiDuiGouP2[0].SetActive(isActive);
	}
	
	public void SetZuoYiActiveZhongP2(bool isActive)
	{
		ZuoYiDuiGouP2[1].SetActive(isActive);
	}
	
	public void SetZuoYiActiveXiaP2(bool isActive)
	{
		ZuoYiDuiGouP2[2].SetActive(isActive);
	}
	
	public void OnClickZuoYiShangP3()
	{
		UnityEngine.Debug.Log("Unity:"+"OnClickZuoYiShangP3...");
		//pcvr.GetInstance().SetZuoYiDianJiSpeed(PlayerEnum.PlayerThree, 1);
	}
	
	public void OnClickZuoYiZhongP3()
	{
		UnityEngine.Debug.Log("Unity:"+"OnClickZuoYiZhongP3...");
		//pcvr.GetInstance().MakeZuoYiMoveCenter(PlayerEnum.PlayerThree);
	}
	
	public void OnClickZuoYiXiaP3()
	{
		UnityEngine.Debug.Log("Unity:"+"OnClickZuoYiXiaP3...");
		//pcvr.GetInstance().SetZuoYiDianJiSpeed(PlayerEnum.PlayerThree, -1);
	}

	public GameObject[] ZuoYiDuiGouP3;
	public void SetZuoYiActiveShangP3(bool isActive)
	{
		ZuoYiDuiGouP3[0].SetActive(isActive);
	}
	
	public void SetZuoYiActiveZhongP3(bool isActive)
	{
		ZuoYiDuiGouP3[1].SetActive(isActive);
	}
	
	public void SetZuoYiActiveXiaP3(bool isActive)
	{
		ZuoYiDuiGouP3[2].SetActive(isActive);
	}

	public void OnClickZuoYiShangP4()
	{
		UnityEngine.Debug.Log("Unity:"+"OnClickZuoYiShangP4...");
		//pcvr.GetInstance().SetZuoYiDianJiSpeed(PlayerEnum.PlayerFour, 1);
	}
	
	public void OnClickZuoYiZhongP4()
	{
		UnityEngine.Debug.Log("Unity:"+"OnClickZuoYiZhongP4...");
		//pcvr.GetInstance().MakeZuoYiMoveCenter(PlayerEnum.PlayerFour);
	}
	
	public void OnClickZuoYiXiaP4()
	{
		UnityEngine.Debug.Log("Unity:"+"OnClickZuoYiXiaP4...");
		//pcvr.GetInstance().SetZuoYiDianJiSpeed(PlayerEnum.PlayerFour, -1);
	}
	
	public GameObject[] ZuoYiDuiGouP4;
	public void SetZuoYiActiveShangP4(bool isActive)
	{
		ZuoYiDuiGouP4[0].SetActive(isActive);
	}
	
	public void SetZuoYiActiveZhongP4(bool isActive)
	{
		ZuoYiDuiGouP4[1].SetActive(isActive);
	}
	
	public void SetZuoYiActiveXiaP4(bool isActive)
	{
		ZuoYiDuiGouP4[2].SetActive(isActive);
	}

	public UILabel DianJiSpeedLB;
	public void SetDianJiSpeed()
	{
		if (UIProgressBar.current == null) {
			return;
		}
		int valZD = Mathf.RoundToInt((UIProgressBar.current.value * 14f) + 1f);
		DianJiSpeedLB.text = valZD.ToString("X1");
		//pcvr.DianJiSpeedP1 = valZD;
		//pcvr.DianJiSpeedP2 = valZD;
		//pcvr.DianJiSpeedP3 = valZD;
		//pcvr.DianJiSpeedP4 = valZD;
		
		//for (int i = 0; i < pcvr.ZuoYiDianJiSpeedVal.Length; i++) {
		//	if (pcvr.ZuoYiDianJiSpeedVal[i] != 0x00) {
		//		pcvr.ZuoYiDianJiSpeedVal[i] = (byte)((0xf0 & pcvr.ZuoYiDianJiSpeedVal[i]) + (0x0f & valZD));
		//	}
		//}
	}

	public void SetQiangZDValue_1()
	{
		if (UIProgressBar.current == null) {
			return;
		}
		int valZD = Mathf.RoundToInt(UIProgressBar.current.value * 255f);
		QiangZDLable[0].text = valZD.ToString("X2");
		GunZhenDongVal[0] = (byte)valZD;
	}

	public void SetQiangZDValue_2()
	{
		if (UIProgressBar.current == null) {
			return;
		}
		int valZD = Mathf.RoundToInt(UIProgressBar.current.value * 255f);
		QiangZDLable[1].text = valZD.ToString("X2");
		GunZhenDongVal[1] = (byte)valZD;
	}
	
	public void SetQiangZDValue_3()
	{
		if (UIProgressBar.current == null) {
			return;
		}
		int valZD = Mathf.RoundToInt(UIProgressBar.current.value * 255f);
		QiangZDLable[2].text = valZD.ToString("X2");
		GunZhenDongVal[2] = (byte)valZD;
	}
	
	public void SetQiangZDValue_4()
	{
		if (UIProgressBar.current == null) {
			return;
		}
		int valZD = Mathf.RoundToInt(UIProgressBar.current.value * 255f);
		QiangZDLable[3].text = valZD.ToString("X2");
		GunZhenDongVal[3] = (byte)valZD;
	}
	public static byte[] GunZhenDongVal = new byte[4];

	public GameObject[] YaoGanDuiGouP1;
	public GameObject[] YaoGanDuiGouP2;
	public GameObject[] YaoGanDuiGouP3;
	public GameObject[] YaoGanDuiGouP4;
	void HiddenYaoGanDuiGou()
	{
		for (int i = 0; i < 4; i++) {
			YaoGanDuiGouP1[i].SetActive(false);
			YaoGanDuiGouP2[i].SetActive(false);
			YaoGanDuiGouP3[i].SetActive(false);
			YaoGanDuiGouP4[i].SetActive(false);
		}
	}

	void UpdateYaoGanDuiGou()
	{
		if (InputEventCtrl.PlayerFX[0] > 0f) {
			YaoGanDuiGouP1[2].SetActive(false);
			YaoGanDuiGouP1[3].SetActive(true);
		}
		
		if (InputEventCtrl.PlayerFX[0] == 0f) {
			YaoGanDuiGouP1[2].SetActive(false);
			YaoGanDuiGouP1[3].SetActive(false);
		}
		
		if (InputEventCtrl.PlayerFX[0] < 0f) {
			YaoGanDuiGouP1[2].SetActive(true);
			YaoGanDuiGouP1[3].SetActive(false);
		}
		
		if (InputEventCtrl.PlayerYM[0] > 0f) {
			YaoGanDuiGouP1[0].SetActive(true);
			YaoGanDuiGouP1[1].SetActive(false);
		}

		if (InputEventCtrl.PlayerYM[0] == 0f) {
			YaoGanDuiGouP1[0].SetActive(false);
			YaoGanDuiGouP1[1].SetActive(false);
		}
		
		if (InputEventCtrl.PlayerYM[0] < 0f) {
			YaoGanDuiGouP1[0].SetActive(false);
			YaoGanDuiGouP1[1].SetActive(true);
		}
		
		if (InputEventCtrl.PlayerFX[1] > 0f) {
			YaoGanDuiGouP2[2].SetActive(false);
			YaoGanDuiGouP2[3].SetActive(true);
		}
		
		if (InputEventCtrl.PlayerFX[1] == 0f) {
			YaoGanDuiGouP2[2].SetActive(false);
			YaoGanDuiGouP2[3].SetActive(false);
		}
		
		if (InputEventCtrl.PlayerFX[1] < 0f) {
			YaoGanDuiGouP2[2].SetActive(true);
			YaoGanDuiGouP2[3].SetActive(false);
		}
		
		if (InputEventCtrl.PlayerYM[1] > 0f) {
			YaoGanDuiGouP2[0].SetActive(true);
			YaoGanDuiGouP2[1].SetActive(false);
		}
		
		if (InputEventCtrl.PlayerYM[1] == 0f) {
			YaoGanDuiGouP2[0].SetActive(false);
			YaoGanDuiGouP2[1].SetActive(false);
		}
		
		if (InputEventCtrl.PlayerYM[1] < 0f) {
			YaoGanDuiGouP2[0].SetActive(false);
			YaoGanDuiGouP2[1].SetActive(true);
		}
		
		if (InputEventCtrl.PlayerFX[2] > 0f) {
			YaoGanDuiGouP3[2].SetActive(false);
			YaoGanDuiGouP3[3].SetActive(true);
		}
		
		if (InputEventCtrl.PlayerFX[2] == 0f) {
			YaoGanDuiGouP3[2].SetActive(false);
			YaoGanDuiGouP3[3].SetActive(false);
		}
		
		if (InputEventCtrl.PlayerFX[2] < 0f) {
			YaoGanDuiGouP3[2].SetActive(true);
			YaoGanDuiGouP3[3].SetActive(false);
		}
		
		if (InputEventCtrl.PlayerYM[2] > 0f) {
			YaoGanDuiGouP3[0].SetActive(true);
			YaoGanDuiGouP3[1].SetActive(false);
		}
		
		if (InputEventCtrl.PlayerYM[2] == 0f) {
			YaoGanDuiGouP3[0].SetActive(false);
			YaoGanDuiGouP3[1].SetActive(false);
		}
		
		if (InputEventCtrl.PlayerYM[2] < 0f) {
			YaoGanDuiGouP3[0].SetActive(false);
			YaoGanDuiGouP3[1].SetActive(true);
		}
		
		if (InputEventCtrl.PlayerFX[3] > 0f) {
			YaoGanDuiGouP4[2].SetActive(false);
			YaoGanDuiGouP4[3].SetActive(true);
		}
		
		if (InputEventCtrl.PlayerFX[3] == 0f) {
			YaoGanDuiGouP4[2].SetActive(false);
			YaoGanDuiGouP4[3].SetActive(false);
		}
		
		if (InputEventCtrl.PlayerFX[3] < 0f) {
			YaoGanDuiGouP4[2].SetActive(true);
			YaoGanDuiGouP4[3].SetActive(false);
		}
		
		if (InputEventCtrl.PlayerYM[3] > 0f) {
			YaoGanDuiGouP4[0].SetActive(true);
			YaoGanDuiGouP4[1].SetActive(false);
		}
		
		if (InputEventCtrl.PlayerYM[3] == 0f) {
			YaoGanDuiGouP4[0].SetActive(false);
			YaoGanDuiGouP4[1].SetActive(false);
		}
		
		if (InputEventCtrl.PlayerYM[3] < 0f) {
			YaoGanDuiGouP4[0].SetActive(false);
			YaoGanDuiGouP4[1].SetActive(true);
		}
	}
	
	public TKMoveState TKMoveSt = TKMoveState.U_FangXiangPan;
	/**
	 * U型方向盘要隐藏的对象.
	 */
	public GameObject[] HiddenObjUDir;
	/**
	 * 摇杆版要隐藏的对象.
	 */
	public GameObject[] HiddenObjYGDir;
	void HandleHiddenObjInfo()
	{
		switch (TKMoveSt) {
		case TKMoveState.U_FangXiangPan:
			for (int i = 0; i < HiddenObjYGDir.Length; i++) {
				HiddenObjYGDir[i].SetActive(false);
			}
			break;
		case TKMoveState.YaoGanBan:
			for (int i = 0; i < HiddenObjUDir.Length; i++) {
				HiddenObjUDir[i].SetActive(false);
			}
			break;
		}
	}

	public UILabel[] ActivePlayerLB;
	public void OnClickActiveP1()
	{
		int indexVal = 0;
		ActivePlayerLB[indexVal].text = ActivePlayerLB[indexVal].text == "1P未激活" ? "1P激活" : "1P未激活";
		XkGameCtrl.IsActivePlayerOne = XkGameCtrl.IsActivePlayerOne == false ? true : false;
	}

	public void OnClickActiveP2()
	{
		int indexVal = 1;
		ActivePlayerLB[indexVal].text = ActivePlayerLB[indexVal].text == "2P未激活" ? "2P激活" : "2P未激活";
		XkGameCtrl.IsActivePlayerTwo = XkGameCtrl.IsActivePlayerTwo == false ? true : false;
	}

	public void OnClickActiveP3()
	{
		int indexVal = 2;
		ActivePlayerLB[indexVal].text = ActivePlayerLB[indexVal].text == "3P未激活" ? "3P激活" : "3P未激活";
		XkGameCtrl.IsActivePlayerThree = XkGameCtrl.IsActivePlayerThree == false ? true : false;
	}

	public void OnClickActiveP4()
	{
		int indexVal = 3;
		ActivePlayerLB[indexVal].text = ActivePlayerLB[indexVal].text == "4P未激活" ? "4P激活" : "4P未激活";
		XkGameCtrl.IsActivePlayerFour = XkGameCtrl.IsActivePlayerFour == false ? true : false;
	}
}

public enum JiaMiJiaoYanEnum
{
	Null,
	Succeed,
	Failed,
}