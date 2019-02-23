#define USE_LINE_CHANGE_DIR
using UnityEngine;
using System.Collections.Generic;

public enum TKMoveState
{
	U_FangXiangPan,		//U型方向盘.
	YaoGanBan,			//摇杆版.
}

public class XKPlayerMoveCtrl : MonoBehaviour
{
    public PlayerEnum PlayerIndex = PlayerEnum.PlayerOne;
	TKMoveState TKMoveSt = TKMoveState.YaoGanBan;
    /// <summary>
    /// 玩家受到JPBoss火焰伤害时的特效.
    /// </summary>
    public GameObject m_HuoYanDamageTX;
    /// <summary>
    /// 新圆圈特效.
    /// </summary>
    public GameObject m_NewCircleTeXiao;
	public UVA LvDaiUVCom;
	public AudioSource PlayerMoveAudio;
	public XKPlayerCheckCamera PlayerPointForward;
	public Transform PiaoFenPoint;
	public GameObject WuDiTXObj;
	GameObject DeathExplodPrefab;
	public GameObject GenZongDanAimPoint;
	/// <summary>
	/// 该变量用来计算车体的前后倾斜角度.
	/// </summary>
	public Transform[] CheTiPointArray;
	bool IsActiveZhuiYa;
	float MaxMvSpeed = 10f;
	float PlayerMvSpeed;
	//float HorizontalSpeedBL;
	float MaxHitPower = 50f;
	Vector3 VecDirSpeed = Vector3.zero;
	Transform PlayerTran;
	Rigidbody RigCom;
	Transform GameCameraTran;
	float LastFXVal;
	bool IsAutoMoving;
	LayerMask TerrainLayer;
	bool IsDeathPlayer = true;
	bool IsWuDiState = true;
	XKPlayerAutoFire FireScript;
	XKPlayerTiaoBanCtrl PlayerTiaoBanScript;
	static XKPlayerMoveCtrl _InstancePOne;
	public static XKPlayerMoveCtrl GetInstancePOne()
	{
		return _InstancePOne;
	}
	
	static XKPlayerMoveCtrl _InstancePTwo;
	public static XKPlayerMoveCtrl GetInstancePTwo()
	{
		return _InstancePTwo;
	}
	
	static XKPlayerMoveCtrl _InstancePThree;
	public static XKPlayerMoveCtrl GetInstancePThree()
	{
		return _InstancePThree;
	}
	
	static XKPlayerMoveCtrl _InstancePFour;
	public static XKPlayerMoveCtrl GetInstancePFour()
	{
		return _InstancePFour;
	}

	public static XKPlayerMoveCtrl GetXKPlayerMoveCom(PlayerEnum playerSt)
	{
		XKPlayerMoveCtrl playerScript = null;
		switch (playerSt)
		{
		case PlayerEnum.PlayerOne:
			playerScript = _InstancePOne;
			break;
		case PlayerEnum.PlayerTwo:
			playerScript = _InstancePTwo;
			break;
		case PlayerEnum.PlayerThree:
			playerScript = _InstancePThree;
			break;
		case PlayerEnum.PlayerFour:
			playerScript = _InstancePFour;
			break;
		}
		return playerScript;
	}
	// Use this for initialization
	void Start()
	{
		if (GameTypeCtrl.IsSetTKMoveSt) {
			//TKMoveSt = GameTypeCtrl.TKMoveStatic;
		}

		TiaoBanObjAy = new GameObject[2];
		TiaoBanObjAy[0] = new GameObject();
		TiaoBanObjAy[1] = new GameObject();
		XKPlayerTiaoBanCtrl PlayerTiaoBanScript = TiaoBanObjAy[0].AddComponent<XKPlayerTiaoBanCtrl>();
		PlayerTiaoBanScript.PlayerMoveScript = this;
		TiaoBanObjAy[0].name = "TiaoBan0";
		TiaoBanObjAy[1].name = "TiaoBan1";
		TiaoBanObjAy[1].transform.parent = TiaoBanObjAy[0].transform;
		TiaoBanObjAy[0].transform.parent = XkGameCtrl.MissionCleanup;
		XKPlayerDongGanCtrl dongGanScript = gameObject.AddComponent<XKPlayerDongGanCtrl>();
		if (dongGanScript != null) {
			dongGanScript.SetPlayerIndex(PlayerIndex);
		}

		PlayerXueTiaoCtrl playerXueTiaoScript = GetComponentInChildren<PlayerXueTiaoCtrl>();
		if (playerXueTiaoScript != null) {
			playerXueTiaoScript.SetPlayerIndex(PlayerIndex);
		}

		DeathExplodPrefab = XKPlayerGlobalDt.GetInstance().DeathExplodPrefab;
		if (GenZongDanAimPoint == null) {
			Debug.LogWarning("Unity:"+"GenZongDanAimPoint is null");
			GenZongDanAimPoint.name = "null";
			return;
		}
		FireScript = GetComponent<XKPlayerAutoFire>();
		XKPlayerGlobalDt.AddPlayerMoveList(this);
		AddPlayerFanWeiList();
		InitPlayerMoveInfo();
		InitUpdatePlayerRotation();
		ResetIsWuDiState();
		RigCom = rigidbody;
		PlayerTran = transform;
		GameCameraTran = XKPlayerCamera.GetInstanceFeiJi().transform;
		TerrainLayer = XkGameCtrl.GetInstance().LandLayer;

		bool isActivePlayer = true;
		switch (PlayerIndex) {
		case PlayerEnum.PlayerOne:
			if (!XkGameCtrl.IsActivePlayerOne) {
				isActivePlayer = false;
			}
			_InstancePOne = this;
			break;
			
		case PlayerEnum.PlayerTwo:
			if (!XkGameCtrl.IsActivePlayerTwo) {
				isActivePlayer = false;
			}
			_InstancePTwo = this;
			break;
			
		case PlayerEnum.PlayerThree:
			if (!XkGameCtrl.IsActivePlayerThree) {
				isActivePlayer = false;
			}
			_InstancePThree = this;
			break;
			
		case PlayerEnum.PlayerFour:
			if (!XkGameCtrl.IsActivePlayerFour) {
				isActivePlayer = false;
			}
			_InstancePFour = this;
			break;
		}

		if (!isActivePlayer) {
			gameObject.SetActive(false);
		}

        if (m_PlayerAiMove == null)
        {
            m_PlayerAiMove = gameObject.AddComponent<XKPlayerAiMove>();
            m_PlayerAiMove.Init(this);
        }
	}

	void FixedUpdate()
	{
		if (!IsMoveToTiaoYueDian) {
			switch (TKMoveSt) {
			case TKMoveState.U_FangXiangPan:
				CheckPlayerFangXiang();
				CheckPlayerYouMen();
				break;
			case TKMoveState.YaoGanBan:
				CheckPlayerYaoGanInput();
				break;
			}
			return;
		}
		//PlayerTran.position = TiaoBanObjAy[1].transform.position;
		PlayerTran.position = Vector3.Lerp(PlayerTran.position,
		                                   TiaoBanObjAy[1].transform.position,
		                                   Time.deltaTime * 20f);
	}

	// Update is called once per frame
	void Update()
	{
		if (IsMoveToTiaoYueDian) {
			//PlayerTran.position = TiaoBanObjAy[1].transform.position;
			return;
		}

		if (!XkGameCtrl.GetInstance().IsCartoonShootTest) {
			CheckPlayerIsResetPosition();
		}
		UpdateJiSuTuoWeiTX();
		switch (TKMoveSt) {
		case TKMoveState.U_FangXiangPan:
			CheckPlayerFangXiang();
			CheckPlayerYouMen();
			break;
		case TKMoveState.YaoGanBan:
			CheckPlayerYaoGanInput();
			break;
		}
		MakePlayerToLand();
		CheckCheTiPointArray();

        CheckPlayerToMainCameraDistance();
    }

	void OnCollisionEnter(Collision collision)
	{
		//Debug.Log("Unity:"+"OnCollisionEnter -> colName "+collision.gameObject.name);
		CheckPlayerIsOpenCheckPaoWuXianTerrain(collision.gameObject);
		if (IsMoveToTiaoYueDian) {
//			if (playerFanWeiScript != null) {
//				OnHitMoveFanWeiByPaoWuXianRun();
//				MovePlayerOverPaoWuXianByITween(1);
//			}
			return;
		}

        XKPlayerMvFanWei playerFanWeiScript = collision.transform.GetComponentInParent<XKPlayerMvFanWei>();
        if (playerFanWeiScript != null) {
			if (playerFanWeiScript.FanWeiState == PointState.Hou) {
				//Debug.LogError("Unity:"+"OnCollisionEnter -> FanWeiState "+playerFanWeiScript.FanWeiState);
				RigCom.drag = 10f;
				IsAutoMoving = true;
                IsHitMvFanWeiHou = true;
                m_LastHitFanWeiHouTime = Time.time;
            }
		}

		XKPlayerMoveCtrl playerMvScript = collision.transform.GetComponentInParent<XKPlayerMoveCtrl>();
		if (playerMvScript != null) {
			CheckHitPlayerTanKe(playerMvScript);
		}
	}
	
	void OnCollisionExit(Collision collision)
	{
		if (IsMoveToTiaoYueDian) {
			return;
		}

		XKPlayerMvFanWei playerFanWeiScript = collision.transform.GetComponentInParent<XKPlayerMvFanWei>();
		if (playerFanWeiScript != null) {
			if (playerFanWeiScript.FanWeiState == PointState.Hou) {
				//Debug.LogError("Unity:"+"OnCollisionExit -> FanWeiState "+playerFanWeiScript.FanWeiState);
				RigCom.drag = 100f;
				IsAutoMoving = false;
                IsHitMvFanWeiHou = false;
            }
		}
	}

	enum TK_Angle_State
	{
		AngleNag_135,
		AngleNag_90,		//负90.
		AngleNag_45,
		Angle_0,
		Angle_45,
		Angle_90,			//正90.
		Angle_135,
		Angle_180,
	}
	TK_Angle_State TKAngleSt = TK_Angle_State.Angle_0;
	void CheckPlayerYaoGanInput()
	{

        if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == true)
        {
            //没有玩家激活游戏.
        }
        else
        {
            if (!XkGameCtrl.GetIsActivePlayer(PlayerIndex))
            {
                PlayerRotStateYG = 1;
                return;
            }
        }

		int indexVal = (int)PlayerIndex - 1;
		float fxVal = InputEventCtrl.PlayerFX[indexVal];
		float ymVal = InputEventCtrl.PlayerYM[indexVal];
		if (XKGlobalData.GameVersionPlayer != 0) {
			fxVal = InputEventCtrl.PlayerFX[indexVal - 2];
			ymVal = InputEventCtrl.PlayerYM[indexVal - 2];
		}
		if (fxVal == 0f && ymVal == 0f) {
			if (LvDaiUVCom != null && LvDaiUVCom.enabled) {
				LvDaiUVCom.enabled = false;
			}
			PlayerRotStateYG = 1;
			return;
		}

		if (LvDaiUVCom != null && !LvDaiUVCom.enabled) {
			LvDaiUVCom.enabled = true;
		}

		if (Mathf.Sign(fxVal) != Mathf.Sign(PlayerFangXiangVal)) {
			PlayerRotStateYG = 1;
		}
		PlayerFangXiangVal = fxVal;
		PlayerRealYM = ymVal;

		float curAngle = 0f;
		int curAngleSt = 0;
		if (ymVal == 0f) {
			curAngle = fxVal < 0f ? -90f : 90f;
			curAngleSt = fxVal < 0f ? -90 : 90;
			ymVal = 1f;
		}
		else {
			if (ymVal > 0f) {
				ymVal = 1f;
				if (fxVal != 0f) {
					curAngle = fxVal < 0f ? -45f : 45f;
					curAngleSt = fxVal < 0f ? -45 : 45;
				}
			}
			else {
				ymVal = -1f;
				if (fxVal != 0f) {
//					curAngle = fxVal < 0f ? 45f : -45f;
//					curAngleSt = fxVal < 0f ? 45 : -45;
					curAngle = fxVal < 0f ? -135f : 135f;
					curAngleSt = fxVal < 0f ? -135 : 135;
				}
				else {
					curAngle = 180f;
					curAngleSt = 180;
				}
			}
		}

		bool isChangeTKCore = false;
		switch (curAngleSt) {
		case -135:
			if (TKAngleSt != TK_Angle_State.AngleNag_135) {
				TKAngleSt = TK_Angle_State.AngleNag_135;
				isChangeTKCore = true;
			}
			break;
		case -90:
			if (TKAngleSt != TK_Angle_State.AngleNag_90) {
				TKAngleSt = TK_Angle_State.AngleNag_90;
				isChangeTKCore = true;
			}
			break;
		case -45:
			if (TKAngleSt != TK_Angle_State.AngleNag_45) {
				TKAngleSt = TK_Angle_State.AngleNag_45;
				isChangeTKCore = true;
			}
			break;
		case 0:
			if (TKAngleSt != TK_Angle_State.Angle_0) {
				TKAngleSt = TK_Angle_State.Angle_0;
				isChangeTKCore = true;
			}
			break;
		case 45:
			if (TKAngleSt != TK_Angle_State.Angle_45) {
				TKAngleSt = TK_Angle_State.Angle_45;
				isChangeTKCore = true;
			}
			break;
		case 90:
			if (TKAngleSt != TK_Angle_State.Angle_90) {
				TKAngleSt = TK_Angle_State.Angle_90;
				isChangeTKCore = true;
			}
			break;
		case 135:
			if (TKAngleSt != TK_Angle_State.Angle_135) {
				TKAngleSt = TK_Angle_State.Angle_135;
				isChangeTKCore = true;
			}
			break;
		case 180:
			if (TKAngleSt != TK_Angle_State.Angle_180) {
				TKAngleSt = TK_Angle_State.Angle_180;
				isChangeTKCore = true;
			}
			break;
		}

//		if (!IsPlayerRotYG) {
//			ChangePlayerMoving(ymVal);
//		}
//		else {
////			Vector3 euLE = PlayerCore.transform.localEulerAngles;
////			euLE.y = euLE.y <= 180f ? euLE.y : (euLE.y - 360f);
//			if (PlayerRotStateYG == 0) {
//				ChangePlayerMoving(ymVal);
//			}
//		}

		#if USE_LINE_CHANGE_DIR
		if (isChangeTKCore) {
			PlayerCore.transform.parent = null;
			PlayerRotStateYG = 1;
		}

		Vector3 veA = GameCameraTran.forward;
		Vector3 veB = Vector3.forward;
		veA.y = veB.y = 0f;
		float angleTmp = Vector3.Angle(veA, veB);
		if (Vector3.Dot(veA, Vector3.right) < 0f) {
			angleTmp = -angleTmp;
		}
		curAngle = curAngle + angleTmp;
		
		Quaternion rotVal = Quaternion.AngleAxis(curAngle, Vector3.up);
		Vector3 euA = rotVal.eulerAngles;
		Vector3 euB = PlayerTran.rotation.eulerAngles;
		euA.x = euB.x;
		euA.z = euB.z;
		rotVal.eulerAngles = euA;
		PlayerTran.rotation = rotVal;
		if (isChangeTKCore) {
			PlayerCore.transform.parent = PlayerTran;
			PlayerCore.transform.localPosition = Vector3.zero;
			euA = PlayerCore.transform.localEulerAngles;
			euA.x = euA.z = 0f;
			PlayerCore.transform.localEulerAngles = euA;
			PlayerCore.transform.localScale = Vector3.one;
			
			//PlayerRotStateYG = 1;

			euA = PlayerCore.transform.localEulerAngles;
			euA.y = euA.y <= 180f ? euA.y : (euA.y - 360f);
			//float speedAngleVal = Time.deltaTime * XKPlayerGlobalDt.GetInstance().DirSpeedYG;
			if (PlayerRotStateYG != 0) {
				euA.y = euA.y >= 0f ? 1f : 359f;
				PlayerCore.transform.localEulerAngles = euA;
				/*Debug.Log("Unity:"+"testAngleY "+PlayerCore.transform.localEulerAngles.y.ToString("f2")
				          +", euA.y "+euA.y.ToString("f2"));*/
			}
			PlayerRotStateYG = 0;
		}

		if (!IsPlayerRotYG) {
			ChangePlayerMoving(ymVal);
		}
		else {
//			Vector3 euLE = PlayerCore.transform.localEulerAngles;
//			euLE.y = euLE.y <= 180f ? euLE.y : (euLE.y - 360f);
//			if (PlayerRotStateYG == 0) {
//				ChangePlayerMoving(ymVal);
//			}
			ChangePlayerMoving(ymVal);
		}

//		else {
//			euA = PlayerCore.transform.localEulerAngles;
//			euA.y = euA.y <= 180f ? euA.y : (euA.y - 360f);
//			//float speedAngleVal = Time.deltaTime * XKPlayerGlobalDt.GetInstance().DirSpeedYG;
//			if (PlayerRotStateYG != 0) {
//				euA.y = euA.y >= 0f ? 1f : 359f;
//				PlayerCore.transform.localEulerAngles = euA;
//				/*Debug.Log("Unity:"+"testAngleY "+PlayerCore.transform.localEulerAngles.y.ToString("f2")
//				          +", euA.y "+euA.y.ToString("f2"));*/
//			}
//			PlayerRotStateYG = 0;



//			if (Mathf.Abs(euA.y) >= speedAngleVal && false) {
//				euA.x = euA.z = 0f;
//				float beiLv = euA.y > 0f ? -1f : 1f;
//				euA.y += beiLv * speedAngleVal;
//				PlayerCore.transform.localEulerAngles = euA;
//				PlayerRotStateYG = 1;
//
//				if (Mathf.Abs(euA.y) < speedAngleVal) {
//					euA.y = euA.y >= 0f ? 1f : 359f;
//					PlayerCore.transform.localEulerAngles = euA;
//					/*Debug.Log("Unity:"+"testAngleY "+PlayerCore.transform.localEulerAngles.y.ToString("f2")
//					          +", euA.y "+euA.y.ToString("f2"));*/
//				}
//			}
//			else {
//				if (PlayerRotStateYG != 0) {
//					euA.y = euA.y >= 0f ? 1f : 359f;
//					PlayerCore.transform.localEulerAngles = euA;
//					/*Debug.Log("Unity:"+"testAngleY "+PlayerCore.transform.localEulerAngles.y.ToString("f2")
//				          +", euA.y "+euA.y.ToString("f2"));*/
//				}
//				PlayerRotStateYG = 0;
//			}
//		}
		#else
		Vector3 veA = GameCameraTran.forward;
		Vector3 veB = Vector3.forward;
		veA.y = veB.y = 0f;
		float angleTmp = Vector3.Angle(veA, veB);
		if (Vector3.Dot(veA, Vector3.right) < 0f) {
			angleTmp = -angleTmp;
		}
		curAngle = curAngle + angleTmp;
		
		Quaternion rotVal = Quaternion.AngleAxis(curAngle, Vector3.up);
		Vector3 euA = rotVal.eulerAngles;
		Vector3 euB = PlayerTran.rotation.eulerAngles;
		euA.x = euB.x;
		euA.z = euB.z;
		rotVal.eulerAngles = euA;
		PlayerTran.rotation = Quaternion.Lerp(PlayerTran.rotation, rotVal, Time.deltaTime * XKPlayerGlobalDt.GetInstance().DirSpeedYG);
		#endif
	}


	void CheckPlayerFangXiang()
	{
		if (!XkGameCtrl.GetIsActivePlayer(PlayerIndex)) {
			return;
		}
		int indexVal = (int)PlayerIndex - 1;
		ChangePlayerFangXiang(InputEventCtrl.PlayerFX[indexVal]);
	}

	/**
	 * IsLineFangXiang == true -> 使玩家运动方向线性增加(u型方向盘).
	 * IsLineFangXiang == false -> 使玩家运动方向非线性增加(u型方向盘).
	 */
	public bool IsLineFangXiang;
	/**
	 * IsPlayerRotYG == true -> 主角转向优先(摇杆版).
	 * IsPlayerRotYG == false -> 主角转向和位移同时进行(摇杆版).
	 */
	public bool IsPlayerRotYG;
	/**
	 * PlayerRotStateYG == 0 -> 主角转向完成.
	 * PlayerRotStateYG == 1 -> 主角正在转向.
	 */
	int PlayerRotStateYG = 0;
	float MaxFangXiangAngle = 45f;
	float MinFangXiangAngle = 45f;
	float KeyFangXiang = 0f;
	void InitPlayerMoveInfo()
	{
		switch (TKMoveSt) {
		case TKMoveState.U_FangXiangPan:
			SetMaxMoveSpeed(XKPlayerGlobalDt.GetInstance().MaxMvSpeed);
			break;
		case TKMoveState.YaoGanBan:
			SetMaxMoveSpeed(XKPlayerGlobalDt.GetInstance().MaxMvSpeedYG,
			                XKPlayerGlobalDt.GetInstance().HorizontalSpeedYGBL);
			break;
		}
		MaxHitPower = XKPlayerGlobalDt.GetInstance().MaxHitPower;
		MaxFangXiangAngle = XKPlayerGlobalDt.GetInstance().MaxFangXiangAngle;
		
		if (JiSuTuoWeiTX != null) {
			JiSuTuoWeiTX.SetActive(false);
		}
	}

	bool IsActiveJiSuSt;
//	float TimeJiSuStart;
	public GameObject JiSuTuoWeiTX;
	void UpdateJiSuTuoWeiTX()
	{
		if (XKTriggerYuLeCtrl.IsActiveYuLeTrigger) {
			if (PlayerYouMenVal <= 0f
			    || PlayerMvSpeedCur < 1f) {
				if (JiSuTuoWeiTX.activeSelf) {
					JiSuTuoWeiTX.SetActive(false);
				}
				return;
			}
			
			if (JiSuTuoWeiTX.activeSelf) {
				return;
			}
			JiSuTuoWeiTX.SetActive(true);
			return;
		}

		if (PlayerYouMenVal <= 0f
		    || PlayerMvSpeedCur < 1f
		    || !IsActiveJiSuSt) {
			if (JiSuTuoWeiTX.activeSelf) {
				JiSuTuoWeiTX.SetActive(false);
			}
			return;
		}

		if (JiSuTuoWeiTX.activeSelf) {
			return;
		}
		JiSuTuoWeiTX.SetActive(true);
	}

	public static void SetPlayerJiSuState(PlayerEnum playerSt)
	{
		XKPlayerMoveCtrl playerScript = GetXKPlayerMoveCom(playerSt);
		if (playerScript == null) {
			return;
		}
		playerScript.ActiveJiSuState();
	}

	public void ResetPlayerJiSuState()
	{
		//UpdateJiSuTuoWeiTX();
		if (!IsActiveJiSuSt) {
			return;
		}
		
		IsActiveJiSuSt = false;
		switch (TKMoveSt) {
		case TKMoveState.U_FangXiangPan:
			SetMaxMoveSpeed(XKPlayerGlobalDt.GetInstance().MaxMvSpeed);
			break;
		case TKMoveState.YaoGanBan:
			SetMaxMoveSpeed(XKPlayerGlobalDt.GetInstance().MaxMvSpeedYG,
			                XKPlayerGlobalDt.GetInstance().HorizontalSpeedYGBL);
			break;
		}
		DaoJuCtrl.GetInstance().HiddenPlayerDaoJuObj(PlayerIndex, BuJiBaoType.JiSuDJ);
		/*if (Time.realtimeSinceStartup - TimeJiSuStart > XKDaoJuGlobalDt.GetInstance().JiSuTimeVal) {
			IsActiveJiSuSt = false;
			SetMaxMoveSpeed(XKPlayerGlobalDt.GetInstance().MaxMvSpeed);
			DaoJuCtrl.GetInstance().HiddenPlayerDaoJuObj(PlayerIndex, BuJiBaoType.JiSuDJ);
		}*/
	}

	void ActiveJiSuState()
	{
		IsActiveJiSuSt = true;
//		TimeJiSuStart = Time.realtimeSinceStartup;
	}

	static void SetJiSuMoveInfo(PlayerEnum playerSt)
	{
		XKPlayerMoveCtrl moveScript = null;
		TKMoveState tkMvState = TKMoveState.U_FangXiangPan;
		moveScript = GetXKPlayerMoveCom(playerSt);
		if (moveScript == null) {
			return;
		}
		
		tkMvState = moveScript.GetTKMoveState();
		switch (tkMvState) {
		case TKMoveState.U_FangXiangPan:
			XKPlayerMoveCtrl.SetPlayerMaxMoveSpeed(playerSt, XKDaoJuGlobalDt.GetInstance().JiSuSpeedVal);
			break;
		case TKMoveState.YaoGanBan:
			XKPlayerMoveCtrl.SetPlayerMaxMoveSpeed(playerSt, XKDaoJuGlobalDt.GetInstance().JiSuSpeedYGVal,
			                                       XKDaoJuGlobalDt.GetInstance().HorizontalSpeedYGBL);
			break;
		}
	}

	public static void SetPlayerJiSuMoveSpeed(PlayerEnum playerSt)
	{
		if (playerSt == PlayerEnum.Null) {
			PlayerEnum indexPlayer = PlayerEnum.Null;
			for (int i = 1; i <= 4; i++) {
				indexPlayer = (PlayerEnum)i;
				SetJiSuMoveInfo(indexPlayer);
			}
			return;
		}
		SetJiSuMoveInfo(playerSt);
	}

	public static void SetPlayerYuLeInfo(XKTriggerYuLeCtrl yuLeScript)
	{
		XKPlayerMoveCtrl playerScript = null;
		PlayerEnum indexPlayer = PlayerEnum.Null;
		TKMoveState tkMvState = TKMoveState.U_FangXiangPan;
		for (int i = 1; i <= 4; i++) {
			indexPlayer = (PlayerEnum)i;
			playerScript = GetXKPlayerMoveCom(indexPlayer);
			if (playerScript == null) {
				continue;
			}

			tkMvState = playerScript.GetTKMoveState();
			switch (tkMvState) {
			case TKMoveState.U_FangXiangPan:
				XKPlayerMoveCtrl.SetPlayerMaxMoveSpeed(indexPlayer, yuLeScript.YuLeSpeedVal);
				break;
			case TKMoveState.YaoGanBan:
				XKPlayerMoveCtrl.SetPlayerMaxMoveSpeed(indexPlayer, yuLeScript.YuLeSpeedYGVal,
				                                       yuLeScript.HorizontalSpeedYGBL);
				break;
			}
		}
	}

	static void SetPlayerMaxMoveSpeed(PlayerEnum playerSt, float speedVal, float horSpeedBL = 1f)
	{
		XKPlayerMoveCtrl playerScript = null;
		if (playerSt == PlayerEnum.Null) {
			PlayerEnum indexPlayer = PlayerEnum.Null;
			for (int i = 1; i <= 4; i++) {
				indexPlayer = (PlayerEnum)i;
				playerScript = GetXKPlayerMoveCom(indexPlayer);
				if (playerScript == null) {
					continue;
				}
				playerScript.SetMaxMoveSpeed(speedVal, horSpeedBL);
			}
		}
		else {
			playerScript = GetXKPlayerMoveCom(playerSt);
			if (playerScript == null) {
				return;
			}
			playerScript.SetMaxMoveSpeed(speedVal, horSpeedBL);
		}
	}

	void SetMaxMoveSpeed(float speedVal, float horSpeedBL = 1f)
	{
		MaxMvSpeed = speedVal;
		PlayerMvSpeed = MaxMvSpeed / 3.6f;
		//HorizontalSpeedBL = horSpeedBL;
	}

	void InitUpdatePlayerRotation()
	{
		PlayerTran = transform;
		MinFangXiangAngle = -MaxFangXiangAngle;
		KeyFangXiang = (MaxFangXiangAngle - MinFangXiangAngle) / 2f;
	}

	/**
	 * 使主角运动方向线性增加,响应外设.
	 */
	void UpdatePlayerRotation(float curFangXiang)
	{
		//float curFangXiang = Input.GetAxis("Horizontal");
		float curAngle = MaxFangXiangAngle - KeyFangXiang * (1f - curFangXiang);
		
		Vector3 veA = GameCameraTran.forward;
		Vector3 veB = Vector3.forward;
		veA.y = veB.y = 0f;
		float angleTmp = Vector3.Angle(veA, veB);
		if (Vector3.Dot(veA, Vector3.right) < 0f) {
			angleTmp = -angleTmp;
		}
		curAngle = curAngle + angleTmp;
		//TestAngle = curAngle;
		
		Quaternion rotVal = Quaternion.AngleAxis(curAngle, Vector3.up);
		Vector3 euA = rotVal.eulerAngles;
		Vector3 euB = PlayerTran.rotation.eulerAngles;
		euA.x = euB.x;
		euA.z = euB.z;
		rotVal.eulerAngles = euA;
		PlayerTran.rotation = Quaternion.Lerp(PlayerTran.rotation, rotVal, Time.deltaTime * 4f);
	}

	/**
	 * key == 0 -> 游戏正常使用的主角转向逻辑.
	 * key == 1 -> 游戏拍摄动画时主角的转向逻辑.
	 */
	void SetPlayerFangXiang(float fxVal, int key = 0)
	{
		if (fxVal == 0f) {
			return;
		}
		
		float cosAB = 0f;
		switch (key) {
		case 0:
			Vector3 vecA = GameCameraTran.forward;
			Vector3 vecB = PlayerTran.forward;
			vecA.y = vecB.y = 0f;
			cosAB = Vector3.Dot(vecA, vecB);
			if (cosAB < 0f) {
				if ((LastFXVal > 0f && fxVal > 0f)
				    ||(LastFXVal < 0f && fxVal < 0f)) {
					return;
				}
			}
			break;
		}
		
		VecDirSpeed.y = XKPlayerGlobalDt.GetInstance().DirSpeed * Time.deltaTime;
		if (fxVal < 0f) {
			if (cosAB > 0f) {
				LastFXVal = -1f;
			}
			VecDirSpeed.y = -VecDirSpeed.y;
		}
		else {
			if (cosAB > 0f) {
				LastFXVal = 1f;
			}
		}
		PlayerTran.Rotate(VecDirSpeed);
	}

	void ChangePlayerFangXiang(float fxVal)
	{
		if (XkGameCtrl.GetInstance().IsCartoonShootTest) {
			SetPlayerFangXiang(fxVal, 1);
			return;
		}

		if (IsLineFangXiang) {
			UpdatePlayerRotation(fxVal);
		}
		else {
			SetPlayerFangXiang(fxVal);
		}
	}

	void CheckPlayerYouMen()
	{
		if (!XkGameCtrl.GetIsActivePlayer(PlayerIndex)) {
			return;
		}
		float youMenVal = 0f;
		int indexVal = (int)PlayerIndex - 1;
		
		if (FireScript == null || !FireScript.IsLockPaoTa || InputEventCtrl.PlayerYM[indexVal] < 0f) {
			youMenVal = InputEventCtrl.PlayerYM[indexVal];
		}
		else {
			Vector3 veA = GameCameraTran.forward;
			Vector3 veB = PlayerTran.forward;
			veA.y = veB.y = 0f;
			float angleVal = Mathf.Clamp(Vector3.Angle(veA, veB), 0f, 90f);
			if (angleVal >= 70f) {
				youMenVal = InputEventCtrl.PlayerFX[indexVal] != 0f ? 1f : InputEventCtrl.PlayerYM[indexVal];
			}
			else {
				youMenVal = InputEventCtrl.PlayerFX[indexVal] != 0f ? 0f : InputEventCtrl.PlayerYM[indexVal];
			}
		}
		ChangePlayerMoving(youMenVal);
	}

	void UpdatePlayerHitCameraBack()
	{
		//if (XKTriggerStopMovePlayer.IsActiveTrigger) {
		//	return;
		//}

		if (PlayerRealYM < 0f && PlayerFangXiangVal == 0f) {
			return;
		}
		float camSpeed = XkPlayerCtrl.GetInstanceFeiJi().GetPlayerMoveSpeed();
		float mvDis = camSpeed * Time.deltaTime;
		Vector3 camForward = GameCameraTran.forward;
		camForward.y = 0f;
		PlayerTran.position += mvDis * camForward;
	}

	float PlayerFangXiangVal;
	float PlayerYouMenVal;
	float PlayerRealYM;
	float PlayerMvSpeedCur;
	void ChangePlayerMoving(float mvVal)
    {
        //SSDebug.Log("PlayerFangXiangVal == " + PlayerFangXiangVal
        //        + ", PlayerRealYM == " + PlayerRealYM
        //        + ", PlayerFangXiangVal == " + PlayerFangXiangVal
        //        + ", TKMoveSt == " + TKMoveSt
        //        + ", mvVal == " + mvVal);
        PlayerYouMenVal = mvVal;
		PlayerMvSpeedCur = RigCom.velocity.magnitude;
		//if (PlayerPointForward.GetIsOutGameCamera()) {
		//	StopMovePlayer();
		//	return;
		//}
        //SSDebug.Log("1111111111111111111111111111111111");

        if (mvVal == 0f) {
			StopMovePlayer();
//			if (!IsInvoking("StopMovePlayer")) {
//				Invoke("StopMovePlayer", 0f);
//			}
			return;
        }
        //SSDebug.Log("22222222222222222222222222222");

        if (IsHitMvFanWeiHou) {
			if (PlayerFangXiangVal == 0f && PlayerRealYM < 0f) {
				return;
			}
        }
        //SSDebug.Log("333333333333333333333333333333333");

        if (RigCom.drag != 10f) {
			RigCom.drag = 10f;
		}

        float mvSpeedVal = PlayerMvSpeed;
		Vector3 moveDir = Vector3.zero;
		switch (TKMoveSt) {
		case TKMoveState.U_FangXiangPan:
			moveDir = PlayerTran.forward;
			break;
		case TKMoveState.YaoGanBan:
			#if USE_LINE_CHANGE_DIR
			if (!IsHitMvFanWeiHou) {
				moveDir = PlayerTran.forward;
			}
			else {
                //SSDebug.LogWarning("PlayerFangXiangVal == " + PlayerFangXiangVal
                //        + ", PlayerRealYM == " + PlayerRealYM + ", PlayerFangXiangVal == " + PlayerFangXiangVal);
				if (PlayerFangXiangVal == 0f || PlayerRealYM > 0f) {
					moveDir = PlayerTran.forward;
				}
				else {
					//float dirState = mvVal < 0f ? 1f : -1f;
					//float dirState = 1f;
					if (PlayerRealYM <= 0f) {
						/*if (PlayerFangXiangVal > 0f) {
							moveDir = (XKPlayerMvFanWei.GetInstanceHou().transform.right * dirState);
						}
						
						if (PlayerFangXiangVal < 0f) {
							moveDir = -(XKPlayerMvFanWei.GetInstanceHou().transform.right * dirState);
						}*/
						
						if (PlayerFangXiangVal > 0f) {
                            //SSDebug.LogWarning("************************111");
							moveDir = -(XKPlayerMvFanWei.GetInstanceHou().transform.right);
						}
						
						if (PlayerFangXiangVal < 0f)
                        {
                            //SSDebug.LogWarning("************************222");
                            moveDir = (XKPlayerMvFanWei.GetInstanceHou().transform.right);
						}
						/*if (PlayerRealYM < 0f) {
							if (PlayerFangXiangVal > 0f) {
								moveDir = -(XKPlayerMvFanWei.GetInstanceHou().transform.right * dirState);
							}
							
							if (PlayerFangXiangVal < 0f) {
								moveDir = (XKPlayerMvFanWei.GetInstanceHou().transform.right * dirState);
							}
						}
						else {
							if (PlayerFangXiangVal > 0f) {
								moveDir = (XKPlayerMvFanWei.GetInstanceHou().transform.right * dirState);
							}
							
							if (PlayerFangXiangVal < 0f) {
								moveDir = -(XKPlayerMvFanWei.GetInstanceHou().transform.right * dirState);
							}
						}*/
					}
				}
				UpdatePlayerHitCameraBack();
			}
			#else
			if (!IsHitMvFanWeiHou) {
				//moveDir = PlayerTran.forward;
				if (PlayerFangXiangVal == 0f) {
					moveDir = GameCameraTran.forward;
				}
				else {
					if (PlayerRealYM == 0f) {
						moveDir = PlayerFangXiangVal > 0f ? GameCameraTran.right : -GameCameraTran.right;
					}
					else {
						float dirState = PlayerRealYM > 0f ? 1f : -1f;
						if (PlayerFangXiangVal > 0f) {
							moveDir = Vector3.Lerp(GameCameraTran.forward, GameCameraTran.right*dirState, 0.5f); 
						}
						else {
							moveDir = Vector3.Lerp(GameCameraTran.forward, -(GameCameraTran.right*dirState), 0.5f); 
						}
					}
				}
			}
			else {
				if (PlayerFangXiangVal == 0f || PlayerRealYM > 0f) {
					//moveDir = PlayerTran.forward;
					moveDir = GameCameraTran.forward;
					if (PlayerRealYM > 0f) {
						if (PlayerFangXiangVal != 0f) {
							if (PlayerFangXiangVal > 0f) {
								moveDir = Vector3.Lerp(GameCameraTran.forward, GameCameraTran.right, 0.5f); 
							}
							else {
								moveDir = Vector3.Lerp(GameCameraTran.forward, -(GameCameraTran.right), 0.5f); 
							}
						}
					}
				}
				else {
					float dirState = mvVal < 0f ? 1f : -1f;
					if (PlayerRealYM <= 0f) {
						if (PlayerFangXiangVal > 0f) {
							moveDir = (XKPlayerMvFanWei.GetInstanceHou().transform.right * dirState);
						}
						
						if (PlayerFangXiangVal < 0f) {
							moveDir = -(XKPlayerMvFanWei.GetInstanceHou().transform.right * dirState);
						}
					}
				}
			}
			moveDir.y = 0f;

			if (PlayerRealYM == 0f && PlayerFangXiangVal != 0f) {
				mvSpeedVal *= HorizontalSpeedBL;
			}
			#endif
			break;
		}
		
		RigCom.velocity = moveDir * mvSpeedVal;
		/*if (mvVal > 0f) {
			RigCom.velocity = moveDir * mvSpeedVal;
		}

		if (mvVal < 0f) {
			RigCom.velocity = -moveDir * mvSpeedVal;
		}*/
	}

	float HighOffset = 0f;
	void MakePlayerToLand()
	{
		RaycastHit hitInfo;
		Vector3 startPos = PlayerTran.position + Vector3.up * 50f;
		Vector3 forwardVal = Vector3.down;
		if (Physics.Raycast(startPos, forwardVal, out hitInfo, 85f, TerrainLayer.value)){
			//if (Vector3.Distance(PlayerTran.position, hitInfo.point) > 1f) {
				Vector3 posTmp = hitInfo.point + Vector3.up * HighOffset;
				PlayerTran.position = Vector3.Lerp(PlayerTran.position, posTmp, 0.1f);
			//}
		}
	}

	void StopMovePlayer()
	{
		if (IsAutoMoving) {
			return;
		}

		if (RigCom.drag == 100f) {
			return;
		}
		RigCom.drag = 100f;
	}

	void CheckCheTiPointArray()
	{
		Vector3 posA = CheTiPointArray[0].position;
		Vector3 posB = CheTiPointArray[1].position;
		
		RaycastHit hitInfo;
		float disVal = 15f;
		Vector3 forwardVal = Vector3.down;
		Vector3 startPos = CheTiPointArray[0].position + (Vector3.up * 5f);
		Physics.Raycast(startPos, forwardVal, out hitInfo, disVal, TerrainLayer.value);
		if (hitInfo.collider != null){
			posA = hitInfo.point;
		}

		startPos = CheTiPointArray[1].position + (Vector3.up * 5f);
		Physics.Raycast(startPos, forwardVal, out hitInfo, disVal, TerrainLayer.value);
		if (hitInfo.collider != null){
			posB = hitInfo.point;
		}

		forwardVal = posA - posB;
		PlayerTran.forward = Vector3.Lerp(PlayerTran.forward, forwardVal, 1f);
	}

	public void PushPlayerTanKe(Vector3 pushDir, float pushPower = 0f)
	{
		if (pushPower <= 0) {
			return;
		}

		RigCom.drag = 10;
		if (pushPower > 0f) {
			RigCom.AddForce(pushDir * pushPower, ForceMode.Impulse);
		}
		else {
			RigCom.AddForce(pushDir * MaxHitPower, ForceMode.Impulse);
		}
	}

	void CheckHitPlayerTanKe(XKPlayerMoveCtrl playerScript)
	{
		if (playerScript == null) {
			return;
		}

		Transform tanKeTran = playerScript.transform;
		Vector3 vecA = PlayerTran.forward;
		Vector3 vecB = tanKeTran.forward;
		Vector3 vecC = tanKeTran.position - PlayerTran.position;
		float dotAB = Vector3.Dot(vecA, vecB);
		float dotAC = Vector3.Dot(vecA, vecC);
		if (dotAB >= 0f && dotAC < 0f) {
			PushPlayerTanKe(PlayerTran.forward);
		}

		if (dotAB <= 0f && dotAC > 0f) {
			PushPlayerTanKe(-PlayerTran.forward);
			playerScript.PushPlayerTanKe(-tanKeTran.forward);
		}
	}

	public void ActivePlayerToPos(Vector3 pos, Vector3 forwordVal, bool isChangePos = false)
	{
		if (gameObject.activeSelf && !isChangePos) {
			return;
		}
		PlayerTran.position = pos;
		PlayerTran.forward = forwordVal;

        //关闭火焰特效.
        SetActiveHuoYanDamageTX(false);

        if (isChangePos) {
			IsDeathPlayer = true;
		}

		if (IsDeathPlayer) {
			ActivePlayerWuDiState();
		}
		IsDeathPlayer = false;
		gameObject.SetActive(true);
	}

	public void HiddenGamePlayer(int key = 0)
	{
		//Debug.Log("Unity:"+"HiddenGamePlayer -> key "+key);
		if (!gameObject.activeSelf)
        {
			return;
		}
		gameObject.SetActive(false);
		IsDeathPlayer = true;
        
        if (FireScript)
        {
            FireScript.ResetInfo();
        }

		ResetIsWuDiState();
		if (DeathExplodPrefab != null && key == 0)
        {
			GameObject objExplode = null;
			objExplode = (GameObject)Instantiate(DeathExplodPrefab, PlayerTran.position, PlayerTran.rotation);
			objExplode.transform.parent = XkGameCtrl.PlayerAmmoArray;
		}
	}

	public bool GetIsDeathPlayer()
	{
		return IsDeathPlayer;
	}

	public bool GetIsWuDiState()
	{
		return IsWuDiState;
	}

	public void ActivePlayerWuDiState()
	{
        bool isActiveWuDiTxObj = false;
        if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == true)
        {
            //游戏处于循环动画阶段,没有激活的玩家.
            //SetActiveNewCircleTeXiao(false);
        }
        else
        {
            //游戏处于玩家战斗阶段,有激活的玩家.
            if (!XkGameCtrl.GetIsActivePlayer(PlayerIndex))
            {
                return;
            }

            if (HuDunCtrl.GetInstance(PlayerIndex) != null)
            {
                HuDunCtrl.GetInstance(PlayerIndex).ShowHuDunUI(XkGameCtrl.GetInstance().WuDiTime);
            }
            //SetActiveNewCircleTeXiao(true);
            isActiveWuDiTxObj = true;
        }
		IsWuDiState = true;
		WuDiTXObj.SetActive(isActiveWuDiTxObj);
        SetActiveNewCircleTeXiao(true);
        //gameObject.layer = LayerMask.NameToLayer("UI");
        //Debug.Log("Unity:"+"active player wuDiState -> playerIndex =============================== "+PlayerIndex);
    }

	public void ResetIsWuDiState()
	{
		Debug.Log("Unity:"+"ResetIsWuDiState -> playerIndex "+PlayerIndex);
		IsWuDiState = false;
		WuDiTXObj.SetActive(false);
		gameObject.layer = LayerMask.NameToLayer("Player");
		if (DaoJuCtrl.GetInstance() != null)
        {
			DaoJuCtrl.GetInstance().HiddenPlayerDaoJuObj(PlayerIndex, BuJiBaoType.NLHuDun);
		}

        //关闭火焰特效.
        SetActiveHuoYanDamageTX(false);
    }

    internal bool GetIsActive()
    {
        return gameObject.activeInHierarchy;
    }

	public GameObject[] ShanShuoObj;
	bool IsShanShuoState;
	float TimeShanShuo;
	public bool GetIsShanShuoState()
	{
		if (IsShanShuoState || IsWuDiState) {
			return true;
		}
		return false;
	}

	public void ShowPlayerShanShuo()
	{
		if (IsShanShuoState) {
			return;
		}
		IsShanShuoState = true;
		TimeShanShuo = Time.realtimeSinceStartup;
		InvokeRepeating("LoopPlayShanShuo", 0f, XkGameCtrl.GetInstance().TimeUnitShanShuo);
	}

	void ClosePlayerShanShuo()
	{
		IsShanShuoState = false;
		CancelInvoke("LoopPlayShanShuo");
		for (int i = 0; i < ShanShuoObj.Length; i++) {
			if (ShanShuoObj[i] != null) {
				ShanShuoObj[i].SetActive(true);
			}
        }

        //关闭火焰特效.
        SetActiveHuoYanDamageTX(false);
    }

	void LoopPlayShanShuo()
	{
		if (Time.realtimeSinceStartup - TimeShanShuo >= XkGameCtrl.GetInstance().TimeShanShuoVal) {
			ClosePlayerShanShuo();
			return;
		}

		for (int i = 0; i < ShanShuoObj.Length; i++) {
			if (ShanShuoObj[i] != null) {
				ShanShuoObj[i].SetActive(!ShanShuoObj[i].activeSelf);
			}
		}
	}

	public void SetIsQianHouFire(bool isFire)
	{
		FireScript.SetIsQianHouFire(isFire);
	}

	public void SetIsChangChengFire(bool isFire)
	{
		FireScript.SetIsChangChengFire(isFire);
	}
	
	public void SetIsJiQiangSanDanFire(bool isFire)
	{
		FireScript.SetIsJiQiangSanDanFire(isFire);
	}

	public void SetIsQiangJiFire(bool isFire)
	{
		FireScript.SetIsQiangJiFire(isFire);
	}
	
	public void SetIsPaiJiPaoFire(bool isFire)
	{
		FireScript.SetIsPaiJiPaoFire(isFire);
    }

    public void SetIsOpenChongJiBoZPFire(bool isFire)
    {
        FireScript.SetIsOpenChongJiBoZPFire(isFire);
    }

    public void SetIsSanDanZPFire(bool isFire)
	{
		FireScript.SetIsSanDanZPFire(isFire);
	}

	public void SetIsHuoLiAllOpen(bool isFire)
	{
		FireScript.SetIsHuoLiAllOpen(isFire);
	}

	public bool GetIsHuoLiAllOpen()
	{
		return FireScript.GetIsHuoLiAllOpen();
	}

	float TimeFanWeiCheck;
	List<XKPlayerMvFanWei> PlayerFanWeiList;
	void AddPlayerFanWeiList()
	{
		if (PlayerFanWeiList == null) {
			PlayerFanWeiList = new List<XKPlayerMvFanWei>();
			PlayerFanWeiList.Clear();
		}

		if (!PlayerFanWeiList.Contains(XKPlayerMvFanWei.GetInstanceQian())) {
			PlayerFanWeiList.Add(XKPlayerMvFanWei.GetInstanceQian());
		}
		
		if (!PlayerFanWeiList.Contains(XKPlayerMvFanWei.GetInstanceHou())) {
			PlayerFanWeiList.Add(XKPlayerMvFanWei.GetInstanceHou());
		}
		
		if (!PlayerFanWeiList.Contains(XKPlayerMvFanWei.GetInstanceZuo())) {
			PlayerFanWeiList.Add(XKPlayerMvFanWei.GetInstanceZuo());
		}
		
		if (!PlayerFanWeiList.Contains(XKPlayerMvFanWei.GetInstanceYou())) {
			PlayerFanWeiList.Add(XKPlayerMvFanWei.GetInstanceYou());
		}
	}

    //	void OnGUI()
    //	{
    //		string strA = "localEulerAngles "+PlayerCore.transform.localEulerAngles.ToString("f2")
    //			+", PlayerRotStateYG "+PlayerRotStateYG;
    //		GUI.Box(new Rect(0f, 0f, 400f, 25f), strA);
    //	}
    
    bool IsHitMvFanWeiHou;
    float m_LastHitFanWeiHouTime = 0f;
	void CheckPlayerMvFanWeiHou()
	{
        if (IsHitMvFanWeiHou == true)
        {
            if (Time.time - m_LastHitFanWeiHouTime >= 1f)
            {
                IsHitMvFanWeiHou = false;
                m_LastHitFanWeiHouTime = Time.time;
            }
        }
		//Transform tranA = XKPlayerMvFanWei.GetInstanceHou().transform;
		//Transform tranB = PlayerTran;
		//Vector3 posA = tranA.position;
		//Vector3 posB = tranB.position;
		//Vector3 vecA = -tranA.forward;
		//Vector3 vecB = Vector3.zero;
		//float cosAB = 0f;
		//float disAB = 0f;
		//float disVal = 0f;
		//vecB = posB - posA;
		//vecA.y = vecB.y = 0f;
		//disAB = vecB.magnitude;
		//cosAB = Vector3.Dot(vecA.normalized, vecB.normalized);
		//disVal = cosAB * disAB;
        //float disPlayerToCol = Mathf.Sqrt(Mathf.Pow(disAB, 2f) - Mathf.Pow(disVal, 2f));
		//float minDis = 0.5f + (tranA.lossyScale.z * 0.5f);
		//float minDis = 1f;
		//TestMinDisVal = minDis;
		//IsHitMvFanWeiHou = disVal > minDis ? false : true;
        //IsHitMvFanWeiHou = disPlayerToCol > minDis ? false : true;
        //SSDebug.Log("disVal ================== " + disVal);
        //SSDebug.Log("disPlayerToCol ================== " + disPlayerToCol);
	}

	void CheckPlayerIsResetPosition()
	{
		if (Time.realtimeSinceStartup - TimeFanWeiCheck < 0.5f) {
			return;
		}
		TimeFanWeiCheck = Time.realtimeSinceStartup;

		if (TKMoveSt == TKMoveState.YaoGanBan) {
			CheckPlayerMvFanWeiHou();
		}

		float disCur = 25f;
		float cosAngCur = 0f;
		Vector3 posA = PlayerTran.position;
		Vector3 posB = PlayerTran.position;
		Vector3 vecA = PlayerTran.forward;
		Vector3 vecB = PlayerTran.forward;
		Vector3 vecBN = PlayerTran.forward;
		Transform tranFW = null;
		bool isResetPosition = false;
		for (int i = 0; i < PlayerFanWeiList.Count; i++) {
			if (PlayerFanWeiList[i] == null) {
				continue;
			}

			tranFW = PlayerFanWeiList[i].transform;
			if (tranFW == null) {
				continue;
			}

			posA = PlayerTran.position;
			posB = tranFW.position;
			posA.y = posB.y = 0f;
			vecA = tranFW.forward;
			vecB = posB - posA;
			vecA.y = vecB.y = 0f;
			vecBN = vecB.normalized;
			cosAngCur = Vector3.Dot(vecA, vecBN);
			disCur = ((cosAngCur - CosAngMin) / KeyCosDis) + DisMin;
			if (cosAngCur < 0f && vecB.magnitude > disCur) {
				isResetPosition = true;
				//Debug.Log("Unity:"+"cosAngCur "+cosAngCur+", disCur "+disCur+", vecBLen "+vecB.magnitude);
				break;
			}
		}

		if (isResetPosition) {
			if (!IsWuDiState
                && GameTimeCtrl.GetInstance() != null
                && GameTimeCtrl.GetInstance().GetIsCheckTimeSprite()) {
				XkGameCtrl.GetInstance().SubGamePlayerHealth(PlayerIndex,
				                                             XKPlayerGlobalDt.GetInstance().DamageFanWeiOut,
				                                             true);
			}
			XkGameCtrl.ActivePlayerToGame(PlayerIndex, true);
		}
	}
	const float CosAngMax = -1f;
	const float CosAngMin = 0f;
	const float DisMax = 1f;
	const float DisMin = 25f;
	float KeyCosDis = (CosAngMax - CosAngMin) / (DisMax - DisMin);

	void SetRigbodyKinematic(bool isKine)
	{
		if (RigCom == null) {
			return;
		}
		//IsMoveToTiaoYueDian = isKine;
		RigCom.useGravity = !isKine;
	}

	public GameObject PlayerCore;
	bool IsMoveToTiaoYueDian;
	public GameObject[] TiaoBanObjAy;
	public bool GetIsMoveToTiaoYueDian()
	{
		return IsMoveToTiaoYueDian;	
	}

	public void MakePlayerToTiaoYueDian(Transform tiaoYueDianTr)
	{
		GameObject obj = (GameObject)Instantiate(XKPlayerGlobalDt.GetInstance().TiaoBanExpObjStart,
		                                         PlayerTran.position,
		                                         Quaternion.identity);
		obj.transform.parent = XkGameCtrl.PlayerAmmoArray;
		XkGameCtrl.CheckObjDestroyThisTimed(obj);

		TiaoBanObjAy[0].transform.position = PlayerTran.position;
		TiaoBanObjAy[1].transform.parent = TiaoBanObjAy[0].transform;
		TiaoBanObjAy[1].transform.localPosition = Vector3.zero;

		SetRigbodyKinematic(true);
		Vector3 endPos = tiaoYueDianTr.position;
		PlayerCore.transform.localEulerAngles = new Vector3(-45f, 0f, 0f);
		float lobHeight = XKPlayerGlobalDt.GetInstance().GDKeyTiaoBanPaoWuXian;
		float lobTime = XKPlayerGlobalDt.GetInstance().FlyTimeTiaoBanPaoWuXian;
		iTween.MoveBy(TiaoBanObjAy[1], iTween.Hash("y", lobHeight,
		                                      "time", lobTime * 0.5f,
		                                      "easeType", iTween.EaseType.easeOutQuad));
		iTween.MoveBy(TiaoBanObjAy[1], iTween.Hash("y", -lobHeight,
		                                      "time", lobTime * 0.5f,
		                                      "delay", lobTime * 0.5f,
		                                      "easeType", iTween.EaseType.easeInCubic));
		iTween.MoveTo(TiaoBanObjAy[0], iTween.Hash("position", endPos,
		                                      "time", lobTime,
		                                      "easeType", iTween.EaseType.linear,
		                                      "oncomplete", "MovePlayerOverPaoWuXianByITween"));
	}

	public void MovePlayerOverPaoWuXianByITween(int key = 0)
	{
		if (!IsMoveToTiaoYueDian) {
			return;
		}
		ClearItweenScript(TiaoBanObjAy[0]);
		ClearItweenScript(TiaoBanObjAy[1]);

		if (key == 0) {
			PlayerCore.transform.localEulerAngles = Vector3.zero;
			PlayerTran.position = TiaoBanObjAy[0].transform.position;
		}
		SetRigbodyKinematic(false);
		CheckPlayerDamageNpcByPaoWuXian();
	}

	void OnHitMoveFanWeiByPaoWuXianRun()
	{
		//Debug.Log("Unity:"+"OnHitMoveFanWeiByPaoWuXianRun...");
		SetRigbodyKinematic(false);
		IsOpenCheckPaoWuXianTerrain = true;
	}

	bool IsOpenCheckPaoWuXianTerrain;
	void CheckPlayerIsOpenCheckPaoWuXianTerrain(GameObject hitObj)
	{
		if (!IsOpenCheckPaoWuXianTerrain) {
			return;
		}

		if (hitObj.GetComponent<Terrain>() == null) {
			return;
		}
		IsOpenCheckPaoWuXianTerrain = false;
		CheckPlayerDamageNpcByPaoWuXian();
	}

	void CheckPlayerDamageNpcByPaoWuXian()
	{
		GameObject obj = (GameObject)Instantiate(XKPlayerGlobalDt.GetInstance().TiaoBanExpObjOver,
		                                         PlayerTran.position,
		                                         Quaternion.identity);
		obj.transform.parent = XkGameCtrl.PlayerAmmoArray;
		XkGameCtrl.CheckObjDestroyThisTimed(obj);

		XKNpcHealthCtrl healthScript = null;
		Transform[] npcArray = XkGameCtrl.GetInstance().GetNpcTranList().ToArray();
		int max = npcArray.Length;
		//Debug.Log("Unity:"+"max *** "+max);
		Vector3 posA = PlayerTran.position;
		Vector3 posB = Vector3.zero;
		for (int i = 0; i < max; i++) {
			if (npcArray[i] == null) {
				continue;
			}
			
			posB = npcArray[i].position;
			posA.y = posB.y = 0f;
			/*float disTest = Vector3.Distance(posA, posB);
			Debug.Log("Unity:"+"disTest "+disTest+", posA "+posA+", posB "+posB+", AmmoDamageDis "+AmmoDamageDis);*/
			if (Vector3.Distance(posA, posB) <= XKPlayerGlobalDt.GetInstance().DamageDisTiaoBan) {
				healthScript = npcArray[i].GetComponentInChildren<XKNpcHealthCtrl>();
				if (healthScript != null) {
					//Add Damage Npc num to PlayerInfo.
					healthScript.OnDamageNpc(10000, PlayerIndex);
				}
			}
		}
	}

	public static void ClearItweenScript(GameObject obj)
	{
		iTween[] itweenArray = obj.GetComponents<iTween>();
		for (int i = 0; i < itweenArray.Length; i++) {
			if (itweenArray[i] == null) {
				continue;
			}
			itweenArray[i].isRunning = false;
			itweenArray[i].enabled = false;
			DestroyObject(itweenArray[i]);
		}
	}
	
	public TKMoveState GetTKMoveState()
	{
		return TKMoveSt;
	}
	
	public bool GetIsActiveZhuiYa()
	{
		return IsActiveZhuiYa;
	}

	public void SetIsActiveZhuiYa(bool isActive)
	{
		IsActiveZhuiYa = isActive;
    }

    /// <summary>
    /// 玩家Ai坦克运动控制组件.
    /// </summary>
    internal XKPlayerAiMove m_PlayerAiMove;

    float m_LastCheckDisCameraTime = 0f;
    /// <summary>
    /// 检测玩家Ai与主摄像机的距离.
    /// </summary>
    void CheckPlayerToMainCameraDistance()
    {
        if (Time.time - m_LastCheckDisCameraTime < 5f)
        {
            return;
        }
        m_LastCheckDisCameraTime = Time.time;

        if (m_PlayerAiMove.IsOpenPlayerAiMove == false)
        {
            return;
        }

        if (Camera.main == null)
        {
            return;
        }

        float dis = 0f;
        Vector3 posA = Camera.main.transform.position;
        Vector3 posB = transform.position;
        posA.y = posB.y = 0f;
        dis = Vector3.Distance(posA, posB);
        if (dis > 200f)
        {
            //需要重置玩家位置了.
            XkGameCtrl.ActivePlayerAiTankToGame(PlayerIndex);
        }
    }

    public void SetActiveNewCircleTeXiao(bool isActive)
    {
        if (m_NewCircleTeXiao != null)
        {
            m_NewCircleTeXiao.SetActive(isActive);
        }
    }

    //***********************************************************************//
    /// <summary>
    /// 玩家获得炸车代金券数量.
    /// </summary>
    int m_ZhanCheDaiJinQuanCount = 0;
    internal void AddZhanCheDaiJinQuanCount()
    {
        m_ZhanCheDaiJinQuanCount++;
        SSDebug.Log(PlayerIndex + "::AddZhanCheDaiJinQuanCount -> m_ZhanCheDaiJinQuanCount ============= " + m_ZhanCheDaiJinQuanCount);
    }
    
    internal int GetZhanCheDaiJinQuanCount()
    {
        return m_ZhanCheDaiJinQuanCount;
    }

    internal void ResetZhanCheDaiJinQuanCount()
    {
        m_ZhanCheDaiJinQuanCount = 0;
        SSDebug.Log(PlayerIndex + "::ResetZhanCheDaiJinQuanCount -> m_ZhanCheDaiJinQuanCount ============= " + m_ZhanCheDaiJinQuanCount);
    }

    public static void ResetZhanCheDaiJinQuanCount(PlayerEnum indexPlayer)
    {
        XKPlayerMoveCtrl playerCom = GetXKPlayerMoveCom(indexPlayer);
        if (playerCom != null)
        {
            playerCom.ResetZhanCheDaiJinQuanCount();
			if (XkPlayerCtrl.GetInstanceFeiJi() != null)
			{
				XkPlayerCtrl.GetInstanceFeiJi().RemoveGetZhanCheDaiJinQuanPlayer(playerCom);
			}
        }
    }

    //********************************************************************************************************//
    /// <summary>
    /// 当JPBoss的火焰武器碰上玩家时进入此函数.
    /// </summary>
    internal void OnJPBossHuoYanHitPlayer()
    {
        if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == true)
        {
            //当没有玩家激活游戏时,不用展示玩家的受伤火焰特效.
            return;
        }

        //打开火焰特效.
        SetActiveHuoYanDamageTX(true);
    }

    /// <summary>
    /// 设置火焰特效激活状态.
    /// </summary>
    void SetActiveHuoYanDamageTX(bool isActive)
    {
        //SSDebug.LogWarning("SetActiveHuoYanDamageTX -> isActive ============= " + isActive);
        if (m_HuoYanDamageTX != null)
        {
            m_HuoYanDamageTX.SetActive(isActive);
        }
    }
}