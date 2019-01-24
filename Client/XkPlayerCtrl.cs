using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerTypeEnum
{
	FeiJi,
	TanKe,
	CartoonCamera,
}

public class XkPlayerCtrl : MonoBehaviour
{
    /// <summary>
    /// 摄像机微动的动画控制组件.
    /// </summary>
    public Animator m_CameraMoveAni;
    AiPathCtrl AiPathScript;
	public PlayerTypeEnum PlayerSt = PlayerTypeEnum.FeiJi;
	//public Transform KaQiuShaAimPoint;
	/**
1．主角飞机机枪开枪音效（3D音效）（主角飞机和坦克相互听得见）.
2．主角飞机打高爆弹音效（3D音效）（主角飞机和坦克相互听得见）.
3．主角飞机发射导弹音效（3D音效）（主角飞机和坦克相互听得见）.
4．主角飞机飞行音效（3D音效）（主角飞机和坦克相互听得见）.
5．主角坦克机枪开枪音效（3D音效）（主角飞机和坦克相互听得见）.
6．主角坦克打高爆弹音效（3D音效）（主角飞机和坦克相互听得见）.
7．主角坦克开炮音效（3D音效）（主角飞机和坦克相互听得见）.
8．主角坦克行驶音效（3D音效）（主角飞机和坦克相互听得见）.

PlayerAudio[0] -> p1主角飞机/坦克机枪开枪音效.
PlayerAudio[1] -> p1主角飞机/坦克打高爆弹音效.
PlayerAudio[2] -> p1主角飞机/坦克发射导弹音效.
PlayerAudio[3] -> p2主角飞机/坦克机枪开枪音效.
PlayerAudio[4] -> p2主角飞机/坦克打高爆弹音效.
PlayerAudio[5] -> p2主角飞机/坦克发射导弹音效.
PlayerAudio[6] -> 主角飞机/坦克行驶音效.
	 */
//	public AudioSource[] PlayerAudio;
//	public Transform[] NpcFirePosArray;
//	public GameObject[] PlayerHiddenArray;
//	public Transform[] PlayerCamPoint; //主角在服务端的跟踪点.
	public static Transform MissionCleanup;
    /// <summary>
    /// 战车npc管理组件.
    /// </summary>
    public SpawnNpcManage m_SpawnNpcManage;
    GameObject PlayerObj;
	Transform PlayerTran;
	public static Transform PlayerTranFeiJi;
	public static Transform PlayerTranTanKe;
//	public static Transform KaQiuShaAimPlayerTranFJ;
//	public static Transform KaQiuShaAimPlayerTranTK;
	bool IsStopMovePlayer = true;
	iTween ITweenScript;
	bool IsOrienttopath;
	bool IsMoveToAiMark;
	List<XKSpawnNpcPoint> AimSpawnPoint;
	GameObject AimNpcObj;
	public static float TimeUnitMove = 0.02f;
	float SpeedA = 0f;
	float SpeedB;
	float JiaSuDuVal;
	Transform AimMvToMarkTran;
	bool IsStartMovePlayerByMark;
	Vector3 ForwardMoveVal;
    /// <summary>
    /// 游戏摄像机到目标点的最大距离.
    /// </summary>
    float m_MaxDistanceToNode = 1000f;
    Vector3 EndPos;
	Quaternion RotationStart;
	Quaternion RotationEnd;
	float TimeRotation;
	Vector3[] PathNodes;
	bool IsDelayMovePlayer;
	float TimeDelayMove;
	Vector3 PlayerPosCur = new Vector3(0f, -1800f, 0f);
	Quaternion PlayerRotCur;
	bool IsPlayerTingLiu = true;
	float TimeTingLiuVal;
	float TimeTingLiuValStart;
	XKPlayerCamera PlayerCamera;
	//PlayerZhiShengJiCtrl PlayerZhiShengJiScript;
	int MarkCount;
	public bool IsTestDrawPath;
//	XKPlayerAutoFire PlayerAutoFireScript;
//	NetworkView NetViewCom;
	bool IsHandleRpc;
	float TimeCheckAimNpcLast;
	float[] TimeFireLast = new float[2];
	XkPlayerTanKePaoTai PaoTaiScript;
	int QuanShuCount;
//	public Transform TestCubeTr;
	static XkPlayerCtrl InstanceFeiJi;
	public static XkPlayerCtrl GetInstanceFeiJi()
	{
		return InstanceFeiJi;
	}
	static XkPlayerCtrl InstanceTanKe;
	public static XkPlayerCtrl GetInstanceTanKe()
	{
		return InstanceTanKe;
	}
	static XkPlayerCtrl InstanceCartoon;
	public static XkPlayerCtrl GetInstanceCartoon()
	{
		return InstanceCartoon;
	}

	// Use this for initialization
	void Awake()
	{
		switch (PlayerSt) {
		case PlayerTypeEnum.FeiJi:
			InstanceFeiJi = this;
			PlayerTranFeiJi = transform;
//			KaQiuShaAimPlayerTranFJ = KaQiuShaAimPoint;
			Invoke("DelaySetFeiJiNpcInfo", 1f);
			break;

		case PlayerTypeEnum.TanKe:
			InstanceTanKe = this;
			PlayerTranTanKe = transform;
//			KaQiuShaAimPlayerTranTK = KaQiuShaAimPoint;
			break;

		case PlayerTypeEnum.CartoonCamera:
			InstanceCartoon = this;
			break;
		}
        if (PlayerSt != PlayerTypeEnum.CartoonCamera) {
			XkGameCtrl.GetInstance().ChangeAudioListParent();
		}
		PlayerObj = gameObject;
		PlayerTran = transform;
		AimSpawnPoint = new List<XKSpawnNpcPoint>();

        if (m_SpawnNpcManage != null)
        {
            m_SpawnNpcManage.gameObject.SetActive(true);
        }
    }

    void DelaySetFeiJiNpcInfo()
	{
		if (PlayerSt != PlayerTypeEnum.FeiJi) {
			return;
		}

//		if (IsHandleRpc) {
//			if (XkGameCtrl.GameModeVal == GameMode.LianJi) {
//				if (Network.peerType != NetworkPeerType.Disconnected) {
//					PlayerHiddenArray[0].SetActive(false);
//					PlayerHiddenArray[1].SetActive(false);
//				}
//				else {
//					PlayerHiddenArray[0].SetActive(false);
//				}
//			}
//			else {
//				PlayerHiddenArray[0].SetActive(false);
//				PlayerHiddenArray[1].SetActive(false);
//			}
//		}
//		else {
//			PlayerHiddenArray[0].SetActive(false);
//		}
	}

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
	{
		if (!XkGameCtrl.IsDrawGizmosObj) {
			return;
		}

		if (!enabled) {
			return;
		}

		if (!IsTestDrawPath || AiPathScript == null) {
			return;
		}

		AiPathCtrl pathScript = AiPathScript;
		if (!pathScript.enabled) {
			pathScript.enabled = true;
		}
		pathScript.DrawPath();
	}
#endif

    void FixedUpdate()
	{
		SmothMovePlayerCamera();
	}
	
	void LateUpdate()
	{
		SmothMovePlayerCamera();
	}

	void SmothMovePlayerCamera()
	{
		if (XKPlayerHeTiData.IsActiveHeTiPlayer) {
			if (PlayerSt == PlayerTypeEnum.FeiJi || PlayerSt == PlayerTypeEnum.TanKe) {
				this.enabled = false;
				return;
			}
			return;
		}

		if (PlayerCamera == null) {
			return;
		}

//		if (PlayerSt == PlayerTypeEnum.TanKe && PlayerZhiShengJiScript != null) {
//			PlayerZhiShengJiScript.MakePlayerTanKeMoveToLand();
//		}
		PlayerCamera.SmothMoveCamera();
	}

	void Update()
	{
//		if (IsHandleRpc) {
//			SendPlayerTransformInfo();
//		}
//		else {
//			UpdatePlayerTransform();
//		}
        UpdateCheckIsCreatGenZongDan();
        UpdateCameraWeiDongPos();
        SmothMovePlayerCamera();
		CheckIsDelayMovePlayer();
//		if (PlayerSt == PlayerTypeEnum.FeiJi
//		         || PlayerSt == PlayerTypeEnum.CartoonCamera) {
//			CheckAimNpcObj();
//		}
	}

	public void SetPlayerAutoFireScript(XKPlayerAutoFire script)
	{
//		PlayerAutoFireScript = script;
	}

	public void SpawnPlayerDaoDan(Transform ammoTran, GameObject playerDaoDan)
	{
//		PlayerAutoFireScript.SpawnPlayerDaoDan(ammoTran, playerDaoDan);
	}

	void UpdatePlayerTransform()
	{
		if (Network.peerType == NetworkPeerType.Disconnected) {
			return;
		}
		
		if (IsHandleRpc) {
			return;
		}
		
		if (PlayerPosCur == new Vector3(0f, -1800f, 0f)) {
			return;
		}

		if (PlayerSt == PlayerTypeEnum.CartoonCamera) {
			if (ScreenDanHeiCtrl.IsStartGame && !XKPlayerHeTiData.IsActiveHeTiPlayer) {
				PlayerObj.SetActive(false);
				return;
			}
		}
		else {
			if (!ScreenDanHeiCtrl.IsStartGame || XKTriggerOpenPlayerUI.IsActiveOpenPlayerUI) {
				return;
			}
		}
		
		PlayerTran.position = PlayerPosCur;
		PlayerTran.rotation = PlayerRotCur;
	}

	void SendPlayerTransformInfo()
	{
		if (!IsHandleRpc) {
			return;
		}

		if (NetworkServerNet.ServerSendState != 0) {
			return;
		}

		if (PlayerSt == PlayerTypeEnum.CartoonCamera) {
			if (Network.peerType != NetworkPeerType.Server) {
				return;
			}
			
			if (ScreenDanHeiCtrl.IsStartGame && !XKTriggerOpenPlayerUI.IsActiveOpenPlayerUI) {
				PlayerObj.SetActive(false);
				return;
			}
		}
		else {
			if (Network.peerType != NetworkPeerType.Client) {
				return;
			}
			
			if (!ScreenDanHeiCtrl.IsStartGame || XKTriggerOpenPlayerUI.IsActiveOpenPlayerUI) {
				return;
			}
		}

//		NetViewCom.RPC("XKPlayerSendOtherTranformInfo", RPCMode.OthersBuffered, PlayerTran.position, PlayerTran.rotation);
	}

	[RPC] void XKPlayerSendOtherTranformInfo(Vector3 pos, Quaternion rot)
	{
		PlayerPosCur = pos;
		PlayerRotCur = rot;
	}

	public void StopMovePlayer()
	{
		if (IsStopMovePlayer) {
			return;
		}
		IsStopMovePlayer = true;

		iTween itweenScript = GetComponent<iTween>();
		if (itweenScript != null) {
			itweenScript.isRunning = false;
			ITweenScript = itweenScript;
		}
	}

	/**
	 * key == 1 -> 强制打开卡通摄像机，并且使卡通摄像机运动.
	 */
	public void RestartMovePlayer(int key = 0)
	{
		if (key == 1) {
			Debug.Log("Unity:"+"RestartMovePlayer:: key is "+key);
			IsStartMovePlayerByMark = false;
			PlayerObj.SetActive(true);
			PlayerCamera.SetEnableCamera(false);
			PlayerCamera.SetActiveCamera(true);
			PlayerCamera.SetEnableCamera(false);
		}

		if (!IsHandleRpc) {
			return;
		}

//		if (PlayerSt != PlayerTypeEnum.CartoonCamera) {
//			XkGameCtrl.AddPlayerYouLiangToMax();
//		}

		if (!XkGameCtrl.GetInstance().IsCartoonShootTest) {
			if (!IsStopMovePlayer) {
				return;
			}
		}
		IsStopMovePlayer = false;

		if (PlayerSt == PlayerTypeEnum.FeiJi) {
			Debug.Log("Unity:"+"RestartMovePlayer -> player "+PlayerSt);
		}

		if (ITweenScript != null) {
			ITweenScript.isRunning = true;
		}
		else {
			MovePlayerByItween();
		}
	}

	public void MakePlayerFlyToPathMark()
	{
		if (!XkGameCtrl.GetInstance().IsCartoonShootTest) {
			if (!IsStopMovePlayer) {
				Debug.Log("Unity:"+"MakePlayerFlyToPathMark -> player "+PlayerSt);
				return;
			}
		}
		
		Transform tran = null;
		if (AiPathScript == null) {
			switch (PlayerSt) {
			case PlayerTypeEnum.FeiJi:
				tran = XkGameCtrl.GetInstance().GmCamMark.transform;
				break;
				
			case PlayerTypeEnum.TanKe:
				//tran = XkGameCtrl.GetInstance().TanKePlayerMark.transform;
				break;
			}
			transform.position = tran.position;
			transform.rotation = tran.rotation;

			if (Network.peerType == NetworkPeerType.Server) {
				PlayerCamera.SetActiveCamera(true);
			}
			return;
		}

		tran = AiPathScript.transform.GetChild(MarkCount);
		switch (PlayerSt) {
		case PlayerTypeEnum.FeiJi:
			if (XKPlayerCamera.GetInstanceTanKe() != null) {
				XKPlayerCamera.GetInstanceTanKe().SetActiveCamera(true);
			}
			PlayerCamera.SetActiveCamera(true);
			PlayerTranFeiJi.position = tran.position;
			PlayerTranFeiJi.rotation = tran.rotation;
			break;

		case PlayerTypeEnum.TanKe:
			if (XKPlayerCamera.GetInstanceFeiJi() != null) {
				XKPlayerCamera.GetInstanceFeiJi().SetActiveCamera(true);
			}
			PlayerCamera.SetActiveCamera(true);
			PlayerTranTanKe.position = tran.position;
			PlayerTranTanKe.rotation = tran.rotation;
			break;

		case PlayerTypeEnum.CartoonCamera:
			transform.position = tran.position;
			transform.rotation = tran.rotation;
			break;
		}
	}

	public void DelayMoveCartoonCamera()
	{
		if (PlayerSt != PlayerTypeEnum.CartoonCamera) {
			return;
		}
		CallRestartMovePlayer();

		//ScreenDanHeiCtrl.GetInstance().StartPlayDanHei();
		//RenWuXinXiCtrl.GetInstance().InitRenWuSprite();
	}

	void CallRestartMovePlayer()
	{
		RestartMovePlayer();

//		CancelInvoke("DelayShowTiaoGuoBt");
//		Invoke("DelayShowTiaoGuoBt", 0f);
	}

	void SetIsPlayerTingLiu(AiMark markScript)
	{
		if (markScript.PlayerAni != ZhiShengJiAction.Null) {
			return;
		}

		IsPlayerTingLiu = markScript.TimePlayerAni > 0f ? true : false;
		TimeTingLiuVal = markScript.TimePlayerAni;
	}

	void ResetIsPlayerTingLiu()
	{
		IsPlayerTingLiu = false;
		TimeTingLiuVal = 0f;
	}

	public void SetPlayerCamera(XKPlayerCamera script)
	{
		PlayerCamera = script;
		if (PlayerSt == PlayerTypeEnum.TanKe) {
			PaoTaiScript = GetComponentInChildren<XkPlayerTanKePaoTai>();
			PaoTaiScript.SetPlayerCameraInfo(script.transform);
		}
	}

	public XKPlayerCamera GetPlayerCameraScript()
	{
		return PlayerCamera;
	}

	public void SetPlayerZhiShengJiScript(PlayerZhiShengJiCtrl script)
	{
		//PlayerZhiShengJiScript = script;
	}

	public void PlayZhuJiaoMarkAction(AiMark markScript)
	{
		SetIsPlayerTingLiu(markScript);
		//PlayerCamera.SetAimTranInfo(markScript);
	}

	public void MovePlayerByItween()
	{
		if (IsStopMovePlayer) {
			return;
		}

		if (Network.peerType == NetworkPeerType.Server && PlayerSt != PlayerTypeEnum.CartoonCamera) {
			return;
		}
		InitMovePlayerByMarkSpeed();
	}

	void InitMovePlayerByMarkSpeed()
	{
		Transform tran = AiPathScript.transform;
		Transform[] tranArray = new Transform[2];
		tranArray[0] = transform;
		tranArray[1] = tran.GetChild(MarkCount);
        //		Debug.Log("Unity:"+"InitMovePlayerByMarkSpeed -> path "+AiPathScript.name
        //		          +", mark "+tranArray[1].name);

        //if (tranArray[1] == XkGameCtrl.GetInstance().GmCamMark.transform)
        //{
        //    QuanShuCount++;
        //    if (QuanShuCount > 1)
        //    {
        //        XkGameCtrl.GetInstance().AddPlayerQuanShu();
        //    }
        //}

        EndPos = tranArray[1].position;
		AiMark markScript = tranArray[1].GetComponent<AiMark>();
		AimMvToMarkTran = markScript.transform;
		if (EndPos == tranArray[0].position) {
			//move to next mark point
			SpeedA = markScript.GetMvSpeed();
			MovePlayerOnCompelteITween();
			return;
		}

		PathNodes = AiPathScript.GetPathNodes(MarkCount);
		if (PathNodes == null) {
			Debug.Log("Unity:"+"PathNodes is null! MarkCount "+MarkCount);
			return;
		}
		
		EndPos = PathNodes[1];
		ForwardMoveVal = Vector3.Normalize(EndPos - tranArray[0].position);
        m_MaxDistanceToNode = Vector3.Distance(EndPos, transform.position);
        SpeedB = markScript.GetMvSpeed();
		float disVal = 0f;
		int maxNodes = PathNodes.Length;
		for (int i = 1; i < maxNodes; i++) {
			disVal += Vector3.Distance(PathNodes[i - 1], PathNodes[i]);
		}

		float timeVal = (2f * disVal) / (SpeedA + SpeedB);
		TimeRotation = timeVal;
		if (SpeedB != SpeedA) {
			JiaSuDuVal = ((SpeedB - SpeedA) * (SpeedA + SpeedB)) / (2f * disVal);
		}
		else {
			JiaSuDuVal = 0f;
		}
		RotationStart = tranArray[0].rotation;
		RotationEnd = tranArray[1].rotation;
//		Debug.LogError("Unity:"+"SpeedA *** "+SpeedA+", MarkCount "+MarkCount+", maxNodes "+maxNodes);
//		Debug.LogWarning("Unity:"+"SpeedA *** "+SpeedA+", SpeedB "+SpeedB);
		if (!IsStartMovePlayerByMark) {
			StartCoroutine(MovePlayerByMarkSpeed());
			IsStartMovePlayerByMark = true;

            if (PlayerSt == PlayerTypeEnum.FeiJi)
            {
                if (m_SpawnNpcManage != null)
                {
                    m_SpawnNpcManage.MakeAllCreatNpcPointsToLand();
                }
                XkGameCtrl.GetInstance().OpenAllAiPlayerTank();
            }
		}
	}

	//public static float TestDTimeVal;
	//public static float TestSpeed;
    //float TestDis;
    //private void OnGUI()
    //{
    //    GUI.Box(new Rect(15f, 15f, 150f, 30f), "dis = " + TestDis.ToString("f2"));
    //}
    IEnumerator MovePlayerByMarkSpeed()
	{
		bool isStopMove = false;
		int countNode = 0;
		int maxCountNode = PathNodes.Length;
		EndPos = PathNodes[1];

//		Debug.Log("Unity:"+"MovePlayerByMarkSpeed -> start time "+Time.realtimeSinceStartup);
		float timeLastVal = Time.realtimeSinceStartup;
		float timeRotationVal = 0f;
		do {
			if (JiFenJieMianCtrl.GetInstance() != null
                && JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()
                && XkGameCtrl.IsLoadingLevel) {
				Debug.Log("Unity:"+"MovePlayerByMarkSpeed -> IsLoadingLevel "+XkGameCtrl.IsLoadingLevel
				          +", IsShowFinishTask "+JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask());
				yield break;
			}

			if (PlayerSt == PlayerTypeEnum.CartoonCamera) {
				if (XkGameCtrl.GetInstance().IsCartoonShootTest) {
					Debug.Log("Unity:"+"MovePlayerByMarkSpeed -> IsCartoonShootTest "
					          +XkGameCtrl.GetInstance().IsCartoonShootTest);
					yield break;
				}

				if (!PlayerCamera.GetActiveCamera()) {
					PlayerObj.SetActive(false);
					isStopMove = true;
					IsStopMovePlayer = true;
					yield break;
				}
			}
			else {
				if (XKPlayerHeTiData.IsActiveHeTiPlayer) {
					IsStopMovePlayer = true;
					yield break;
				}
			}

			if (IsMoveToAiMark) {
				countNode = 0;
				yield return new WaitForSeconds(0.1f);
				continue;
			}

			if (XKTriggerStopMovePlayer.IsActiveTrigger) {
				yield return new WaitForSeconds(0.1f);
				continue;
			}
			
            /*if (XkGameCtrl.PlayerActiveNum <= 0)
            {
                //玩家激活数量小于1时，镜头停止前进.
                if (PlayerSt == PlayerTypeEnum.FeiJi)
                {
                    yield return new WaitForSeconds(0.1f);
                    continue;
                }
            }*/

			if (XkGameCtrl.PlayerActiveNum <= 0
			    && PlayerSt != PlayerTypeEnum.CartoonCamera) {
				if (!XkGameCtrl.IsMoveOnPlayerDeath) {
					XkGameCtrl.PlayerYouLiangCur = 0;
					timeLastVal = Time.realtimeSinceStartup;
					yield return new WaitForSeconds(0.1f);
					continue;
				}
			}
			else {
				if (IsDelayMovePlayer) {
					IsDelayMovePlayer = false;
					ResetIsPlayerTingLiu();
					yield return new WaitForSeconds(TimeDelayMove);
					continue;
				}
			}

			float ds = 0f;
            float dTime = TimeUnitMove;
            //float dTime = Time.realtimeSinceStartup - timeLastVal;
			//if (dTime > 1f) {
			//	dTime = TimeUnitMove;
			//}
			/*else {
				float minTimeUnit = 0.03f;
				dTime = dTime > minTimeUnit ? minTimeUnit : dTime;
			}*/

			//bool  isRecordDTime = false;
			//if (XkGameCtrl.GameJiTaiSt == GameJiTaiType.FeiJiJiTai) {
			//	if (PlayerSt == PlayerTypeEnum.FeiJi) {
			//		isRecordDTime = true;
			//	}
			//}
			//else if (XkGameCtrl.GameJiTaiSt == GameJiTaiType.TanKeJiTai) {
			//	if (PlayerSt == PlayerTypeEnum.TanKe) {
			//		isRecordDTime = true;
			//	}
			//}

			//if (isRecordDTime) {
			//	TestDTimeVal = dTime;
			//}

			//dTime = TimeUnitMove; //test
			timeLastVal = Time.realtimeSinceStartup;
			float disAimNode = Vector3.Distance(transform.position, EndPos);
            //TestDis = disAimNode;
            if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().GmCamMark != null)
            {
                SpeedA = XkGameCtrl.GetInstance().GmCamMark.GetMvSpeed();
            }

			if (JiaSuDuVal == 0) {
				ds = SpeedA * dTime;
			}
			else {
				ds = (SpeedA * dTime) + (0.5f * JiaSuDuVal * Mathf.Pow(dTime, 2f));
				if ((SpeedA >= SpeedB && JiaSuDuVal >= 0f)
				    || (SpeedA <= SpeedB && JiaSuDuVal <= 0f)) {
					SpeedA = SpeedB;
				}
				else {
					if (disAimNode >= ds) {
						SpeedA = SpeedA + (JiaSuDuVal * dTime);
					}
					else {
						float speedValTmp = 2f*JiaSuDuVal*disAimNode + Mathf.Pow(SpeedA, 2f);
						if (speedValTmp > 0) {
							SpeedA = Mathf.Sqrt(speedValTmp);
						}
					}
					
					if ((SpeedA >= SpeedB && JiaSuDuVal >= 0f)
					    || (SpeedA <= SpeedB && JiaSuDuVal <= 0f)) {
						SpeedA = SpeedB;
					}
				}
			}

			//if (XkGameCtrl.GameJiTaiSt == GameJiTaiType.FeiJiJiTai && PlayerSt == PlayerTypeEnum.FeiJi) {
			//	TestSpeed = SpeedA;
			//}
			//else if (XkGameCtrl.GameJiTaiSt == GameJiTaiType.TanKeJiTai && PlayerSt == PlayerTypeEnum.TanKe) {
			//	TestSpeed = SpeedA;
			//}

			if (PlayerSt == PlayerTypeEnum.FeiJi
			    || PlayerSt == PlayerTypeEnum.CartoonCamera) {
				timeRotationVal += dTime;
				if (timeRotationVal <= TimeRotation) {
					float perRot = timeRotationVal / TimeRotation;
					perRot = perRot > 1f ? 1f : perRot;
					transform.rotation = Quaternion.Lerp(RotationStart, RotationEnd, perRot);
				}
			}
			else {
				if (Vector3.Distance(EndPos, transform.position) > 0.01f) {
					Vector3 forwardVal = Vector3.Normalize(EndPos - transform.position);
					transform.forward = Vector3.Lerp(transform.forward, forwardVal, dTime * SpeedA * 0.5f);
				}
			}
            //Debug.Log("Unity:"+"SpeedA "+SpeedA+", ds "+ds+", dTime "+dTime + ", disAimNode " + disAimNode);

            Vector3 posA = transform.position;
            Vector3 posB = EndPos;
            posA.y = posB.y = 0f;
            Vector3 forwardValTmp = Vector3.Normalize(posB - posA);
            Vector3 forwardSelf = transform.forward;
            forwardSelf.y = 0f;
            float dotVal = Vector3.Dot(forwardValTmp, forwardSelf.normalized);
            //if (dotVal <= 0f)
            //{
            //    Debug.LogWarning("Unity: dorVal ================= " + dotVal);
            //}

            //bool isRun = false;
            //if (((disAimNode <= ds || dotVal <= 0f) && isRun == true) || m_MaxDistanceToNode < disAimNode) {
            if (disAimNode <= ds || dotVal <= 0f || (m_MaxDistanceToNode * 5f < disAimNode)) {
                //Debug.Log("Unity:" + "Over, ds " + ds + ", m_MaxDistanceToNode " + m_MaxDistanceToNode
                //    + ", disAimNode " + disAimNode);
                countNode++;
				float disVal = ds - disAimNode;
				float disNode = 0f;
				int count = 0;
				for (int i = countNode; i < (maxCountNode - 2); i++) {
					if ((i+1) >= PathNodes.Length) {
						break;
					}

					disNode = Vector3.Distance(PathNodes[i], PathNodes[i+1]);
					if (disVal > disNode) {
						disVal = disVal - disNode;
						count++;

                        if (dotVal > 0f)
                        {
                            transform.position = PathNodes[countNode];
                        }
						SmothMovePlayerCamera();
					}
					else {
						break;
					}
				}
				countNode += count;
                /*if (count > 0) {
					string outPut = PlayerSt == PlayerTypeEnum.FeiJi ? "feiJi: " : "tanKe: ";
					outPut += "countNode "+countNode+",count "+count+", maxCountNode "+maxCountNode;
					Debug.Log("Unity:"+outPut);
				}*/

                if (countNode < (maxCountNode - 1) && (countNode + 1) < PathNodes.Length)
                {
                    if (dotVal > 0f)
                    { 
                        transform.position = EndPos;
                    }
					SmothMovePlayerCamera();
					EndPos = PathNodes[countNode+1]; //更新EndPos.
					ForwardMoveVal = Vector3.Normalize(EndPos - transform.position); //更新ForwardMoveVal.
                    m_MaxDistanceToNode = Vector3.Distance(EndPos, transform.position);

//					Debug.Log("Unity:"+"***realDis "+Vector3.Distance(transform.position, EndPos));
//					Debug.Log("Unity:"+"***ForwardMoveVal "+Vector3.Distance(ForwardMoveVal, Vector3.zero));
					yield return new WaitForSeconds(TimeUnitMove);
					continue;
				}
				else {
					if (IsPlayerTingLiu) {
						TimeTingLiuValStart = Time.realtimeSinceStartup;
						yield return new WaitForSeconds(TimeTingLiuVal);
						if (!XkGameCtrl.IsMoveOnPlayerDeath) {
							if (XkGameCtrl.IsActivePlayerOne || XkGameCtrl.IsActivePlayerTwo) {
								if (!IsDelayMovePlayer) {
									ResetIsPlayerTingLiu();
								}
							}
						}
						else {
							if (!IsDelayMovePlayer) {
								ResetIsPlayerTingLiu();
							}
						}
					}

					timeLastVal = Time.realtimeSinceStartup;
					timeRotationVal = 0f;
					SpeedA = SpeedB;
                    if (dotVal > 0f)
                    {
                        transform.position = EndPos;
                    }
					SmothMovePlayerCamera();

					if (!MovePlayerOnCompelteITween()) {
						isStopMove = true;
						break;
					}
					countNode = 0;
					maxCountNode = PathNodes.Length;
					yield return new WaitForSeconds(TimeUnitMove);
					continue;
				}
			}
			
			transform.position += (ForwardMoveVal * ds);
			SmothMovePlayerCamera();
			yield return new WaitForSeconds(TimeUnitMove);
		} while (!isStopMove);
	}

	public float GetPlayerMoveSpeed()
	{
		return SpeedA;
	}

	void DebugDrawStuff(Vector3 startPos, Vector3 endPos, Color valColor)
	{
		Debug.DrawLine(startPos, endPos, valColor);
	}

	void CheckIsDelayMovePlayer()
	{
		if (!XkGameCtrl.IsMoveOnPlayerDeath) {
			if (!XkGameCtrl.IsActivePlayerOne && !XkGameCtrl.IsActivePlayerTwo) {
				if (!IsDelayMovePlayer && IsPlayerTingLiu) {
					IsDelayMovePlayer = true;
					TimeDelayMove = TimeTingLiuVal - (Time.realtimeSinceStartup - TimeTingLiuValStart);
					//Debug.Log("Unity:"+"CheckIsDelayMovePlayer***************TimeDelayMove "+TimeDelayMove);
				}
			}
		}
	}

	bool MovePlayerOnCompelteITween()
	{
		MarkCount++;
		Transform pathTran = AiPathScript.transform;
		if (MarkCount >= pathTran.childCount) {
			if (AiPathScript.mNextPath1 == null) {
				switch (PlayerSt) {
				case PlayerTypeEnum.FeiJi:
					Debug.Log("Unity:"+"FeiJiPlayer move to end position");
					break;
					
				case PlayerTypeEnum.TanKe:
					//Debug.Log("Unity:"+"TanKePlayer move to end position");
					break;
				}
				return false;
			}
			MarkCount = 0;
			AiPathScript = AiPathScript.mNextPath1.GetComponent<AiPathCtrl>();
			//Debug.Log("Unity:"+"move next path, path is "+AiPathScript.name);
		}
		MovePlayerByItween();
		return true;
	}

	public void ExitPlayerLoopPath()
	{
		//Debug.Log("Unity:"+"ExitPlayerLoopPath***");
		if (AiPathScript.mNextPath1 == null) {
			Debug.Log("Unity:"+"Player move to end position");
			return;
		}
		
		AiPathScript = AiPathScript.mNextPath1.GetComponent<AiPathCtrl>();
		MovePlayerByItween();
	}

	public void SetAiPathScript(AiPathCtrl script)
	{
		IsHandleRpc = true;
		AiPathScript = script;
		switch (PlayerSt) {
		case PlayerTypeEnum.FeiJi:
			MarkCount = XkGameCtrl.GetInstance().GetFeiJiMarkIndex() - 1;
			break;

		case PlayerTypeEnum.TanKe:
			MarkCount = XkGameCtrl.GetInstance().GetTanKeMarkIndex() - 1;
			break;

		case PlayerTypeEnum.CartoonCamera:
			MarkCount = XkGameCtrl.GetInstance().GetCartoonCamMarkIndex() - 1;
			MakePlayerFlyToPathMark();
			//XkGameCtrl.SetActiveGameWaterObj(true);
			break;
		}
	}

	public AiPathCtrl GetAiPathScript()
	{
		return AiPathScript;
	}

	public Transform GetAimMvToMarkTran()
	{
		return AimMvToMarkTran;
	}

	public void MakePlayerMoveToAiMark(AiMark script)
	{
		IsMoveToAiMark = true;
		Transform markTran = script.transform;
		transform.position = markTran.position;
		transform.rotation = markTran.rotation;

		MarkCount = script.getMarkCount();
		AiPathScript = markTran.parent.GetComponent<AiPathCtrl>();
		InitMovePlayerByMarkSpeed();
		IsMoveToAiMark = false;
	}

	/// <summary>
	/// particleType == 0 ---> 普通子弹.
	/// particleType == 1 ---> 导弹.
	/// particleType == 2 ---> 高爆弹.
	/// </summary>
	public void CallOtherPortSpawnPlayerAmmoParticle(int playerIndex, int particleIndex, Vector3 firePos)
	{
		if (particleIndex != 1) {
			switch (playerIndex) {
			case 1:
				if (Time.realtimeSinceStartup - TimeFireLast[0] < 0.05f) {
					return;
				}
				TimeFireLast[0] = Time.realtimeSinceStartup;
				break;
				
			case 2:
				if (Time.realtimeSinceStartup - TimeFireLast[1] < 0.05f) {
					return;
				}
				TimeFireLast[1] = Time.realtimeSinceStartup;
				break;
			}
		}

		if (Network.peerType != NetworkPeerType.Client) {
			return;
		}
//		NetViewCom.RPC("XKPlayerSendSpawnAmmoParticle", RPCMode.OthersBuffered, playerIndex, particleIndex, firePos);
	}

	[RPC] void XKPlayerSendSpawnAmmoParticle(int playerIndex, int particleIndex, Vector3 firePos)
	{
//		PlayerAutoFireScript.SpawnPlayerAmmoParticle(playerIndex, particleIndex, firePos);
	}
	
	public GameObject GetAimNpcObj()
	{
		return AimNpcObj;
	}
	
	public void SetXKTriggerPlayerAim(XKTriggerPlayerAim script)
	{
		PlayerCamera.SetCameraAimNpcSpeed(script.AimSpeed, script.LeaveSpeed);
		int max = script.SpawnPoint.Length;
		for (int i = 0; i < max; i++) {
			if (script.SpawnPoint[i] != null) {
				AddAimSpawnPoint(script.SpawnPoint[i]);
			}
		}
	}
	
	void AddAimSpawnPoint(XKSpawnNpcPoint script)
	{
		if (AimSpawnPoint.Contains(script)) {
			return;
		}
		AimSpawnPoint.Add(script);
	}
	
	public void RemoveAimSpawnPoint(XKSpawnNpcPoint script)
	{
		if (!AimSpawnPoint.Contains(script)) {
			return;
		}
		AimSpawnPoint.Remove(script);
	}
	
	void CheckAimNpcObj()
	{
		float dTime = Time.realtimeSinceStartup - TimeCheckAimNpcLast;
		if (dTime < 0.1f) {
			return;
		}
		TimeCheckAimNpcLast = Time.realtimeSinceStartup;
		
		if (AimSpawnPoint.Count <= 0) {
			AimNpcObj = null; //没有需要瞄准的npc.
			return;
		}
		
		float disVal = 999999f;
		GameObject npcObj = null;
		int max = AimSpawnPoint.Count;
		for (int i = 0; i < max; i++) {
			GameObject pointNpc = AimSpawnPoint[i].GetNpcLoopObj();
			if (pointNpc == null) {
				continue;
			}

			if (pointNpc.transform.position == new Vector3(0f, -1800f, 0f)) {
				continue;
			}

			XKNpcMoveCtrl npcMoveScript = pointNpc.GetComponent<XKNpcMoveCtrl>();
			if (npcMoveScript == null || npcMoveScript.GetIsDeathNPC()) {
				continue;
			}
			
			Transform pointNpcTran = pointNpc.transform;
			float disTmp = Vector3.Distance(PlayerTran.position, pointNpcTran.position);
			if (disTmp < disVal) {
				disVal = disTmp;
				npcObj = pointNpc;
			}
		}
		
		if (npcObj == null) {
			AimNpcObj = null; //没有找到需要瞄准的npc.
			return;
		}
		
		if (npcObj != AimNpcObj) {
			AimNpcObj = npcObj; //寻找并改变距离主角最近的npc.
		}
	}

//	void DelayShowTiaoGuoBt()
//	{
//		if (Network.peerType == NetworkPeerType.Disconnected) {
//			TiaoGuoBtCtrl.GetInstanceCartoon().ShowTiaoGuoBt();
//		}
//		else {
//			if (Network.peerType == NetworkPeerType.Client) {
//				TiaoGuoBtCtrl.GetInstanceCartoon().ShowTiaoGuoBt();
//			}
//		}
//	}

	public void OpenPlayerMoveAudio()
	{
//		if (PlayerAudio[6].isPlaying) {
//			return;
//		}
//		PlayerAudio[6].Play();
	}
	
	public void StopPlayerMoveAudio()
	{
//		if (!PlayerAudio[6].isPlaying) {
//			return;
//		}
//		PlayerAudio[6].Stop();
	}

	public void HandlePlayerHiddenArray()
	{
		SetPlayerHiddenArray();
//		if (Network.peerType != NetworkPeerType.Disconnected) {
//			NetViewCom.RPC("XKPlayerSendHandlePlayerHiddenArray", RPCMode.OthersBuffered);
//		}
	}
	
	void SetPlayerHiddenArray()
	{
//		int max = PlayerHiddenArray.Length;
//		for (int i = 0; i < max; i++) {
//			PlayerHiddenArray[i].SetActive(false);
//		}
	}
	
	[RPC] void XKPlayerSendHandlePlayerHiddenArray()
	{
		SetPlayerHiddenArray();
	}

	/**
	 * key == 1 -> 使主角摄像机依附于父级摄像机并且停止跟踪.
	 */
	public void SetPlayerCameraTran(int key = 0)
	{
		PlayerCamera.SetPlayerCameraTran(key);
//		if (PlayerSt == PlayerTypeEnum.TanKe) {
//			if (key == 1 && PaoTaiScript != null) {
//				if (PlayerZhiShengJiScript != null) {
//					Debug.Log("Unity:"+"SetPlayerCameraTran -> fix tanKe info...");
//					Transform tkTran = PlayerZhiShengJiScript.transform;
//					tkTran.parent = transform;
//					tkTran.localPosition = Vector3.zero;
//					tkTran.localEulerAngles = Vector3.zero;
//				}
//				PaoTaiScript.ResetTransformRotation();
//			}
//		}
	}

	public void ClosePlayerRigidbody()
	{
		//PlayerZhiShengJiScript.ClosePlayerRigidbody();
	}

	public bool GetIsHandleRpc()
	{
		return IsHandleRpc;
	}

    /// <summary>
    /// 设置游戏镜头微动的动画开关.
    /// </summary>
    public void SetCameraMoveAni(bool isMove)
    {
        if (m_CameraMoveAni != null)
        {
            Debug.Log("Unity: SetCameraMoveAni *************************** isMove === " + isMove);
            if (isMove != IsCameraPosWeiDong)
            {
                if (isMove == true)
                {
                    m_CameraWeiDongPos = transform.position;
                }
                IsCameraPosWeiDong = isMove;
                m_CameraMoveAni.SetBool("IsMove", isMove);
            }
        }
        else
        {
            Debug.LogWarning("SetCameraMoveAni -> m_CameraMoveAni was null...............");
        }
    }

    /// <summary>
    /// 摄像机位置微动.
    /// </summary>
    bool IsCameraPosWeiDong = false;
    Vector3 m_CameraWeiDongPos;
    /// <summary>
    /// 更新镜头微动坐标.
    /// </summary>
    void UpdateCameraWeiDongPos()
    {
        if (m_CameraMoveAni == null)
        {
            return;
        }

        if (IsCameraPosWeiDong == false)
        {
            return;
        }

        Vector3 forwardVal = transform.forward;
        forwardVal.y = 0f;
        transform.position = m_CameraWeiDongPos + forwardVal * m_CameraMoveAni.transform.localPosition.z;
    }

    //********************************************************************************************************//
    /// <summary>
    /// 攻击已经得到战车优惠券的跟踪弹配置信息.
    /// </summary>
    [System.Serializable]
    public class GenZongDanData
    {
        /// <summary>
        /// 间隔时间.
        /// </summary>
        public float TimeVal = 5f;
        float TimeLast = 0f;
        /// <summary>
        /// 子弹产生点.
        /// </summary>
        public Transform[] SpawnPointArray01;
        /// <summary>
        /// 子弹产生点.
        /// </summary>
        public Transform[] SpawnPointArray02;
        /// <summary>
        /// 跟踪弹预制.
        /// </summary>
        public GameObject GenZongDanPrefab;
        /// <summary>
        /// 获得战车代金券的玩家列表信息.
        /// </summary>
        List<XKPlayerMoveCtrl> PlayerComList = new List<XKPlayerMoveCtrl>();
        public void AddPlayerCom(XKPlayerMoveCtrl playerCom)
        {
            if (playerCom == null)
            {
                return;
            }

            if (PlayerComList.Contains(playerCom) == false)
            {
                PlayerComList.Add(playerCom);
            }
            playerCom.AddZhanCheDaiJinQuanCount();
        }
        
        public void RemovePlayerCom(XKPlayerMoveCtrl playerCom)
        {
            if (playerCom == null)
            {
                return;
            }

            if (PlayerComList.Contains(playerCom) == true)
            {
                PlayerComList.Remove(playerCom);
            }
            playerCom.ResetZhanCheDaiJinQuanCount();
        }

        /// <summary>
        /// 创建跟踪弹.
        /// </summary>
        internal void CheckIsCreatGenZongDan()
        {
            if (Time.time - TimeLast < TimeVal)
            {
                return;
            }
            TimeLast = Time.time;

            if (PlayerComList.Count <= 0)
            {
                return;
            }

            int randVal = Random.Range(0, 100) % PlayerComList.Count;
            XKPlayerMoveCtrl playerCom = PlayerComList[randVal];
            if (playerCom == null)
            {
                return;
            }

            int max = 0;
            int zhanCheDaiJinQuanCount = playerCom.GetZhanCheDaiJinQuanCount();
            if (zhanCheDaiJinQuanCount > 1)
            {
                max = 2;
            }
            else if(zhanCheDaiJinQuanCount == 1)
            {
                max = 1;
            }

            Transform point = null;
            for (int i = 0; i < max; i++)
            {
                if (i == 0)
                {
                    if (SpawnPointArray01.Length == 0)
                    {
                        continue;
                    }
                    randVal = Random.Range(0, 100) % SpawnPointArray01.Length;
                    point = SpawnPointArray01[randVal];
                }
                else
                {
                    if (SpawnPointArray02.Length == 0)
                    {
                        continue;
                    }
                    randVal = Random.Range(0, 100) % SpawnPointArray02.Length;
                    point = SpawnPointArray02[randVal];
                }
                CreatGenZongDanAmmo(point, playerCom);
            }
        }

        /// <summary>
        /// 创建跟踪弹.
        /// </summary>
        void CreatGenZongDanAmmo(Transform spawnPoint, XKPlayerMoveCtrl playerCom)
        {
            if (GenZongDanPrefab == null || spawnPoint == null || playerCom == null)
            {
                return;
            }

            GameObject obj = (GameObject)Instantiate(GenZongDanPrefab, spawnPoint.position, spawnPoint.rotation);
            NpcAmmoCtrl ammoScript = obj.GetComponent<NpcAmmoCtrl>();
            if (ammoScript != null)
            {
                ammoScript.SetIsAimFeiJiPlayer(false);
                ammoScript.SetGenZongDanAimTarget(playerCom);
            }
        }
    }
    /// <summary>
    /// 攻击获得战车代金券的跟踪弹数据.
    /// </summary>
    public GenZongDanData m_GenZongDanData;
    void UpdateCheckIsCreatGenZongDan()
    {
        if (m_GenZongDanData != null)
        {
            m_GenZongDanData.CheckIsCreatGenZongDan();
        }
    }
    
    /// <summary>
    /// 添加获得战车代金券的玩家信息.
    /// </summary>
    public void AddGetZhanCheDaiJinQuanPlayer(XKPlayerMoveCtrl playerCom)
    {
        if (m_GenZongDanData != null)
        {
            m_GenZongDanData.AddPlayerCom(playerCom);
        }
    }

    /// <summary>
    /// 删除获得战车代金券的玩家信息.
    /// </summary>
    public void RemoveGetZhanCheDaiJinQuanPlayer(XKPlayerMoveCtrl playerCom)
    {
        if (m_GenZongDanData != null)
        {
            m_GenZongDanData.RemovePlayerCom(playerCom);
        }
    }
}